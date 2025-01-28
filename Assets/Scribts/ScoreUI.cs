using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScore(0);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
