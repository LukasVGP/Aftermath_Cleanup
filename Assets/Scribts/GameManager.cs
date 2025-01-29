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
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreUI?.UpdateScore(currentScore);
    }

    public void TriggerGameEnd()
    {
        isGameActive = false;
        Debug.Log("Game Complete! Final Score: " + currentScore);
        // Add additional game end logic here
    }

    public void NotifyZombieTornApart()
    {
        FindObjectOfType<ZombieSpawner>()?.OnZombieTornApart();
    }

    public void NotifyZombiePartDestroyed()
    {
        totalZombiesProcessed++;
        FindObjectOfType<ZombieSpawner>()?.OnZombiePartDestroyed();
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetTotalZombiesProcessed()
    {
        return totalZombiesProcessed;
    }
}
