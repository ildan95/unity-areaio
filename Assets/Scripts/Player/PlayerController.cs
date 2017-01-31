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
#if UNITY_EDITOR
        _input = new InputPC();
#elif UNITY_STANDALONE
        _input = new InputPC();
#elif UNITY_ANDROID
        _input = new InputAndroid();
#endif
    }

    void Start()
    {
        playerModel = new BasicPlayerModel(gameObject, Color.red);
        playerModel.X = (int)Math.Round(transform.position.x, 0);
        playerModel.Z = (int)Math.Round(transform.position.z, 0);
        gameObject.GetComponent<MeshRenderer>().material.color = playerModel.color;
        playerModel.addInitBlocks(2);
    }

    void Update()
    {
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
        if (Area.isOut(playerModel.X, playerModel.Z))
            return;
        AreaBlock block = Area.blocks[playerModel.X][playerModel.Z];
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
                playerModel.Z += playerModel.speed;
                break;
            case Direction.Down:
                playerModel.Z -= playerModel.speed;
                break;
            case Direction.Right:
                playerModel.X += playerModel.speed;
                break;
            case Direction.Left:
                playerModel.X -= playerModel.speed;
                break;
        }
        
        if (Area.isOut(playerModel.X, playerModel.Z))
            died();
        else
            transform.localPosition = new Vector3(playerModel.X, 2, playerModel.Z);
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
