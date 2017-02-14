using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : IInput {

    private PlayerModel model;
    private Moving move = Moving.Up;
    private List<Vector3> directions = new List<Vector3>() { new Vector3(0, 1), new Vector3(0, -1), new Vector3(1, 0), new Vector3(-1, 0) };
    private int oneSideSteps = 0;


    public AI(PlayerModel _model)
    {
        model = _model;
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

    public void Update () {
		if (isNextBorder() || oneSideSteps > 2)
        {
            oneSideSteps = 0;
            if (move == Moving.Up)
                move = Moving.Right;
            if (move == Moving.Right)
                move = Moving.Down;
            if (move == Moving.Down)
                move = Moving.Left;
            if (move == Moving.Left)
                move = Moving.Up;
        }
        oneSideSteps++;
	}

    private bool isNextBorder()
    {
        return Area.isOut( directions[(int)model.direction] + new Vector3(model.X, model.Y));
    }
}
