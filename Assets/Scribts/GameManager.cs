using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private ScoreUI scoreUI;

    [Header("Game Settings")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private float gameEndDelay = 2f;

    private int currentScore = 0;
    private int totalZombiesProcessed = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isGameActive = true;
        currentScore = 0;
        totalZombiesProcessed = 0;
        UpdateUI();
        Debug.Log("Game Started!");
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
        Debug.Log($"Score added: {points}, Total score: {currentScore}");
    }

    private void UpdateUI()
    {
        if (scoreUI != null)
        {
            scoreUI.UpdateScore(currentScore);
        }
    }

    public void TriggerGameEnd()
    {
        isGameActive = false;
        Debug.Log($"Game Complete! Final Score: {currentScore}");
    }

    public bool IsGameActive() => isGameActive;
    public int GetCurrentScore() => currentScore;
    public int GetTotalZombiesProcessed() => totalZombiesProcessed;
}
