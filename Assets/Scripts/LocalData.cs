using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LocalData {

    public static bool IsSync { get; private set; }
    static int _syncCount = 0;
    private static int SyncCount
    {
        get { return _syncCount; }
        set {
            if (value == 0)
                IsSync = true;
            else
                IsSync = false;
            _syncCount = value;
        }
    }

    public static string _lastRoomName = "";

    public static string PlayFabId { get; set; }
    public static string SessionTicket { get; set; }
    public static string Username { get; set; }
    public static string DisplayName { get; set; }

    public static float BestArea { get; set; }
    public static int Gold { get; private set; }
    public static int MostKills { get; private set; }
    public static int GoldPerAreaPercent { get; private set; }
    public static int GoldPerKill { get; private set; }

	public static void LoadAll () {
        LoadMoney();
        LoadKillsLeaderboard();
        GoldPerKill = 10;
        GoldPerAreaPercent = 3;
	}

    public static void LoadMoney()
    {
        SyncCount++;
        AddUserVirtualCurrencyRequest reqCur = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "GD",
            Amount = 0,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(reqCur, (ModifyUserVirtualCurrencyResult res) =>
        {
            if (res != null)
                Gold = res.Balance;
            SyncCount--;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }
    public static void LoadKillsLeaderboard()
    {
        SyncCount++;
        GetLeaderboardAroundPlayerRequest req = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "Most dangerous",
        };
        PlayFabClientAPI.GetLeaderboardAroundPlayer(req, (GetLeaderboardAroundPlayerResult res) =>
        {
            if (res.Leaderboard != null)
                foreach (var person in res.Leaderboard)
                    if (person.PlayFabId == PlayFabId)
                    {
                        MostKills = person.StatValue;
                        break;
                    }
            SyncCount--;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }

    public static void SaveMoney(int aValue)
    {
        SyncCount++;
        AddUserVirtualCurrencyRequest reqCur = new AddUserVirtualCurrencyRequest()
        {
            VirtualCurrency = "GD",
            Amount = aValue,
        };
        PlayFabClientAPI.AddUserVirtualCurrency(reqCur, (ModifyUserVirtualCurrencyResult res) =>
        {
            if (res != null)
                Gold = res.Balance;
            SyncCount--;
        },
        (PlayFabError err) => Debug.Log(err.ErrorMessage));
    }
    public static void SaveKills(int kills)
    {
        UpdatePlayerStatisticsRequest req = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate> { new StatisticUpdate() { StatisticName = "Most dangerous", Value =kills  } }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(req, (UpdatePlayerStatisticsResult r) => { MostKills = kills; }, (PlayFabError err) => { Debug.Log(err.ErrorMessage); });
    }
}
