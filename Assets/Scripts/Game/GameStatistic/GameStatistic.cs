using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatistic : MonoBehaviour
{

    public GameStatisticLine percent;
    public GameStatisticLine kills;
    public Text totalGold;

    public void ShowStatistic(PlayerModel playerModel)
    {
        percent.ValueText.text = playerModel.Percent.ToString() + "%";
        percent.GoldText.text = LocalData.GoldPerAreaPercent.ToString();
        percent.TotalGoldText.text = ((int)System.Math.Round(playerModel.Percent * LocalData.GoldPerAreaPercent, 0)).ToString();
        kills.ValueText.text = playerModel.Kills.ToString();
        kills.GoldText.text = LocalData.GoldPerKill.ToString();
        kills.TotalGoldText.text = (playerModel.Kills * LocalData.GoldPerKill).ToString();
        totalGold.text = playerModel.TotalMoneyWin().ToString();
        gameObject.SetActive(true);
    }
}
