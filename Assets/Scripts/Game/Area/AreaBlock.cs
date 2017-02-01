using UnityEngine;
using System.Collections;

public class AreaBlock : MonoBehaviour {

    public GameObject Owner { get; private set; }
    public GameObject PreOwner { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool isFree
    {
        get
        {
            return Owner == null && PreOwner == null;
        }
        private set { }
    }

    public bool isOwn(GameObject player)
    {
        return Owner == player;
    }

    void Start () {
        X = (int)System.Math.Round(transform.position.x, 0);
        Y = (int)System.Math.Round(transform.position.y, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setOwner(GameObject obj)
    {
        Owner = obj;
        float hue = obj.GetComponent<PlayerController>().playerModel.hueColor;
        gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue, 1, 1);
        PreOwner = null;
    }

    public void setPreOwner(GameObject obj)
    {
        PreOwner = obj;
        float hue = obj.GetComponent<PlayerController>().playerModel.hueColor;
        gameObject.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue,0.5f,1);
    }

    

}
