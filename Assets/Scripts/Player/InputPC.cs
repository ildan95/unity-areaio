using UnityEngine;

public class InputPC:IInput {

    public override bool isUpKey()
    {
        return Input.GetKeyDown(KeyCode.W);
    }
    public override bool isDownKey()
    {
        return Input.GetKeyDown(KeyCode.S);
    }
    public override bool isRightKey()
    {
        return Input.GetKeyDown(KeyCode.D);
    }
    public override bool isLeftKey()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

}
