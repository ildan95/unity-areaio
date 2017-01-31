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
    public Color color;
    public GameObject self;
    public int X;
    public int Z;

    public float Percent { get; protected set; }

    public bool isAlive = true;


    public IPlayerModel(GameObject _self)
    {
        self = _self;
    }
    public abstract void reservateBlock(AreaBlock block);
    public abstract void addBlocks(List<AreaBlock> toAddBlock);
    public abstract void addReservedToOwn();
    public abstract void addBlocks(int x, int z, int size);
    public abstract void addInitBlocks(int size);
}
