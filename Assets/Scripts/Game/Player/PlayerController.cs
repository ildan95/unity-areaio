using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class PlayerController
{
    public delegate void MovedHandler();
    public delegate void DeathHandler();
    public delegate void KillHandler(float killerHueColor, float targetHueColor);
    public delegate void ReadyHandler();

    public delegate void BlockPreOwnedHandler(AreaBlock block);
    public delegate void BlocksOwnedHandler();
    public event BlockPreOwnedHandler BlockPreOwnedEvent;
    public event BlocksOwnedHandler BlocksOwnedEvent;

    public delegate void ChangedDirectionHandler(Direction direction);
    public event ChangedDirectionHandler ChangedDirectionEvent;

    public event ReadyHandler ReadyEvent;
    public event MovedHandler MovedEvent;
    public event DeathHandler DeathEvent;
    public static event KillHandler KillEvent;
}

public partial class PlayerController : Photon.PunBehaviour {

    public bool isPC = true;
    public bool isBot = false;
    private IInput _input;
    public PlayerModel model;
    public PlayerView view;


    bool isFirstUpdate = true;
    Direction lastDirection = Direction.Up;

    void OnEnable()
    {
        DeathEvent += PlayerController_DeathEvent;
        KillEvent += PrintKillEvent;
        KillEvent += kill;
        MovedEvent += PlayerController_MovedEvent;
        if (photonView.isMine)
        {
            Debug.Log("Next Move Event ADD");
            GameManager.NextMoveEvent += PlayerController_NextMoveEvent;
        }
    }

    private void PlayerController_MovedEvent()
    {
        if (photonView.isMine)
            photonView.RPC("updatePos", PhotonTargets.Others, model.X, model.Y);
    }

    void OnDisable()
    {
        DeathEvent -= PlayerController_DeathEvent;
        KillEvent -= PrintKillEvent;
        KillEvent -= kill;
        MovedEvent -= PlayerController_MovedEvent;
        GameManager.NextMoveEvent -= PlayerController_NextMoveEvent;
    }

    private void PlayerController_DeathEvent()
    {
        dead();
        photonView.RPC("dead", PhotonTargets.Others);
        photonView.RPC("removeHueColor", PhotonTargets.OthersBuffered, model.hueColor);
        PrintDeathEvent();

    }

    private void PlayerController_NextMoveEvent()
    {
        updateBlock();
        move();
        UpdateGUI();
    }

    public void Awake()
    {
        model = new PlayerModel(gameObject);
#if UNITY_EDITOR
        _input = new InputPC();
#elif UNITY_STANDALONE
        Debug.Log("Standalone input");
        _input = new InputPC();
#elif UNITY_ANDROID
        Debug.Log("Android input");
        _input = new InputAndroid();
        Debug.Log("End android input");
#endif
    }

    void Run()
    {
        model.setHueColor(generateRandomHueColor());
        photonView.RPC("setHueColorRPC", PhotonTargets.OthersBuffered, model.hueColor);
        transform.position = Area.randomFreeSquare(2);
        model.X = (int)Math.Round(transform.position.x, 0);
        model.Y = (int)Math.Round(transform.position.y, 0);
        model.addInitBlocks(2);
        if (ReadyEvent != null)
            ReadyEvent();
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            Run();
        }

