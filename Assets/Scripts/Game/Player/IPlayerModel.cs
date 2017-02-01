using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
    Up,
    Down,
    Right,
    Left

} 

public abstract class IPlayerModel {
    public int speed = 1;
    public Direction direction = Direction.Up;
    public float hueColor;
    public Color color;
    public GameObject self;
    public int X;
    public int Y;

    public float Percent { get; protected set; }

    public bool isAlive = true;


    public IPlayerModel(GameObject _self, float _hueColor)
    {
        self = _self;
        hueColor = _hueColor;
        color = Color.HSVToRGB(hueColor, 1, 0.5f);
    }
    public abstract void reservateBlock(AreaBlock block);
    public abstract void addBlocks(List<AreaBlock> toAddBlock);
    public abstract void addReservedToOwn();
    public abstract void addBlocks(int x, int y, int size);
    public abstract void addInitBlocks(int size);
}
