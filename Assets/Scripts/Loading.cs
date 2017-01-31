using UnityEngine;
using System.Collections;

public enum LoadingScene
{
    Lobby,
    Game
}

public class Loading : MonoBehaviour
{

    private static LoadingScene _nextScene { get; set; }

    void Update()
    {

    }

    void Start()
    {
        //if (_nextScene == LoadingScene.Game)

    }

    void OnJoinedLobby()
    {
        //PhotonNetwork.LoadLevel("3 Lobby");
    }

    public static void Load(LoadingScene nextScene)
    {
        _nextScene = nextScene;
        //PhotonNetwork.LoadLevel("2 Loading");
    }
}
