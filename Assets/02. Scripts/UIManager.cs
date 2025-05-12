using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    
    [Header("Countdown")] 
    [SerializeField] private Text countdownText;

    [Header("Game Over Info")]
    [SerializeField] private Text resultText;

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowCountdown(Action onComplete)
    {
        StartCoroutine(CountdownRoutine(onComplete));
    }

    private System.Collections.IEnumerator CountdownRoutine(Action onComplete)
    {
        hudPanel.SetActive(true);
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public void ShowPauseMenu()
    {
        pausePanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        pausePanel.SetActive(false);
    }

    public void ShowGameOver(bool victory)
    {
        gameOverPanel.SetActive(true);
        resultText.text = victory ? "Victory!" : "Defeat";
    }

}