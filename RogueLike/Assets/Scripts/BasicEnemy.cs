using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float moveSpeed = 3f;  // Speed at which the enemy moves
    private Transform targetPlayer;  // The transform of the nearest player
    private EnemySpawner spawner;

    public int maxHealth = 100;
    private int health = 0;

    public int damage;

    // Sprite-related variables
    public Sprite[] enemySprites;  // Array to hold the possible sprites
    private SpriteRenderer spriteRenderer;  // The sprite renderer to change the sprite and color
    private Color originalColor;  // The original color of the sprite for resetting

    // Flashing effect parameters
    public float flashDuration = 0.1f;  // Duration of the flash
    private float flashTimer = 0f;  // Timer to track the flash duration

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<EnemySpawner>();
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer component
        health = maxHealth;

        spawner.EnemySpawned();

        // Randomly assign a sprite from the list
        if (enemySprites.Length > 0)
        {
            int randomIndex = Random.Range(0, enemySprites.Length);
            spriteRenderer.sprite = enemySprites[randomIndex];  // Set a random sprite
        }

        // Save the original color to reset after flashing
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
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

            if (distanceToPlayer < closestDistance)
            {
                closestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }

        // If a nearest player was found, set the target to that player's transform
        if (nearestPlayer != null)
        {
            targetPlayer = nearestPlayer.transform;

            // Move the enemy towards the player
            MoveTowardsPlayer();

            // Flip the sprite to face the player
            FlipSprite();
        }

        // Handle the flash effect if damage is taken
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                spriteRenderer.color = originalColor;  // Reset color after the flash
            }
        }
    }

    // Moves the enemy towards the nearest player
    void MoveTowardsPlayer()
    {
        if (targetPlayer == null)
            return;

        // Calculate the direction from the enemy to the player
        Vector3 direction = (targetPlayer.position - transform.position).normalized;

        // Move the enemy towards the player
        transform.position = Vector3.MoveTowards(transform.position, targetPlayer.position, moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Movement>().TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        // Trigger the red flash when the enemy takes damage
        FlashRed();

        if (health <= 0)
        {
            Die();
        }
    }

    // Flash the enemy sprite red for a short time
    private void FlashRed()
    {
        spriteRenderer.color = Color.red;  // Change the sprite color to red
        flashTimer = flashDuration;  // Start the flash timer
    }

    // Flip the sprite to face the player based on their position
    private void FlipSprite()
    {
        if (targetPlayer == null)
            return;

        // Check if the player is to the left or right of the enemy
        if (targetPlayer.position.x < transform.position.x)
        {
            // Flip the sprite to face left
            spriteRenderer.flipX = true;
        }
        else
        {
            // Flip the sprite to face right
            spriteRenderer.flipX = false;
        }
    }

    void Die()
    {
        spawner.EnemyDestroyed();
        Destroy(gameObject);
    }
}
