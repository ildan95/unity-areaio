using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : Photon.PunBehaviour {

    public bool isPC = true;
    private IInput _input;
    public PlayerModel model;
    public PlayerView view;

    public float moveCooldown = 0.1f;
    private float moveCooldownTime = 0;
    public void Awake()
    {
        Debug.Log("Player controller awake start");

        model = new PlayerModel(gameObject);
        Debug.Log("Choosing input");
#if UNITY_EDITOR
        Debug.Log("Editor input");
        _input = new InputPC();
#elif UNITY_STANDALONE
        Debug.Log("Standalone input");
        _input = new InputPC();
#elif UNITY_ANDROID
        Debug.Log("Android input");
        _input = new InputAndroid();
        Debug.Log("End android input");
#endif
        Debug.Log("Player controller awake end");

    }

    void Run()
    {
        model.setHueColor(generateRandomHueColor());
        photonView.RPC("setHueColorRPC", PhotonTargets.OthersBuffered, model.hueColor);
        transform.localPosition = Area.randomFreeSquare(2);
        model.X = (int)Math.Round(transform.position.x, 0);
        model.Y = (int)Math.Round(transform.position.y, 0);
        model.addInitBlocks(2);
        model.isReady = true;
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        if (!model.isAlive)
            return;

        if (!model.isReady)
            Run();

        _input.Update();
        model.direction = checkDirection();
        if (moveCooldownTime >= moveCooldown)
        {
            move();
            updateBlock();
            view.UpdatePos();
            moveCooldownTime = 0;
            GameObject.Find("Canvas/PercentStatistic/PlayerPercentStatistic/Percent").GetComponent<Text>().text = string.Format("{0} %", Math.Round(model.Percent,2));
        }
        else
            moveCooldownTime += Time.deltaTime;
    }

    private void updateBlock() {
        if (Area.isOut(model.X, model.Y))
            return;
        AreaBlock block = Area.blocks[model.X][model.Y];
        if (block.PreOwnerH != -1)
        {
            if (block.PreOwnerH == model.hueColor)
            {
                photonView.RPC("died", PhotonTargets.Others);
                died();
            }
            else
            {
                kill(block.PreOwnerH);
                model.reservateBlock(block);
            }
        }
        else
        {
            if (block.OwnerH != model.hueColor)
                model.reservateBlock(block);
            else
                model.addReservedToOwn();
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

        if (Area.isOut(model.X, model.Y))
        {
            photonView.RPC("died", PhotonTargets.Others);
            died();
        }
        
        //photonView.RPC("updatePos", PhotonTargets.Others, model.X, model.Y);
    }

    private void stop()
    {
        model.speed = 0;
    }

    [PunRPC]
    public void died()
    {
        Debug.Log(model.hueColor.ToString() + " is dead");
        model.isAlive = false;
        stop();
        if (photonView.isMine)
        {
            StartCoroutine(FreeBlocksCoroutine(model.blocksId, model.hueColor));
            model.setFreeBlocks();
        }

        model.X = -100;
        model.Y = -100;
    }

    private void kill(float playerH)
    {
        Debug.Log("Kill start");
        Debug.Log("Players count: " + FindObjectsOfType<PlayerController>().Length.ToString());
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            Debug.Log("find player color: " + player.model.hueColor.ToString() + ", to kill player color: " + playerH.ToString());
            if (player.model.hueColor == playerH)
            {
                Debug.Log("Killed player with color: " + playerH.ToString());
                player.photonView.RPC("died", PhotonTargets.Others);
                player.died();
                model.Kills++;
                return;
            }
        }
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
        List<int> ids = AreaBlock.toListInt(Ids);
        foreach (var Id in ids)
        {
            Area.blocks[Id / Area.rows][Id % Area.rows].setOwner(playerH);
        }
    }

    [PunRPC]
    private void setBlockPreOwnerRPC(int X, int Y, float playerH)
    {
        Area.blocks[X][Y].setPreOwner(playerH);
    }

    [PunRPC]
    private void setBlocksFreeRPC(string Ids, float playerH)
    {
        List<int> ids = AreaBlock.toListInt(Ids);
        //foreach (var Id in ids)
        //{
        //    Area.blocks[Id / Area.rows][Id % Area.rows].setFree(playerH);
        //}
        StartCoroutine(FreeBlocksCoroutine(ids, playerH));
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
    }

    //[PunRPC]
    //public void updatePos(int X, int Y)
    //{
    //    model.X = X;
    //    model.Y = Y;
    //    view.UpdatePos();
    //}


    IEnumerator FreeBlocksCoroutine(List<int> ids, float playerH)
    {
        foreach (var Id in ids)
        {
            Area.blocks[Id / Area.rows][Id % Area.rows].setFree(playerH);
            yield return null;
        }
    }
}
