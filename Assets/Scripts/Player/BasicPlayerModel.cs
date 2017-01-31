using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicPlayerModel : IPlayerModel
{
    private List<AreaBlock> blocks = new List<AreaBlock>();
    public int xMin { get; private set; }
    public int xMax { get; private set; }
    public int zMin { get; private set; }
    public int zMax { get; private set; }
    public List<AreaBlock> reservatedBlocks = new List<AreaBlock>();
    

    public BasicPlayerModel(GameObject _self, Color _color):base(_self)
    {
        color = _color;
        xMin = Area.rows;
        zMin = Area.cols;
    }

    public void addBlock(AreaBlock block)
    {
        if (block.X > xMax)
            xMax = block.X;
        else if (block.X < xMin)
            xMin = block.X;
        if (block.Z > zMax)
            zMax = block.Z;
        if (block.Z < zMin)
            zMin = block.Z;
        blocks.Add(block);
        block.setOwner(self);
        
    }

    public override void addBlocks(List<AreaBlock> toAddBlock)
    {
        foreach (var block in toAddBlock)
        {
            addBlock(block);
        }  
    }

    private void addBlocksAndClear(List<AreaBlock> toAddBlock)
    {
        addBlocks(toAddBlock);
        toAddBlock.Clear();
    }

    public override void addReservedToOwn()
    {
        if (reservatedBlocks.Count == 0)
            return;
        addBlocks(reservatedBlocks);
        checkClosedArea(reservatedBlocks);
        reservatedBlocks.Clear();
        Percent = (float)blocks.Count / Area.blocksCount * 100;
    }

    public override void reservateBlock(AreaBlock block)
    {
        reservatedBlocks.Add(block);
        block.setPreOwner(self);
    }

    public override void addInitBlocks(int size)
    {
        for (int i = X; i < X + size; i++)
            for (int j = Z; j < Z + size; j++)
            {
                addBlock(Area.blocks[i][j]);
            }
    }

    public override void addBlocks(int x, int z, int size)
    {
        for (int i = x; i < x + size; i++)
            for (int j = z; j < z + size; j++)
            {
                addBlock(Area.blocks[i][j]);
            }
    }

    private void checkClosedArea(List<AreaBlock> addedBlocks)
    {
        if (addedBlocks.Count == 0)
            return;
        foreach (var addedBlock in addedBlocks)
        {
            int i = addedBlock.X;
            int j = addedBlock.Z;
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
                    addBlocksAndClear(looked);
                    break;
                }
            }
        }
    }

    private bool isAroundOwn(AreaBlock block, ref List<AreaBlock> looked)
    {
        looked.Add(block);
        int i = block.X;
        int j = block.Z;

        if (i == 0 || i == Area.rows-1 || j == 0 || j == Area.cols-1)
            return false;
        if (xMin > i || i > xMax || zMin > j || j > zMax)
            return false;

        var b = Area.blocks[i + 1][j];
        if (!b.isOwn(self) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i - 1][j];
        if (!b.isOwn(self) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i][j+1];
        if (!b.isOwn(self) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        b = Area.blocks[i][j-1];
        if (!b.isOwn(self) && !looked.Contains(b))
            if (!isAroundOwn(b, ref looked))
                return false;
        return true;
    }

}
