using System.Collections.Generic;
using UnityEngine;

public class LevelPellet : MonoBehaviour
{
    private Gamemanager gm;
    public int xpAmount = 10;

    public AudioClip pickupSound;
    private AudioSource audioSource;

    // Removed pellet's detection radius and speed, instead use player's detection radius
    private Transform closestPlayerTransform;  // Reference to the closest player's position

    private List<Transform> playerTransforms = new List<Transform>(); // List to store both player transforms

    void Start()
    {
        audioSource = FindObjectOfType<Movement>().gameObject.GetComponent<AudioSource>();
        Destroy(gameObject, 90f);
        gm = Gamemanager.instance;

        // Find players by tag and add their transforms to the list
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            playerTransforms.Add(player.transform);
        }
    }

    void Update()
    {
        if (playerTransforms.Count > 0)
        {
            // Find the closest player
            closestPlayerTransform = GetClosestPlayer();

            if (closestPlayerTransform != null)
            {
                Movement playerMovement = closestPlayerTransform.GetComponent<Movement>(); // Get the Movement component
                float distanceToPlayer = Vector2.Distance(transform.position, closestPlayerTransform.position);

                // Check if the pellet is within the player's detection radius
                if (distanceToPlayer <= playerMovement.detectionRadius)
                {
                    // Move the pellet toward the closest player
                    transform.position = Vector2.Lerp(transform.position, closestPlayerTransform.position, 10 * Time.deltaTime / distanceToPlayer);
                }
            }
        }
    }

    private Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform playerTransform in playerTransforms)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestPlayer = playerTransform;
            }
        }

        return closestPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Ensure it only triggers when colliding with a player
        {
            gm.IncreaseXp(xpAmount);

            if (collision.GetComponent<Movement>().greedyCollector == true)
            {
                if (xpAmount >= 75)
                    xpAmount = 75;
                collision.GetComponent<Movement>().Heal(xpAmount / 3.5f, true);
            }
            audioSource.pitch = (Random.Range(0.9f, 1.1f));
            audioSource.PlayOneShot(pickupSound, 0.2f);         
            Destroy(gameObject); // Destroy the pellet after it's collected
        }
    }
}
