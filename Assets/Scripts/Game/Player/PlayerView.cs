using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {

    public PlayerController controller;
    public PlayerModel model;

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    private Vector3 lastPos, pos;
    //private float startTime;

    void Start()
    {
        model = controller.model;
        lastPos = pos = new Vector3(model.X, model.Y, 0);
    }

    void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.position, pos, ref velocity, dampTime);
        //transform.localPosition = Vector3.Lerp(lastPos, pos, Time.time - startTime);
    }

    public void UpdatePos()
    {
        lastPos = pos;
        pos = new Vector3(model.X, model.Y, 0);
        //startTime = Time.time;
    }
}
