using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPellet : MonoBehaviour
{
    private Gamemanager gm;
    public int xpAmount = 10;

    public AudioClip pickupSound;
    private AudioSource audioSource;

    // New variables for floating toward the closest player
    public float detectionRadius = 5f;  // Distance at which pellet starts moving toward player
    public float moveSpeed = 3f;        // Speed at which pellet moves toward player
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
                float distanceToPlayer = Vector2.Distance(transform.position, closestPlayerTransform.position);

                // Check if the closest player is within detection radius
                if (distanceToPlayer <= detectionRadius)
                {
                    // Move the pellet toward the closest player
                    transform.position = Vector2.Lerp(transform.position, closestPlayerTransform.position, moveSpeed * Time.deltaTime / distanceToPlayer);
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
            audioSource.pitch = (Random.Range(0.8f, 1.2f));
            audioSource.PlayOneShot(pickupSound, 0.5f);
            gm.IncreaseXp(xpAmount);
            Destroy(gameObject); // Destroy the pellet after it's collected
        }
    }
}
