using UnityEngine;

public class Furnace : MonoBehaviour
{
    [Header("Point Settings")]
    [SerializeField] private int goldCoinValue = 100;
    [SerializeField] private int silverCoinValue = 50;

    private int zombieHalfCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<ZombieBody>(out ZombieBody body))
        {
            // Award points for a full zombie body (equivalent to 2 gold coins)
            AwardPoints(goldCoinValue * 2);
            Destroy(body.gameObject);
        }
        else if (other.TryGetComponent<ZombieHalf>(out ZombieHalf half))
        {
            zombieHalfCount++;
            if (zombieHalfCount >= 2)
            {
                // Award points for 2 zombie halves (equivalent to 2 silver coins)
                AwardPoints(silverCoinValue * 2);
                zombieHalfCount = 0;
            }
            Destroy(half.gameObject);
        }
    }

    private void AwardPoints(int points)
    {
        // Determine which player is closer to the furnace to award them the points
        GameObject redPlayer = GameObject.FindGameObjectWithTag("RedPlayer");
        GameObject bluePlayer = GameObject.FindGameObjectWithTag("BluePlayer");

        if (redPlayer != null && bluePlayer != null)
        {
            float distanceToRed = Vector2.Distance(transform.position, redPlayer.transform.position);
            float distanceToBlue = Vector2.Distance(transform.position, bluePlayer.transform.position);

            if (distanceToRed <= distanceToBlue)
            {
                // Award points to player 1 (red)
                GameManager.Instance?.AddPointsToPlayer1(points);
                Debug.Log($"Awarded {points} points to Player 1");
            }
            else
            {
                // Award points to player 2 (blue)
                GameManager.Instance?.AddPointsToPlayer2(points);
                Debug.Log($"Awarded {points} points to Player 2");
            }
        }
        else
        {
            // If we can't find both players, just add to total score
            GameManager.Instance?.AddScore(points);
            Debug.Log($"Awarded {points} points to total score");
        }
    }
}
