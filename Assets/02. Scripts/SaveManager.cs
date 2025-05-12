using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string Key_HighScore = "HighScore";
    private const string Key_Money = "Money";

    private int currentMoney;
    public int CurrentMoney => currentMoney;

    public void Init()
    {
        currentMoney = PlayerPrefs.GetInt(Key_Money, 0);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        PlayerPrefs.SetInt(Key_Money, currentMoney);
    }

    public bool TrySpendMoney(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            PlayerPrefs.SetInt(Key_Money, currentMoney);
            return true;
        }
        return false;
    }

    public void SaveHighScore(int score)
    {
        int oldScore = PlayerPrefs.GetInt(Key_HighScore, 0);
        if (score > oldScore)
        {
            PlayerPrefs.SetInt(Key_HighScore, score);
        }
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(Key_HighScore, 0);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey(Key_HighScore);
        PlayerPrefs.DeleteKey(Key_Money);
        PlayerPrefs.Save();
    }
}