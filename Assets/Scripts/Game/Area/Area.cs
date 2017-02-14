using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Area : Photon.PunBehaviour {

    public static int rows = 100;
    public static int cols = 100;
    public static int blocksCount = rows * cols;
    public static List<List<AreaBlock>> blocks;
    

    public AreaBlock blockPrefab;
	void Awake () {
        transform.localScale = new Vector2(rows, cols);
        var mat = new Material(Shader.Find("Sprites/Default")); 
        blocks = new List<List<AreaBlock>>();
        GameObject bl = new GameObject("blocks");
        bl.transform.SetParent(gameObject.transform);
        for (int i = 0; i < rows; i++)
        {
            GameObject row = new GameObject("rows" + i.ToString());
            row.transform.SetParent(bl.transform);
            blocks.Add(new List<AreaBlock>());
            for (int j = 0; j < cols; j++)
            {
                AreaBlock block = Instantiate(Resources.Load<AreaBlock>("Block2D"), new Vector3(i, j, 1), Quaternion.identity).GetComponent<AreaBlock>();
                block.name = "block" + j.ToString();
                block.transform.SetParent(row.transform);
                block.GetComponent<SpriteRenderer>().sharedMaterial = mat;
                blocks[i].Add(block);
            }
        }
    }

    public static bool isFreeSquare(int x, int y, int size)
    {
        if (x + size >= rows || y + size >= cols)
            return false;
        for (int i = x; i < x+size; i++)
            for (int j = y; j < y+size; j++)
                if (!blocks[i][j].isFree)
                    return false;
        return true;
    }

    public static bool isOut(int x, int y)
    {
        return x > rows-1 || x < 0 || y > cols-1 || y < 0;
    }

    public static bool isOut(Vector2 pos)
    {
        return pos.x > rows - 1 || pos.x < 0 || pos.y > cols - 1 || pos.y < 0;
    }

    public static Vector2 randomFreeSquare(int side)
    {
        int X = Random.Range(0, rows);
        int Y = Random.Range(0, cols);
        int count = 0;
        while (!isFreeSquare(X, Y, side + 2))
        {
            if (count >= rows * cols)
            {
                PhotonNetwork.LeaveRoom();
                LocalData._lastRoomName = "";
                PhotonNetwork.LoadLevel("Lobby");
                return new Vector2(0,0);
            }
            count++;
            X = Random.Range(0, rows);
            Y = Random.Range(0, cols);
        }
        return new Vector2(X, Y);
    }
}
