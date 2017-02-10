using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {

    public PlayerController controller;
    public PlayerModel model;

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    private Vector3 pos;
    private Camera playerCamera;
    private int playerCameraLvl = 0;
    public LineRenderer lineRenderer;
    public List<Vector3> linePoints = new List<Vector3>();

    void Start()
    {
        model = controller.model;
        pos = new Vector3(model.X, model.Y, 0);
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
    }

    void Update()
    {
        if (model.isAlive)
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, dampTime);
    }

    public void UpdatePos()
    {
        pos = new Vector3(pos.x, pos.y);
        if (model.isAlive)
        {
            addPoint(pos);
        }
        pos = new Vector3(model.X, model.Y, -0.5f);
        //startTime = Time.time;
    }


    public void clearLineRenderer()
    {
        linePoints.Clear();
        lineRenderer.numPositions = 0;
    }

    public void addPoint(Vector3 point)
    {
        if (linePoints.Contains(pos))
            return;
        linePoints.Add(point);
        lineRenderer.numPositions = linePoints.Count;
        lineRenderer.SetPosition(linePoints.Count - 1, linePoints[linePoints.Count - 1]);
    }

    public void updateCameraSize()
    {
        int[] lvls = new int[5] { 5, 7, 9, 12, 15 };

        if (model.blocksId.Count > 500 && playerCameraLvl <= 4)
            StartCoroutine(CameraResize(playerCamera.orthographicSize, lvls[playerCameraLvl++]));
        else if (model.blocksId.Count > 100 && playerCameraLvl <= 3)
            StartCoroutine(CameraResize(playerCamera.orthographicSize, lvls[playerCameraLvl++]));
        else if (model.blocksId.Count > 50 && playerCameraLvl <= 2)
            StartCoroutine(CameraResize(playerCamera.orthographicSize, lvls[playerCameraLvl++]));
        else if (model.blocksId.Count > 10 && playerCameraLvl <= 1)
            StartCoroutine(CameraResize(playerCamera.orthographicSize, lvls[playerCameraLvl++]));
    }

    IEnumerator CameraResize(float startSize, float endSize, int smoothness=20)
    {

        for (int i=0; i<smoothness; i++)
        {
            playerCamera.orthographicSize += (endSize - startSize) / smoothness;
            yield return null;
        }
    }
}
