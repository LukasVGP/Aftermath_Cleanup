using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    public float levelTime = 300f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float currentTime;
    private bool isTimerRunning = true;

    void Start()
    {
        currentTime = levelTime;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                GameManager.Instance.TriggerGameOver();
                isTimerRunning = false;
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
}
