using System.Collections;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public float moveSpeed = 2f;               // Speed of the summoner's movement
    public float stopDistance = 5f;           // Distance to maintain from the player
    public float summonInterval = 3f;         // Time interval between summons
    public GameObject enemyPrefab;            // Prefab of the enemy to summon
    public GameObject summoningEffectPrefab;  // Prefab for the summoning visual effect

    private Transform targetPlayer;           // The transform of the nearest player
    private float summonTimer = 0f;           // Timer to track summon intervals

    public Animator animator;                 // Animator for handling animations
    private bool isSummoning = false;         // Whether the summoner is currently summoning

    void Start()
    {
        // Initialize the summon timer
        summonTimer = 0;
    }

    void Update()
    {
        if (FindObjectOfType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        // Find all game objects tagged as "Player"
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // If there are no players, return
        if (players.Length == 0)
            return;

        // Find the closest player
        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject player in players)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < closestDistance && !player.gameObject.GetComponent<Movement>().knocked)
            {
                closestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        // If a nearest player was found, set the target to that player's transform
        if (nearestPlayer != null)
        {
            targetPlayer = nearestPlayer.transform;

            // Move the summoner towards the player or stop at a distance
            if (targetPlayer == null || isSummoning)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);

            if (distanceToPlayer > stopDistance || summonTimer > 0f)
            {
                Vector3 direction = (targetPlayer.position - transform.position).normalized;

                transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
            }
            else if (distanceToPlayer <= stopDistance)
            {
                
                if (summonTimer <= 0f)
                {
                    StartCoroutine(SummonEnemy());
                    summonTimer = summonInterval;
                }
            }

            summonTimer -= Time.deltaTime;
        }
    }

    IEnumerator SummonEnemy()
    {
        isSummoning = true;

        // Play summoning animation
        if (animator != null)
        {
            animator.SetTrigger("Summon");
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 summonPosition = transform.position;
        if (targetPlayer != null)
        {
            summonPosition += (targetPlayer.position - transform.position).normalized;
        }

        if (summoningEffectPrefab != null)
        {
            Destroy(Instantiate(summoningEffectPrefab, summonPosition, Quaternion.identity), 0.8f);
        }

        yield return new WaitForSeconds(0.8f);



 

        // Spawn the enemy
        if (enemyPrefab != null)
        {
            Instantiate(enemyPrefab, summonPosition, Quaternion.identity);
        }

        isSummoning = false;
    }
}
