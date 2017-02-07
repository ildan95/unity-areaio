using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Direction
{
    Up,
    Down,
    Right,
    Left

}

public class PlayerModel
{
    public List<int> blocksId = new List<int>();
    public int xMin { get; private set; }
    public int xMax { get; private set; }
    public int yMin { get; private set; }
    public int yMax { get; private set; }
    public List<AreaBlock> reservatedBlocks = new List<AreaBlock>();

    public int speed = 1;
    public Direction direction = Direction.Up;
    public float hueColor;
    public Color color;
    public GameObject self;
    public int X;
    public int Y;

    public float Percent { get; protected set; }
    public int Kills { get; set; }

    public bool isReady = false;

    public bool isAlive = true;


    public PlayerModel(GameObject _self)
    {
        self = _self;
        xMin = Area.rows;
        yMin = Area.cols;
    }

    public void setHueColor(float _hueColor)
    {
        hueColor = _hueColor;
        color = Color.HSVToRGB(hueColor, 1, 0.7f);
        self.GetComponent<SpriteRenderer>().color = color;
    }

    private void addBlocks(List<int> toAddBlocksId)
    {
        foreach (var blockId in toAddBlocksId)
        {
            UpdateBlockMinMax(blockId);
            blocksId.Add(blockId);
            Area.blocks[blockId / Area.rows][blockId % Area.rows].setOwner(hueColor);
        }  
        self.GetComponent<PlayerController>().photonView.RPC("setBlocksOwnerRPC", PhotonTargets.OthersBuffered, AreaBlock.toStr(toAddBlocksId), hueColor);
    }

    private void addBlocksAndClear(List<int> toAddBlock)
    {
        addBlocks(toAddBlock);
        toAddBlock.Clear();
    }

    public void addReservedToOwn()
    {
        if (reservatedBlocks.Count == 0)
            return;
        addBlocks(AreaBlock.toListInt(reservatedBlocks));
        checkClosedArea(reservatedBlocks);
        reservatedBlocks.Clear();
        Percent = (float)blocksId.Count / Area.blocksCount * 100;
        updateCameraSize();
    }

    public void reservateBlock(AreaBlock block)
    {
        reservatedBlocks.Add(block);
        block.setPreOwner(hueColor);
        self.GetComponent<PlayerController>().photonView.RPC("setBlockPreOwnerRPC", PhotonTargets.OthersBuffered, block.X, block.Y, hueColor);
    }

    public void setFreeBlocks()
    {
        Debug.Log("set free blocks. Hue color: " + hueColor.ToString());

        self.GetComponent<PlayerController>().photonView.RPC("setBlocksFreeRPC", PhotonTargets.OthersBuffered, AreaBlock.toStr(reservatedBlocks), hueColor);
        self.GetComponent<PlayerController>().photonView.RPC("setBlocksFreeRPC", PhotonTargets.OthersBuffered, AreaBlock.toStr(blocksId), hueColor);

        
        foreach (var block in reservatedBlocks)
            block.setFree(hueColor);
        //foreach (var blockId in blocksId)
            //Area.blocks[blockId / Area.rows][blockId % Area.rows].setFree(hueColor);

        
    }

    public void addInitBlocks(int size)
    {
        List<int> blocksId = new List<int>();
        for (int i = X; i < X + size; i++)
            for (int j = Y; j < Y + size; j++)
            {
                blocksId.Add(i * Area.rows + j);
            }
        addBlocks(blocksId);
    }

    private void checkClosedArea(List<AreaBlock> addedBlocks)
    {
        if (addedBlocks.Count == 0)
            return;
        foreach (var addedBlock in addedBlocks)
        {
            int i = addedBlock.X;
            int j = addedBlock.Y;
            List<AreaBlock> blocksToCheck = new List<AreaBlock>();
            if (!(i == 0 || i == Area.rows - 1))
                blocksToCheck.Add(Area.blocks[i + 1][j]);
            if (!(i == 0 || i == Area.rows - 1))
                blocksToCheck.Add(Area.blocks[i - 1][j]);
            if (!(j == 0 || j == Area.cols - 1))
                blocksToCheck.Add(Area.blocks[i][j + 1]);
            if (!(j == 0 || j == Area.cols - 1))
                blocksToCheck.Add(Area.blocks[i][j - 1]);
            foreach (var block in blocksToCheck)
            {
                if (!block.isFree)
                    continue;
                List<AreaBlock> looked = new List<AreaBlock>();
                if (isAroundOwn(block, ref looked))
                {
                    addBlocksAndClear(AreaBlock.toListInt(looked));
                    break;
                }
            }
        }
    }

    private bool isAroundOwn(AreaBlock block, ref List<AreaBlock> looked)
    {
        looked.Add(block);
        int i = block.X;
        int j = block.Y;

        if (i == 0 || i == Area.rows-1 || j == 0 || j == Area.cols-1)
            return false;
        if (xMin > i || i > xMax || yMin > j || j > yMax)
            return false;

        var b = Area.blocks[i + 1][j];
        if (!b.isOwn(hueColor) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i - 1][j];
        if (!b.isOwn(hueColor) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i][j+1];
        if (!b.isOwn(hueColor) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i][j-1];
        if (!b.isOwn(hueColor) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        return true;
    }

    public int TotalMoneyWin()
    {
        return (int)Math.Round(Percent * LocalData.GoldPerAreaPercent + Kills * LocalData.GoldPerKill, 0);
    }

    public void UpdatePlayerInfo()
    {
        LocalData.BestArea = LocalData.BestArea > Percent ? LocalData.BestArea : Percent;
        LocalData.SaveKills(Kills);
        LocalData.SaveMoney(TotalMoneyWin());
    }

    private void UpdateBlockMinMax(int blockId)
    {
        var blockX = blockId / Area.rows;
        var blockY = blockId % Area.rows;
        if (blockX > xMax)
            xMax = blockX;
        else if (blockX < xMin)
            xMin = blockX;
        if (blockY > yMax)
            yMax = blockY;
        if (blockY < yMin)
            yMin = blockY;
    }

    private void updateCameraSize()
    {
        int size = 5;
        if (blocksId.Count > 500)
            size = 15;
        else if (blocksId.Count > 100)
            size = 12;
        else if (blocksId.Count > 50)
            size = 10;
        else if (blocksId.Count > 10)
            size = 7;
        GameObject.Find("Camera").GetComponent<Camera>().orthographicSize = size;
    }
}
