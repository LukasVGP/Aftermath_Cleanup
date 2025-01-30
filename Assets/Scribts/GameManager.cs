using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private ScoreUI scoreUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Text finalScoreText;

    [Header("Game Settings")]
    [SerializeField] private bool isGameActive = false;
    [SerializeField] private float gameEndDelay = 2f;
    [SerializeField] private int targetScore = 1000;

    private int currentScore = 0;
    private int totalZombiesProcessed = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetGameResolution();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void SetGameResolution()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
    }

    private void InitializeGame()
    {
        isGameActive = true;
        currentScore = 0;
        totalZombiesProcessed = 0;

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);

        UpdateUI();
        Debug.Log("Game Started!");
    }

    public void AddScore(int points)
    {
        if (!isGameActive) return;

        currentScore += points;
        UpdateUI();
        Debug.Log($"Score added: {points}, Total score: {currentScore}");

        if (currentScore >= targetScore)
        {
            TriggerVictory();
        }
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
        Debug.Log($"Game Over! Final Score: {currentScore}");

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {currentScore}";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void TriggerVictory()
    {
        isGameActive = false;
        Debug.Log($"Victory! Final Score: {currentScore}");

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Victory Score: {currentScore}";
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public bool IsGameActive() => isGameActive;

    public int GetCurrentScore() => currentScore;

    public int GetTotalZombiesProcessed() => totalZombiesProcessed;

    public void IncrementZombiesProcessed()
    {
        totalZombiesProcessed++;
        Debug.Log($"Zombies Processed: {totalZombiesProcessed}");
    }
}
