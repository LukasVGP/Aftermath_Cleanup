using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int currentScore = 0;
    [SerializeField] private int targetScore = 1000;

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

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
        CheckWinCondition();
    }

    public void DeductPoints(int points)
    {
        currentScore -= points;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update score display
    }

    private void CheckWinCondition()
    {
        if (currentScore >= targetScore)
        {
            // Trigger win state
        }
    }
}
