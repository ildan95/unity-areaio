using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public GameObject player;
    public static List<float> hueColorsInGame = new List<float>();

    void Start()
    {
        var position = randomFreeSquare(2);
        player = Instantiate(Resources.Load<GameObject>("Player2D"), position, Quaternion.identity);
    }

    void Update() 
    {

    }

    public Vector3 randomFreeSquare(int side)
    {
        int X = Random.Range(0, Area.rows);
        int Y = Random.Range(0, Area.cols);
        while (!Area.isFreeSquare(X, Y, side))
        {
            X = Random.Range(0, Area.rows);
            Y = Random.Range(0, Area.cols);
        }
        return new Vector3(X, Y, 0);
    }

    public static float randomHueColor()
    {
        float hue=0;
        int colorsCount = hueColorsInGame.Count;
        int count = 0;
        while (count < colorsCount) {
            count = 0;
            hue = Random.Range(0, 1);
            foreach (var hueColor in hueColorsInGame)
            {
                if (Mathf.Abs(hueColor - hue) > 0.05)
                    count++;
                else
                    break;
            }
        }
        return hue;
    }
}
