using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;

public enum LoadingScene
{
    Start,
    Lobby,
    Game
}

public class Loading : MonoBehaviour
{

    private static LoadingScene _nextScene { get; set; }
    public Text statusText;
    public Slider progress;
    public string version = "0.0.1";

    private int _connectingToPhoton = -1, _loging = -1;
    private bool _startLoadingData = false;
    private float progressValue = 0;

    void Update()
    {
        progress.value = progressValue;
        if (_nextScene == LoadingScene.Start)
        {
            if (_connectingToPhoton == -1)
            {
                statusText.text = "Подключаемся к серверу ...";
                _connectingToPhoton = ConnectToPhoton() ? 1 : 0;
                PhotonNetwork.logLevel = PhotonLogLevel.Full;
            }
            else if (_connectingToPhoton == 0)
            {
                statusText.color = Color.red;
                progress.value = 0;
                statusText.text = "Не можем подключиться к серверу. Проверьте интернет соединение ...";
                return;
            }

            if (PhotonNetwork.connectionState == ConnectionState.Connecting)
                if (progressValue < 0.1f)
                    progressValue += 0.1f;

            if (PhotonNetwork.connectionState == ConnectionState.InitializingApplication)
                if (progressValue < 0.3f)
                    progressValue += 0.3f;

            if (_loging == -1)
            {
                statusText.text = "Заходим в аккаунт ...";
                LogIn();
                _loging = -2;
            }
            else if (_loging == 0)
            {
                statusText.color = Color.red;
                progress.value = 0;
                statusText.text = "Не можем зайти в аккаунт. Проверьте интернет соединение ...";
                return;
            }
            else if (_loging == 1)
            {
                if (progressValue < 0.7f)
                    progressValue += 0.3f;
                if (!_startLoadingData)
                {
                    statusText.text = "Загружаем данные ...";
                    progress.value = 0.7f;
                    LocalData.LoadAll();
                    _startLoadingData = true;
                }
                else
                    if (LocalData.IsSync)
                {
                    if (progressValue < 1)
                        progressValue += 0.3f;
                    
                }
            }
            if (progressValue == 1)
                PhotonNetwork.LoadLevel("Lobby");
        }
    }

    void Start()
    {
        if (_nextScene == LoadingScene.Game)
        {
            GameObject.Find("Status").SetActive(false);
            PhotonNetwork.LoadLevel("Game");
        }
        else if (_nextScene == LoadingScene.Lobby)
        {
            PhotonNetwork.LoadLevel("Lobby");
        }
        Debug.Log("Loading start end");
    }

    public static void Load(LoadingScene nextScene)
    {
        _nextScene = nextScene;
        PhotonNetwork.LoadLevel("Loading");
        //SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
    }

    private void LogIn()
    {
        LoginWithAndroidDeviceIDRequest req = new LoginWithAndroidDeviceIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true
            }
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(req, (LoginResult res) =>
        {
            LocalData.PlayFabId = res.PlayFabId;
            LocalData.SessionTicket = res.SessionTicket;
            string displayName = res.InfoResultPayload.AccountInfo.TitleInfo.DisplayName;
            LocalData.DisplayName = displayName == "" ? res.PlayFabId.Substring(0,5) : displayName;
            _loging = 1;
        },
        (PlayFabError err) =>
        {
            statusText.text = err.ErrorMessage;
            //statusText.color = Color.red;
            string http = string.Format("HTTP:{0}", err.HttpCode);
            string message = string.Format("ERROR:{0} -- {1}", err.Error, err.ErrorMessage);
            string details = string.Empty;
            Debug.LogError(string.Format("{0}\n {1}\n {2}\n", http, message, details));
            _loging = 0;
        });
    }

    private bool ConnectToPhoton()
    {
       return PhotonNetwork.ConnectUsingSettings(version);
    }


    void OnJoinedLobby()
    {
        Debug.Log("Joined to lobby");
    }
    public void OnConnectedToMaster()
    {
        Debug.Log("on connected to master");
    }

    void OnConnectedToPhoton()
    {
        Debug.Log("Connected to photon");
    }
}