        _input.Update();
        model.direction = checkDirection();
    }

    void UpdateGUI()
    {
        GameObject.Find("Canvas/PercentStatistic/PlayerPercentStatistic/Percent").GetComponent<Text>().text = string.Format("{0} %", Math.Round(model.Percent, 2));
    }

    

    public void updateBlock() {
        if (Area.isOut(model.X, model.Y))
        {
            if (DeathEvent != null)
                DeathEvent();
            return;
        }
        AreaBlock block = Area.blocks[model.X][model.Y];
        if (block.PreOwnerH != -1)
        {

            if (block.PreOwnerH == model.hueColor)
            {
                if (DeathEvent != null)
                    DeathEvent();
            }
            else
            {
                if (KillEvent != null)
                    KillEvent(model.hueColor, block.PreOwnerH);
                if (BlockPreOwnedEvent != null)
                    BlockPreOwnedEvent(block);
            }
        }
        else
        {
            if (!block.isOwn(model.color))
            {
                if (BlockPreOwnedEvent != null)
                    BlockPreOwnedEvent(block);
            }
            else
            {
                if (BlocksOwnedEvent != null)
                    BlocksOwnedEvent();
            }
        }
    }

    private Direction checkDirection()
    {
        if (_input.isUpKey())
            return Direction.Up;
        if (_input.isDownKey())
            return Direction.Down;
        if (_input.isRightKey())
            return Direction.Right;
        if (_input.isLeftKey())
            return Direction.Left;
        else
           return model.direction;
    }

    private void move()
    {
            switch (model.direction)
            {
                case Direction.Up:
                    model.Y += model.speed;
                    break;
                case Direction.Down:
                    model.Y -= model.speed;
                    break;
                case Direction.Right:
                    model.X += model.speed;
                    break;
                case Direction.Left:
                    model.X -= model.speed;
                    break;
            }
        
        if (lastDirection != model.direction)
        {
            if (ChangedDirectionEvent != null)
                ChangedDirectionEvent(model.direction);
            lastDirection = model.direction;
        }
        if (MovedEvent != null)
            MovedEvent();
    }

    private void stop()
    {
        model.speed = 0;
    }

    [PunRPC]
    public void dead()
    {
        Debug.Log(model.hueColor.ToString() + " is dead");
        model.isAlive = false;
        stop();
        view.clearLineRenderer();
        //model.setFreeBlocks();
        if (photonView.isMine)
            photonView.RPC("setBlocksFreeRPC", PhotonTargets.All, AreaBlock.toStr(model.reservatedBlocks), model.hueColor);
        gameObject.SetActive(false);
    }

    private void kill(float killerHueColor, float targetHueColor)
    {
        if (killerHueColor == model.hueColor)
            model.Kills++;
        else if (targetHueColor == model.hueColor)
            photonView.RPC("callDeathEvent", PhotonPlayer.Find(photonView.ownerId));

    }

    [PunRPC]
    public void callDeathEvent()
    {
        if (DeathEvent != null)
            DeathEvent();
    }

    public float generateRandomHueColor()
    {
        float hue = 0;
        int colorsCount = GameManager.hueColorsInGame.Count;
        Debug.Log("Colors count: " + colorsCount.ToString());
        int count = 0;
        foreach (var hueColor in GameManager.hueColorsInGame)
            Debug.Log("color in game: " + hueColor.ToString());
        while (count < colorsCount)
        {
            count = 0;
            hue = UnityEngine.Random.Range(0.0f, 1.0f);
            
            foreach (var hueColor in GameManager.hueColorsInGame)
            {
                if (Mathf.Abs(hueColor - hue) > 0.05)
                    count++;
                else
                    break;
            }
        }
        photonView.RPC("addHueColor", PhotonTargets.OthersBuffered, hue);
        Debug.Log("Choosed color hue: " + hue.ToString());
        return hue;
    }

    [PunRPC]
    private void setBlocksOwnerRPC(string Ids, float playerH)
    {
        view.clearLineRenderer();
        List<int> ids = AreaBlock.toListInt(Ids);
        foreach (var Id in ids)
            Area.blocks[Id / Area.rows][Id % Area.rows].setOwner(model.material);
    }

    [PunRPC]
    private void setBlockPreOwnerRPC(int X, int Y, float playerH)
    {
        Area.blocks[X][Y].PreOwnerH = playerH;
        //view.addPoint(new Vector2(X,Y));
    }

    [PunRPC]
    private void setBlocksFreeRPC(string ReservatedIds, float playerH)
    {
        model.material.color = Color.white;
        List<int> ids = AreaBlock.toListInt(ReservatedIds);
        foreach (var id in ids)
            Area.blocks[id / Area.rows][id % Area.rows].PreOwnerH = -1;
    }

    [PunRPC]
    public void addHueColor(float hueColor)
    {
        GameManager.hueColorsInGame.Add(hueColor);
    }

    [PunRPC]
    public void removeHueColor(float hueColor)
    {
        GameManager.hueColorsInGame.Remove(hueColor);
    }

    [PunRPC]
    public void setHueColorRPC(float hueColor)
    {
        model.setHueColor(hueColor);
        view.lineRenderer.startColor = view.lineRenderer.endColor = Color.HSVToRGB(model.hueColor, 0.4f, 1);
    }

    [PunRPC]
    public void updatePos(int X, int Y)
    {
        model.X = X;
        model.Y = Y;
        if (MovedEvent != null)
            MovedEvent();
        view.UpdatePos();
    }

    [PunRPC]
    public void initPlayerInfo(string blocksId, string reservatedBlocksId) 
    {
        foreach (var block in AreaBlock.toListBlock(blocksId))
            block.setOwner(model.material);
        view.clearLineRenderer();
        foreach (var block in AreaBlock.toListBlock(reservatedBlocksId))
        {
            block.PreOwnerH = model.hueColor;
            view.addPoint(block.transform.position);
        }
    }
    
    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.isMasterClient || photonView.isMine)
            return;
        foreach (var player in FindObjectsOfType<PlayerController>())
            player.photonView.RPC("initPlayerInfo", info.sender, AreaBlock.toStr(player.model.blocksId), AreaBlock.toStr(player.model.reservatedBlocks));
    }

    public void PrintDeathEvent()
    {
        Debug.Log("Death Event Called.");
    }
    public void PrintKillEvent(float k, float t)
    {
        Debug.Log("Kill Event Called. Killer: "+k+", Target: "+t);
    }

    void OnDestroy()
    {
        Debug.Log("Player Controller destroy.");
    }
}
