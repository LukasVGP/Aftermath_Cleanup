using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI player1ScoreText;
    [SerializeField] private TextMeshProUGUI player2ScoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI scoreText; // Added back for backward compatibility
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Settings")]
    [SerializeField] private LevelTimer levelTimer;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevels = 3;
    [SerializeField] private float baseZombiePoints = 100f;
    [SerializeField] private int requiredWaves = 5;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float menuTransitionDelay = 3f;
    [SerializeField] private Transform exitPoint;

    private float currentTime;
    private int player1Score = 0;
    private int player2Score = 0;
    private int totalScore = 0;
    private int zombiesDisposed = 0;
    private int requiredZombies = 0;
    private bool isGameActive = true;
    private int completedWaves = 0;
    private bool canExit = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        player1Score = 0;
        player2Score = 0;
        totalScore = 0;
        zombiesDisposed = 0;
        completedWaves = 0;
        canExit = false;
        currentTime = levelTimer != null ? levelTimer.levelTime : 300f;
        isGameActive = true;
        UpdateUI();
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            if (currentTime <= 0)
            {
                TriggerGameOver();
            }

            // Check if all waves are completed and all zombies disposed
            if (completedWaves >= requiredWaves && zombiesDisposed >= requiredZombies)
            {
                TriggerWin();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void SetRequiredZombies(int amount)
    {
        requiredZombies = amount;
        UpdateUI();
    }

    public void AddDisposedZombie(float pointMultiplier = 1.0f)
    {
        zombiesDisposed++;
        AddScore(Mathf.RoundToInt(baseZombiePoints * pointMultiplier)); // Use the original method
        CheckLevelCompletion();
    }

    // Original method for backward compatibility
    public void AddScore(int points)
    {
        // Add to total score only
        totalScore += points;
        UpdateUI();
    }

    // New method for player-specific scoring
    public void AddScore(int points, int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                player1Score += points;
                break;
            case 2:
                player2Score += points;
                break;
            default:
                // Add to total score only
                break;
        }

        // Always update total score
        totalScore += points;
        UpdateUI();
    }

    // Method for player 1 to collect coins
    public void Player1CollectCoin(int points)
    {
        player1Score += points;
        totalScore += points;
        UpdateUI();
    }

    // Method for player 2 to collect coins
    public void Player2CollectCoin(int points)
    {
        player2Score += points;
        totalScore += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (player1ScoreText != null)
        {
            player1ScoreText.text = $"P1 Score: {player1Score}";
        }

        if (player2ScoreText != null)
        {
            player2ScoreText.text = $"P2 Score: {player2Score}";
        }

        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {totalScore}";
        }

        // For backward compatibility
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }

        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel}";
        }
    }

    public void WaveCompleted()
    {
        completedWaves++;
        Debug.Log($"Wave completed: {completedWaves}/{requiredWaves}");

        if (completedWaves >= requiredWaves)
        {
            canExit = true;
            Debug.Log("All waves completed! Exit is now available.");
            CheckLevelCompletion();
        }
    }

    private void CheckLevelCompletion()
    {
        Debug.Log($"Checking level completion: Zombies disposed {zombiesDisposed}/{requiredZombies}, Waves completed {completedWaves}/{requiredWaves}");

        if (zombiesDisposed >= requiredZombies && completedWaves >= requiredWaves)
        {
            canExit = true;
            Debug.Log("Level completion conditions met!");
        }
    }

    public void TriggerGameOver()
    {
        if (!isGameActive) return; // Prevent multiple triggers

        isGameActive = false;
        Time.timeScale = 0f;
        Debug.Log("Game Over triggered!");

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {totalScore}\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
            }
        }
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    public void TriggerWin()
    {
        if (!isGameActive) return; // Prevent multiple triggers

        isGameActive = false;
        Time.timeScale = 0f;
        Debug.Log("Win triggered!");

        if (winScreen != null)
        {
            winScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {totalScore}\nPlayer 1: {player1Score}\nPlayer 2: {player2Score}";
            }
        }
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSecondsRealtime(menuTransitionDelay);
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        zombiesDisposed = 0;
        completedWaves = 0;
        canExit = false;
        currentTime = levelTimer != null ? levelTimer.levelTime : 300f;
        SceneManager.LoadScene("Level_" + currentLevel);
    }

    public void RestartGame()
    {
        currentLevel = 1;
        InitializeGame();
        SceneManager.LoadScene("Level_1");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Getter methods
    public bool IsGameActive() => isGameActive;
    public int GetPlayer1Score() => player1Score;
    public int GetPlayer2Score() => player2Score;
    public int GetTotalScore() => totalScore;
    public int GetCurrentScore() => totalScore; // For backward compatibility
    public int GetCurrentLevel() => currentLevel;
    public int GetZombiesDisposed() => zombiesDisposed;
    public int GetRequiredZombies() => requiredZombies;
    public float GetCurrentTime() => currentTime;
    public int GetRequiredWaves() => requiredWaves;
    public int GetCompletedWaves() => completedWaves;
    public int GetMaxLevels() => maxLevels;
    public bool CanExitLevel() => canExit;
    public Transform GetExitPoint() => exitPoint;

    // Set exit point
    public void SetExitPoint(Transform exit)
    {
        exitPoint = exit;
        Debug.Log("Exit point set");
    }
}
