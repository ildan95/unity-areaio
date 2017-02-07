using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaBlock : MonoBehaviour {

    public float OwnerH { get; private set; }
    public float PreOwnerH { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Id { get; private set; }

    public bool isFree
    {
        get
        {
            return OwnerH == -1 && PreOwnerH == -1;
        }
        private set { }
    }

    public bool isOwn(float playerH)
    {
        return OwnerH == playerH;
    }

    void Awake () {
        OwnerH = PreOwnerH = -1;
        X = (int)System.Math.Round(transform.position.x, 0);
        Y = (int)System.Math.Round(transform.position.y, 0);
        Id = X * Area.rows + Y;
    }
	
    public void setOwner(float playerH)
    {
        OwnerH = playerH;
        gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(playerH, 1, 1);
        PreOwnerH = -1;
    }

    public void setPreOwner(float playerH)
    {
        PreOwnerH = playerH;
        gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(playerH,0.5f,1);
    }

    private void setFree()
    {
        PreOwnerH = OwnerH = -1;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void setFree(float playerH)
    {
        Debug.Log("set free " + playerH.ToString());
        if (PreOwnerH == playerH)
        {
            if (OwnerH == -1)
                setFree();
            else
                setOwner(OwnerH);

        }
        else if (OwnerH == playerH)
        {
            if (PreOwnerH == -1)
                setFree();
            else
                setPreOwner(OwnerH);
        }
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

}
