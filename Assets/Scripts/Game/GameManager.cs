using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class GameManager
{
    public delegate void PlayerCreatedHandler(PlayerController controller);
    public static event PlayerCreatedHandler PlayerCreatedEvent;

    public delegate void NextMoveHandler();
    public static event NextMoveHandler NextMoveEvent;
}

public partial class GameManager : MonoBehaviour
{
    public PlayerController controller;
    public static List<float> hueColorsInGame = new List<float>();
    public static float moveCooldown = 0.2f;


    void Awake()
    {

    }

    void Start()
    {
        createPlayer();
    }
     
    public void createPlayer()
    {
        var player = PhotonNetwork.Instantiate("Player2D", new Vector2(-100,-100), Quaternion.identity,0);
        controller = player.GetComponent<PlayerController>();
        if (PlayerCreatedEvent != null)
            PlayerCreatedEvent(controller);
        controller.ReadyEvent += StartGame;
        controller.DeathEvent += EndGame;
    }

    public void toLobby()
    {
        LocalData._lastRoomName = PhotonNetwork.room.Name;
        PhotonNetwork.LeaveRoom();
        Loading.Load(LoadingScene.Lobby);
    }

    void StartGame()
    {
        StartCoroutine(nextMove());
        controller.ReadyEvent -= StartGame;
    }

    void EndGame()
    {
        StopAllCoroutines();
        controller.DeathEvent -= EndGame;
    }

    IEnumerator nextMove()
    {
        if (NextMoveEvent != null)
            NextMoveEvent();
        yield return new WaitForSeconds(moveCooldown);
        StartCoroutine(nextMove());
    }
}
