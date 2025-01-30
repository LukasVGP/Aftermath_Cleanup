using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevels = 3;
    [SerializeField] private float baseZombiePoints = 100f;

    private int currentScore = 0;
    private int zombiesDisposed = 0;
    private int requiredZombies = 0;
    private bool isGameActive = true;

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
        isGameActive = true;
        UpdateUI();
        Time.timeScale = 1f;
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

    public void CheckLevelCompletion()
    {
        if (zombiesDisposed >= requiredZombies)
        {
            if (currentLevel >= maxLevels)
            {
                TriggerWin();
            }
            else
            {
                LoadNextLevel();
            }
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
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        zombiesDisposed = 0;
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
        SceneManager.LoadScene("MainMenu");
    }

    public bool IsGameActive() => isGameActive;
    public int GetCurrentScore() => currentScore;
    public int GetCurrentLevel() => currentLevel;
    public int GetZombiesDisposed() => zombiesDisposed;
    public int GetRequiredZombies() => requiredZombies;
}
