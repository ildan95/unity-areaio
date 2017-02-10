using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaBlock : MonoBehaviour {

    public Material sharedMaterial;

    public float PreOwnerH { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Id { get; private set; }

    public bool isFree
    {
        get
        {
            return sharedMaterial.color == Color.white && PreOwnerH == -1;
        }
        private set { }
    }

    public bool isOwn(Color playerColor)
    {
        return sharedMaterial.color==playerColor;
    }

    void Awake () {
        PreOwnerH = -1;
        sharedMaterial = GetComponent<SpriteRenderer>().sharedMaterial;
        X = (int)System.Math.Round(transform.position.x, 0);
        Y = (int)System.Math.Round(transform.position.y, 0);
        Id = X * Area.rows + Y;
    }


    public void setOwner(Material mat)
    {
        //gameObject.GetComponent<SpriteRenderer>().sharedMaterial = mat;
        gameObject.GetComponent<SpriteRenderer>().sharedMaterial = sharedMaterial = mat;
        PreOwnerH = -1;
    }

    public static string toStr(List<int> blocksId)
    {
        if (blocksId.Count == 0)
            return "";
        string s="";
        foreach (var blockId in blocksId)
        {
            s += blockId.ToString() + " ";
        }
        return s.Substring(0, s.Length - 1);
    }

    public static string toStr(List<AreaBlock> blocks)
    {
        if (blocks.Count == 0)
            return "";
        string s = "";
        foreach (var block in blocks)
        {
            s += block.Id.ToString() + " ";
        }
        return s.Substring(0, s.Length - 1);
    }

    public static List<int> toListInt(string blocksIdStr)
    {
        List<int> blocksId = new List<int>();

        if (blocksIdStr == "")
            return blocksId;

        foreach (var blockIdStr in blocksIdStr.Split(' '))
        {
            blocksId.Add(System.Convert.ToInt32(blockIdStr));
        }
        return blocksId;
    }

    public static List<int> toListInt(List<AreaBlock> blocks)
    {
        List<int> blocksId = new List<int>();
        foreach (var block in blocks)
        {
            blocksId.Add(block.Id);
        }
        return blocksId;
    }

    public static List<AreaBlock> toListBlock(string blocksIdStr)
    {
        List<AreaBlock> blocksId = new List<AreaBlock>();

        if (blocksIdStr == "")
            return blocksId;

        foreach (var blockIdStr in blocksIdStr.Split(' '))
        {
            int id = System.Convert.ToInt32(blockIdStr);
            blocksId.Add(Area.blocks[id / Area.rows][id % Area.rows]);
        }
        return blocksId;
    }
}
