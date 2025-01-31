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
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Game Settings")]
    [SerializeField] private LevelTimer levelTimer;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevels = 3;
    [SerializeField] private float baseZombiePoints = 100f;
    [SerializeField] private int requiredWaves = 3;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float menuTransitionDelay = 3f;

    private float currentTime;
    private int currentScore = 0;
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
        currentScore = 0;
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
        AddScore(Mathf.RoundToInt(baseZombiePoints * pointMultiplier));
        CheckLevelCompletion();
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel}";
        }
    }

    public void WaveCompleted()
    {
        completedWaves++;
        if (completedWaves >= requiredWaves)
        {
            canExit = true;
            CheckLevelCompletion();
        }
    }

    private void CheckLevelCompletion()
    {
        if (zombiesDisposed >= requiredZombies && completedWaves >= requiredWaves)
        {
            canExit = true;
        }
    }

    public void TriggerGameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {currentScore}";
            }
        }
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    public void TriggerWin()
    {
        isGameActive = false;
        Time.timeScale = 0f;
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {currentScore}";
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
    public int GetCurrentScore() => currentScore;
    public int GetCurrentLevel() => currentLevel;
    public int GetZombiesDisposed() => zombiesDisposed;
    public int GetRequiredZombies() => requiredZombies;
    public float GetCurrentTime() => currentTime;
    public int GetRequiredWaves() => requiredWaves;
    public int GetCompletedWaves() => completedWaves;
    public int GetMaxLevels() => maxLevels;
    public bool CanExitLevel() => canExit;
}
