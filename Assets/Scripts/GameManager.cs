using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public GameObject player;

    void Start()
    {
        var position = randomFreeSquare(2);
        player = Instantiate(Resources.Load<GameObject>("Player"), position, Quaternion.identity);
    }

    void Update() 
    {

    }

    public Vector3 randomFreeSquare(int side)
    {
        int X = Random.Range(0, Area.rows);
        int Z = Random.Range(0, Area.cols);
        while (!Area.isFreeSquare(X, Z, side))
        {
            X = Random.Range(0, Area.rows);
            Z = Random.Range(0, Area.cols);
        }
        return new Vector3(X, 2, Z);
    }
}
