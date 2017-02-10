using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject player;

    private PlayerController _playerController; 
    private PlayerModel _playerModel;
    public GameStatistic gameStatistic;
    public static List<float> hueColorsInGame = new List<float>();

    private bool _updatedPlayerInfo = false;

    void Awake()
    {
        Debug.Log("Game manager awake");
        
       
        Debug.Log("Game manager awake end");
    }

    void Start()
    {
        Debug.Log("Game manager start start");
        createPlayer();

        Debug.Log("Game manager start end");

    }

    void Update() 
    {
        if (!_playerModel.isAlive && !_updatedPlayerInfo)
        {
            _playerController.photonView.RPC("removeHueColor", PhotonTargets.OthersBuffered, _playerModel.hueColor);
            gameStatistic.ShowStatistic(_playerModel);
            _playerModel.UpdatePlayerInfo();
            _updatedPlayerInfo = true;
        }
    }
       
    public void createPlayer()
    {
        
        player = PhotonNetwork.Instantiate("Player2D", new Vector3(-100,-100,-100), Quaternion.identity,0);
        _playerController = player.GetComponent<PlayerController>();
        _playerModel = _playerController.model;
        var camera = GameObject.Find("Camera").GetComponent<CameraScript>();
        camera.setTarget(player.transform);
        camera.dampTime = _playerController.moveCooldown;
        Debug.Log("Player: " + _playerController.photonView.viewID);
    }

    public void toLobby()
    {
        LocalData._lastRoomName = PhotonNetwork.room.Name;
        PhotonNetwork.LeaveRoom();
        Loading.Load(LoadingScene.Lobby);
    }

    
}
