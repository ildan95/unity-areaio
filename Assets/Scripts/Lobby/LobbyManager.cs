using System.Collections.Generic;
using UnityEngine;


public class LobbyManager : Photon.PunBehaviour{
 
    public void StartGame()
    {
        Debug.Log("Rooms count: " + PhotonNetwork.countOfRooms);
        if (!PhotonNetwork.inRoom)
        {
            if (LocalData._lastRoomName != "")
                PhotonNetwork.JoinRoom(LocalData._lastRoomName);
            else
                PhotonNetwork.JoinRandomRoom();
        }
        else
            Loading.Load(LoadingScene.Game);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        Debug.Log(codeAndMsg[0]);
        Debug.Log(codeAndMsg[1]);
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log(codeAndMsg[0]);
        Debug.Log(codeAndMsg[1]);
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 10 }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On joined: "+PhotonNetwork.room.Name);
        Loading.Load(LoadingScene.Game);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined to lobby (lobby manager)");
    }
}
