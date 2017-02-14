using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatistic : MonoBehaviour
{

    public GameStatisticLine percent;
    public GameStatisticLine kills;
    public Text totalGold;

    PlayerModel model;
    bool playerCreated = false;

    void OnEnable()
    {
        if (!playerCreated)
            GameManager.PlayerCreatedEvent += OnPlayerCreated;
    }

    void OnPlayerCreated(PlayerController controller)
    {
        GameManager.PlayerCreatedEvent -= OnPlayerCreated;
        model = controller.model;
        controller.DeathEvent += ShowStatistic;
        playerCreated = true;
        gameObject.SetActive(false);
    }

    public void ShowStatistic()
    {
        percent.ValueText.text = model.Percent.ToString();
        percent.GoldText.text = LocalData.GoldPerAreaPercent.ToString()+"J";
        percent.TotalGoldText.text = ((int)System.Math.Round(model.Percent * LocalData.GoldPerAreaPercent, 0)).ToString() + "J";
        kills.ValueText.text = model.Kills.ToString();
        kills.GoldText.text = LocalData.GoldPerKill.ToString();
        kills.TotalGoldText.text = (model.Kills * LocalData.GoldPerKill).ToString();
        totalGold.text = "K "+model.TotalMoneyWin().ToString();
        gameObject.SetActive(true);
        model.controller.DeathEvent -= ShowStatistic;
    }
}
