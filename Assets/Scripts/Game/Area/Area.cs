using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Area : MonoBehaviour {

    public static int rows = 100;
    public static int cols = 100;
    public static int blocksCount = rows * cols;
    public static List<List<AreaBlock>> blocks;

    public AreaBlock blockPrefab;
	// Use this for initialization
	void Awake () {
        blocks = new List<List<AreaBlock>>();
        GameObject bl = new GameObject("blocks");
        bl.transform.SetParent(gameObject.transform);
        for (int i = 0; i < rows; i++)
        {
            GameObject row = new GameObject("cols");
            row.transform.SetParent(bl.transform);
            blocks.Add(new List<AreaBlock>());
            for (int j = 0; j < cols; j++)
            {
                AreaBlock block = Instantiate(blockPrefab, new Vector3(i, j, 0), Quaternion.identity);
                block.transform.SetParent(row.transform);
                block.tag = "AreaBlock";
                blocks[i].Add(block);
            }
        }
       /* GameObject border = GameObject.CreatePrimitive(PrimitiveType.Cube);
        border.transform.position = new Vector3(rows / 2.0f, 1.5f, -1f);
        border.transform.localScale = new Vector3(rows + 2, 2, 1);
        border.transform.SetParent(gameObject.transform);
        border.tag = "Border";
        //border.GetComponent<MeshRenderer>().enabled = false;
        border = Instantiate(border);
        border.transform.position = new Vector3(rows / 2.0f, 1.5f, cols);
        border.transform.SetParent(gameObject.transform);
        border.tag = "Border";
        //border.GetComponent<MeshRenderer>().enabled = false;
        border = (GameObject)Instantiate(border, new Vector3(-1f, 1.5f, cols / 2.0f), Quaternion.identity);
        border.transform.localEulerAngles = new Vector3(0, 90, 0);
        border.transform.SetParent(gameObject.transform);
        border.tag = "Border";
        //border.GetComponent<MeshRenderer>().enabled = false;
        border = (GameObject)Instantiate(border, new Vector3(rows, 1.5f, cols / 2.0f), Quaternion.identity);
        border.transform.localEulerAngles = new Vector3(0, 90, 0);
        border.transform.SetParent(gameObject.transform);
        border.tag = "Border";
        //border.GetComponent<MeshRenderer>().enabled = false;
        */
    }

    // Update is called once per frame
    void Update () {
	
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
}
