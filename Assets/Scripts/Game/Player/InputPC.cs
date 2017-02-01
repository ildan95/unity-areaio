using UnityEngine;

public class InputPC:IInput {

    public bool isUpKey()
    {
        return Input.GetKeyDown(KeyCode.W);
    }
    public bool isDownKey()
    {
        return Input.GetKeyDown(KeyCode.S);
    }
    public bool isRightKey()
    {
        return Input.GetKeyDown(KeyCode.D);
    }
    public bool isLeftKey()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public void Update() { }

}
