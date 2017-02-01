using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Moving
{
    Up,
    Down,
    Right,
    Left
}

public class InputAndroid : IInput{

    Vector2 beganPosition;
    Moving move = Moving.Up;

    public void Update()
    {
        
        if (Input.touchCount > 0) {

            if (Input.GetTouch(0).phase == TouchPhase.Began)
                beganPosition = Input.GetTouch(0).position;
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                var deltaPos = Input.GetTouch(0).position - beganPosition;
                if (Math.Abs(deltaPos.y) >= Math.Abs(deltaPos.x))
                {
                    if (deltaPos.y >= 0)
                        move = Moving.Up;
                    else
                        move = Moving.Down;
                }
                else
                {
                    if (deltaPos.x > 0)
                        move = Moving.Right;
                    else
                        move = Moving.Left;
                }
            }
        }
    }

    public bool isUpKey()
    {
        return move == Moving.Up;
    }
    public bool isDownKey()
    {
        return move == Moving.Down;
    }
    public bool isRightKey()
    {
        return move == Moving.Right;
    }
    public bool isLeftKey()
    {
        return move == Moving.Left;
    }
}
