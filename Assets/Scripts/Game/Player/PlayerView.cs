using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour {

    public PlayerController controller;
    public PlayerModel model;
    public float time = 0, lastTime = 0;
    public Vector2 pos, lastPos;

    private Camera playerCamera;
    private int playerCameraLvl = 0;
    public LineRenderer lineRenderer;
    public List<Vector2> linePoints = new List<Vector2>();
    public Vector3 velocity = Vector2.zero;
    bool changedDir = false;
    public float distance;
    public int forwardCoef = 3;
    void Awake()
    {
        Debug.Log("player view awake");
        model = controller.model;
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();

        controller.ReadyEvent += initPos;
        controller.MovedEvent += firstPosUpdate;
        controller.MovedEvent += UpdatePos;
        controller.BlocksOwnedEvent += clearLineRenderer;
        controller.BlocksOwnedEvent += updateCameraSize;
        controller.ChangedDirectionEvent += Controller_ChangedDirectionEvent;
    }

    private void Controller_ChangedDirectionEvent(Direction direction)
    {
        changedDir = true;
    }

    private void firstPosUpdate()
    {
        lastPos = new Vector2(model.X, model.Y);
        transform.position = lastPos;
        controller.MovedEvent -= firstPosUpdate;
    }

    void OnDisable()
    {
        controller.BlocksOwnedEvent -= clearLineRenderer;
        controller.BlocksOwnedEvent -= updateCameraSize;
        controller.MovedEvent -= UpdatePos;
    }

    void initPos()
    {
        Color c = Color.HSVToRGB(model.hueColor, 1, 1);
        c.a = 0.7f;
        lineRenderer.startColor = lineRenderer.endColor = c;
        pos = lastPos = new Vector2(model.X, model.Y);
        addPoint(pos);
        controller.ReadyEvent -= initPos;
    }

    void Update()
    {
        if (!model.isAlive)
            return;
        
        if (lineRenderer.numPositions > 0)
            lineRenderer.numPositions -= 1;
        distance = Vector2.Distance(transform.position, pos);
        var newPos = Vector3.SmoothDamp(transform.position, pos, ref velocity, Mathf.Clamp01(distance) * Mathf.Clamp01(GameManager.moveCooldown-(Time.time-time)));
        transform.position = newPos;
        if (distance <= 0.5f || distance > 1)
            addPoint(lastPos);
        else if (changedDir)
        {
            addPoint(lastPos);
            changedDir = false;
        }
        lineRenderer.numPositions += 1;
        lineRenderer.SetPosition(lineRenderer.numPositions - 1, newPos);
    }


    public void UpdatePos()
    {
        time = Time.time;
        lastPos = pos;
        pos = new Vector2(model.X, model.Y);
    }

    public void clearLineRenderer()
    {
        linePoints.Clear();
        lineRenderer.numPositions = 0;
    }

    public void addPoint(Vector2 point)
    {
        if (linePoints.Contains(point) || Area.isOut(point))
            return;
        linePoints.Add(point);
        lineRenderer.numPositions = linePoints.Count;
        lineRenderer.SetPosition(linePoints.Count - 1, linePoints[linePoints.Count - 1]);
    }

    public void updateCameraSize()
    {
        int[] lvls = new int[5] { 5, 7, 9, 10, 12 };

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
