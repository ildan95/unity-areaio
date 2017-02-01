using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public bool isPC = true;
    private IInput _input;
    public IPlayerModel playerModel;


    public float moveCooldown = 0.1f;
    private float moveCooldownTime = 0;
    public void Awake()
    {
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
    }

    void Start()
    {
        playerModel = new BasicPlayerModel(gameObject, GameManager.randomHueColor());
        playerModel.X = (int)Math.Round(transform.position.x, 0);
        playerModel.Y = (int)Math.Round(transform.position.y, 0);
        gameObject.GetComponent<SpriteRenderer>().color = playerModel.color;
        playerModel.addInitBlocks(2);
    }

    void Update()
    {
        _input.Update();
        playerModel.direction = checkDirection();
        if (moveCooldownTime >= moveCooldown)
        {
            move();
            updateBlock();
            moveCooldownTime = 0;
            GameObject.Find("Canvas/PlayerPercentStatistic/Percent").GetComponent<Text>().text = string.Format("{0} %", Math.Round(playerModel.Percent,2));
        }
        else
        {
            moveCooldownTime += Time.deltaTime;
        }
    }

    public void updateBlock() {
        if (Area.isOut(playerModel.X, playerModel.Y))
            return;
        AreaBlock block = Area.blocks[playerModel.X][playerModel.Y];
        if (block.PreOwner != null)
        {
            if (block.PreOwner == gameObject)
                died();
            else
                kill(block.PreOwner);
        }
        else
        {
            if (block.Owner != gameObject)
                playerModel.reservateBlock(block);
            else
                playerModel.addReservedToOwn();
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
           return playerModel.direction;
    }

    private void move()
    {
        switch (playerModel.direction)
        {
            case Direction.Up:
                playerModel.Y += playerModel.speed;
                break;
            case Direction.Down:
                playerModel.Y -= playerModel.speed;
                break;
            case Direction.Right:
                playerModel.X += playerModel.speed;
                break;
            case Direction.Left:
                playerModel.X -= playerModel.speed;
                break;
        }
        
        if (Area.isOut(playerModel.X, playerModel.Y))
            died();
        else
            transform.localPosition = new Vector3(playerModel.X, playerModel.Y,0);
    }

    private void stop()
    {
        playerModel.speed = 0;

    }

    private void died()
    {
        stop();
    }

    private void kill(GameObject obj)
    {
        obj.GetComponent<PlayerController>().died();
    }
}
