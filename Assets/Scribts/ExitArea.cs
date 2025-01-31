using UnityEngine;

public class ExitArea : MonoBehaviour
{
    private int playersInside = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside++;
            CheckCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside--;
        }
    }

    private void CheckCompletion()
    {
        if (playersInside >= 2 && GameManager.Instance.CanExitLevel())
        {
            if (GameManager.Instance.GetCurrentLevel() >= GameManager.Instance.GetMaxLevels())
            {
                GameManager.Instance.TriggerWin();
            }
            else
            {
                GameManager.Instance.LoadNextLevel();
            }
        }
    }
}
