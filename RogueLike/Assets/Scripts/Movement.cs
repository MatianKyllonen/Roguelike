using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    public int maxHealth = 100;
    int health = 0;

    public float moveSpeed = 5f;
    public float moveSpeedMultiplier = 1f;
    public int playerNumber = 1;
    public bool knocked;

    private Vector2 movementInput;
    private float horizontalMove;
    private float verticalMove;
    private SpriteRenderer spriteRenderer;
    private Sprite mainSprite;
    public Sprite knockdownSprite;
    Rigidbody2D rb;

    public Slider healthbar;
    public TextMeshProUGUI healthCount;
    private Gun shooting;

    // New revive variables
    public float reviveDistance = 1.5f; // Distance within which the other player needs to be for revival
    public float reviveTime = 3f; // Time required to revive
    private float reviveCounter = 0f; // Countdown timer for revival

    // Revive bar UI references
    public GameObject reviveBarUI; // Reference to the revive bar UI (entire panel or bar)
    public Slider reviveSlider; // The actual revive progress slider

    private void Start()
    {
        shooting = GetComponent<Gun>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
        mainSprite = spriteRenderer.sprite;

        // Ensure the revive bar is hidden at the start
        reviveBarUI.SetActive(false);
    }

    void Update()
    {
        // Regular movement if not knocked down
        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Check for revive condition if knocked
        if (knocked)
        {
            // Attempt to revive if there is another player nearby
            Movement otherPlayer = FindOtherPlayer();

            if (otherPlayer != null)
            {
                AttemptRevive(otherPlayer);
            }
            return;
        }

        float horizontalMove = Input.GetAxisRaw("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxisRaw("Vertical" + playerNumber);

        // Create a normalized movement vector
        Vector2 movement = new Vector2(horizontalMove, verticalMove).normalized;

        // Update Rigidbody velocity for instant movement
        rb.velocity = movement * (moveSpeed * moveSpeedMultiplier);

        // Flip sprite based on horizontal movement direction
        if (horizontalMove != 0)
        {
            spriteRenderer.flipX = horizontalMove < 0;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        CalculateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        spriteRenderer.sprite = knockdownSprite;
        knocked = true;
        shooting.enabled = false;
        rb.velocity = Vector2.zero; // Stop movement on knockdown
        rb.isKinematic = true; // Make Rigidbody kinematic to prevent movement

        // Check if both players are knocked down
        if (AreBothPlayersKnocked())
        {
            Gamemanager.instance.GameLost();
        }

        // Show revive bar when knocked down
        reviveBarUI.SetActive(true);
    }

    // New method for attempting to revive the player
    public void AttemptRevive(Movement otherPlayer)
    {
        float distance = Vector2.Distance(transform.position, otherPlayer.transform.position);

        if (distance <= reviveDistance)
        {
            reviveCounter += Time.deltaTime;
            reviveSlider.value = reviveCounter / reviveTime; // Update the revive bar based on time elapsed

            // Show the revive bar UI while reviving
            reviveBarUI.SetActive(true);

            if (reviveCounter >= reviveTime)
            {
                Revive();
            }
        }
        else
        {
            // Reset revive counter if the other player moves out of range
            reviveCounter = 0f;
            reviveSlider.value = 0f; // Reset the revive progress bar

            // Hide the revive bar if out of range
            reviveBarUI.SetActive(false);
        }
    }

    void Revive()
    {
        shooting.enabled = true;
        rb.isKinematic = false;
        health = maxHealth / 4; // Restore a quarter of max health
        knocked = false;
        reviveCounter = 0f;
        spriteRenderer.sprite = mainSprite; // Change back to the original sprite
        CalculateHealth();

        // Hide the revive bar after successful revive
        reviveBarUI.SetActive(false);
    }

    void CalculateHealth()
    {
        healthCount.text = health.ToString() + "/" + maxHealth.ToString();
        healthbar.value = ((float)health / (float)maxHealth);
    }

    // Helper method to find the other player for revival or to check knockdown state
    Movement FindOtherPlayer()
    {
        foreach (Movement player in FindObjectsOfType<Movement>())
        {
            if (player != this && !player.knocked)
            {
                return player; // Return the first alive player found
            }
        }
        return null;
    }

    // Method to check if both players are knocked down
    bool AreBothPlayersKnocked()
    {
        foreach (Movement player in FindObjectsOfType<Movement>())
        {
            if (!player.knocked)
            {
                return false; // If at least one player is not knocked, return false
            }
        }
        return true; // All players are knocked
    }
}
