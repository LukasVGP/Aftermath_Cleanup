using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI zombieCountText;

    [Header("Game Settings")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private float gameEndDelay = 2f;
    [SerializeField] private int requiredZombiesForLevel = 10;

    private int currentScore = 0;
    private int processedZombieCount = 0;
    private int totalZombiesProcessed = 0;
    private Timer levelTimer;

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
        InitializeGame();
        levelTimer = FindFirstObjectByType<Timer>();
    }

    private void InitializeGame()
    {
        isGameActive = true;
        currentScore = 0;
        processedZombieCount = 0;
        Time.timeScale = 1;
        UpdateUI();
        HideScreens();
    }

    public void AddScore(int points)
    {
        if (!isGameActive) return;

        currentScore += points;
        UpdateUI();
    }

    public void ProcessZombie(bool isWhole)
    {
        if (!isGameActive) return;

        processedZombieCount++;
        totalZombiesProcessed++;
        AddScore(isWhole ? 100 : 50);

        if (processedZombieCount >= requiredZombiesForLevel)
        {
            CheckLevelCompletion();
        }

        UpdateZombieCountDisplay();
    }

    private void UpdateZombieCountDisplay()
    {
        if (zombieCountText != null)
        {
            zombieCountText.text = $"Zombies Processed: {processedZombieCount}/{requiredZombiesForLevel}";
        }
    }

    private void UpdateUI()
    {
        if (scoreUI != null)
        {
            scoreUI.UpdateScore(currentScore);
        }
    }

    private void HideScreens()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
    }

    public void GameOver(string reason)
    {
        isGameActive = false;
        Time.timeScale = 0;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {currentScore}\n{reason}";
            }
        }

        if (levelTimer != null)
        {
            levelTimer.PauseTimer();
        }
    }

    public void ShowWinScreen()
    {
        isGameActive = false;
        Time.timeScale = 0;

        if (winScreen != null)
        {
            winScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Victory!\nFinal Score: {currentScore}\nTotal Zombies Processed: {totalZombiesProcessed}";
            }
        }

        if (levelTimer != null)
        {
            levelTimer.PauseTimer();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        InitializeGame();
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            InitializeGame();
        }
        else
        {
            ShowWinScreen();
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
        InitializeGame();
    }

    private void CheckLevelCompletion()
    {
        if (processedZombieCount >= requiredZombiesForLevel)
        {
            LoadNextLevel();
        }
    }

    public bool IsGameActive() => isGameActive;
    public int GetCurrentScore() => currentScore;
    public int GetProcessedZombieCount() => processedZombieCount;
    public int GetTotalZombiesProcessed() => totalZombiesProcessed;
    public int GetRequiredZombiesForLevel() => requiredZombiesForLevel;
}
