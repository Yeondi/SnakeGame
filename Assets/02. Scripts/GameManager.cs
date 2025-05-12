using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Preparing,
        Playing,
        Paused,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    // 상태 이벤트
    public event Action<GameState, GameState> OnGameStateChanged;

    public PlayerManager PlayerManager { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public WaveManager WaveManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public PoolManager PoolManager { get; private set; }

    [Header("Manager Prefabs")] [SerializeField]
    private GameObject playerManagerPrefab;

    [SerializeField] private GameObject weaponManagerPrefab;
    [SerializeField] private GameObject waveManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private GameObject saveManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject poolManagerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitManagers();
    }

    private void InitManagers()
    {
        PlayerManager = Instantiate(playerManagerPrefab).GetComponent<PlayerManager>();
        WeaponManager = Instantiate(weaponManagerPrefab).GetComponent<WeaponManager>();
        WaveManager = Instantiate(waveManagerPrefab).GetComponent<WaveManager>();
        UIManager = Instantiate(uiManagerPrefab).GetComponent<UIManager>();
        SaveManager = Instantiate(saveManagerPrefab).GetComponent<SaveManager>();
        AudioManager = Instantiate(audioManagerPrefab).GetComponent<AudioManager>();
        PoolManager = Instantiate(poolManagerPrefab).GetComponent<PoolManager>();
    }

    private void Start()
    {
        ChangeState(GameState.MainMenu);
        StartGame();
    }

    public void StartGame()
    {
        ChangeState(GameState.Preparing);
        // UIManager.ShowCountdown(() =>
        // {
        //     PlayerManager.SpawnPlayer();
        //     WaveManager.StartFirstWave();
        //     ChangeState(GameState.Playing);
        // });
        PlayerManager.SpawnPlayer();
        WaveManager.StartFirstWave();
        ChangeState(GameState.Playing);
    }

    public void EndGame(bool victory)
    {
        ChangeState(GameState.GameOver);
        UIManager.ShowGameOver(victory);
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
            UIManager.ShowPauseMenu();
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
            UIManager.HidePauseMenu();
        }
    }

    private void ChangeState(GameState newState)
    {
        if (CurrentState == newState) return;
        var oldState = CurrentState;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(oldState, newState);
    }
}