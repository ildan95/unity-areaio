using UnityEngine;
using System.Collections;
using System.Collections.Generic;



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
        
    }
    public abstract void reservateBlock(AreaBlock block);
    public abstract void addBlocks(List<AreaBlock> toAddBlock);
    public abstract void addReservedToOwn();
    public abstract void addBlocks(int x, int y, int size);
    public abstract void addInitBlocks(int size);
    public abstract void clear();
}
