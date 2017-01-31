using UnityEngine;
using System.Collections;

public class AreaBlock : MonoBehaviour {

    public GameObject Owner { get; private set; }
    public GameObject PreOwner { get; private set; }
    public int X { get; private set; }
    public int Z { get; private set; }

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
        Z = (int)System.Math.Round(transform.position.z, 0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setOwner(GameObject obj)
    {
        Owner = obj;
        gameObject.GetComponent<MeshRenderer>().material.color = Owner.GetComponent<MeshRenderer>().material.color;
        PreOwner = null;
    }

    public void setPreOwner(GameObject obj)
    {
        PreOwner = obj;
        var mesh = obj.GetComponent<MeshRenderer>();
        Color c = mesh.material.color;
        c.g = 0.5f;
        gameObject.GetComponent<MeshRenderer>().material.color = c;
        //obj.GetComponent<PlayerController>().playerModel.reservateBlock(this);
    }

    

}
