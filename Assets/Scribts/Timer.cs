using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float levelTime = 300f; // 5 minutes default
    private float currentTime;
    private bool isTimerRunning = true;

    private void Start()
    {
        currentTime = levelTime;
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                TimeUp();
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void TimeUp()
    {
        isTimerRunning = false;
        GameManager.Instance.GameOver("Time's Up!");
    }

    public void PauseTimer() => isTimerRunning = false;
    public void ResumeTimer() => isTimerRunning = true;
}
