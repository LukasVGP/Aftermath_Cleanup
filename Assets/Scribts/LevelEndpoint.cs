using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndpoint : MonoBehaviour
{
    [SerializeField] private int nextLevelIndex;
    [SerializeField] private bool isFinalLevel;
    [SerializeField] private int requiredZombieCount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CheckLevelCompletion();
        }
    }

    private void CheckLevelCompletion()
    {
        if (GameManager.Instance.GetProcessedZombieCount() >= requiredZombieCount)
        {
            if (isFinalLevel)
            {
                GameManager.Instance.ShowWinScreen();
            }
            else
            {
                SceneManager.LoadScene(nextLevelIndex);
            }
        }
    }
}
