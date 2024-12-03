using System.Collections;
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

    public float reviveDistance = 1.5f;
    public float reviveTime = 3f;
    private float reviveCounter = 0f;

    public GameObject reviveBarUI;
    public Slider reviveSlider;

    public GameObject dashBarUI;
    public Slider dashSlider;

    public float dashSpeed = 10f;
    public float dashCooldown = 2f;
    public float dashTime = 0.25f;
    private float dashCooldownTimer = 0f;
    private bool dashing;
    public Animator animator;

    public GameObject playerClone;

    private AudioSource audioSoure;
    public AudioClip dashSound;

    private Color originalColor;          // The original color of the sprite for resetting
    public float flashDuration = 0.1f;    // Duration of the flash
    private float flashTimer = 0f;

    private float invincibilityTimer = 0f;
    private bool invincibility = false;

    //Sploog
    public bool splooged;
    public float sploogedDuration = 5f; // Duration for being splooged
    private float sploogedTimer = 0f;   // Timer to track splooged state
    public float sploogedSpeedMultiplier = 0.5f; // Speed reduction when splooged
    public GameObject sploogeSprite;

    private float particleTimer = 0f;
    public GameObject walkEffect;
    private UpgradeManager upgradeManager;

    private void Start()
    {
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        audioSoure = GetComponent<AudioSource>();
        shooting = GetComponent<Gun>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
        mainSprite = spriteRenderer.sprite;
        reviveBarUI.SetActive(false);
        animator.SetInteger("PlayerNumber", playerNumber);

        originalColor = spriteRenderer.color;
    }

    void Update()
    {
        // Flash timer for visual effects
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0f)
            {
                spriteRenderer.color = originalColor;
            }
        }

        // Handle Splooged State
        if (splooged)
        {
            sploogedTimer -= Time.deltaTime;
            if (sploogedTimer <= 0f)
            {
                sploogeSprite.SetActive(false);
                splooged = false; // Automatically remove splooged state
                moveSpeedMultiplier = 1f; // Reset move speed multiplier
                dashBarUI.SetActive(true); // Re-enable dash UI
            }
        }

        // Block movement and dashing if the shop is open
        if (upgradeManager.shopOpen == true)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            animator.SetBool("isDashing", false);
            return;
        }

        // Handle invincibility
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                invincibility = false;
            }
        }

        // Handle knocked state
        if (knocked)
        {
            if (health < 0)
                health = 0;

            Movement otherPlayer = FindOtherPlayer();
            if (otherPlayer != null)
            {
                AttemptRevive(otherPlayer);
            }
            animator.SetBool("isMoving", false);
            animator.SetBool("isDashing", false);
            return;
        }

        // Get movement input
        float horizontalMove = Input.GetAxisRaw("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxisRaw("Vertical" + playerNumber);
        Vector2 movement = new Vector2(horizontalMove, verticalMove).normalized;

        // Apply movement
        if (!dashing)
        {
            rb.velocity = movement * (moveSpeed * moveSpeedMultiplier);

            if(movement.magnitude > 0)
            {
                particleTimer += Time.deltaTime;

                if (particleTimer > 0.1f)
                {
                    Vector3 spawnPoisiton = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
                    Instantiate(walkEffect, spawnPoisiton, Quaternion.identity);
                    particleTimer = 0f;
                }
            }

        }
        else
        {
            rb.velocity = movement * (dashSpeed * moveSpeedMultiplier);
        }

        // Handle sprite flipping
        if (horizontalMove != 0)
        {
            spriteRenderer.flipX = horizontalMove < 0;
        }

        // Update animator states
        animator.SetBool("isMoving", movement.magnitude > 0 && !dashing);
        animator.SetBool("isDashing", dashing);

        // Handle dash cooldown
        if (dashCooldownTimer > 0)
        {
            dashSlider.value = dashCooldownTimer / dashCooldown;
            dashCooldownTimer -= Time.deltaTime;
        }
        else
        {
            dashBarUI.SetActive(false);
        }

        // Handle dash input
        if ((Input.GetButtonDown("Submit" + playerNumber)) && dashCooldownTimer <= 0 && !splooged)
        {
            Dash();
        }
    }

    // Apply the splooged state
    public void ApplySploog()
    {
        if (splooged)
            return;

        sploogeSprite.SetActive(true);
        splooged = true;
        sploogedTimer = sploogedDuration;    // Set the duration
        moveSpeedMultiplier = sploogedSpeedMultiplier; // Reduce speed
        dashBarUI.SetActive(false);         // Disable dash UI
        rb.velocity = Vector2.zero;         // Stop movement immediately
    }

    // Dash method remains unchanged
    void Dash()
    {   
        audioSoure.PlayOneShot(dashSound, 0.2f);
        dashing = true;
        StartCoroutine(ResetVelocityAfterDash(dashTime));
    }


    IEnumerator ResetVelocityAfterDash(float dashDuration)
    {
        StartCoroutine(DashEffect());
        yield return new WaitForSeconds(dashDuration);
        dashCooldownTimer = dashCooldown;
        dashBarUI.SetActive(true);
        rb.velocity = Vector2.zero;
        dashing = false;
    }

    private void FlashRed()
    {
        spriteRenderer.color = Color.red;
        flashTimer = flashDuration;
    }


    private void FlashGreen()
    {
        spriteRenderer.color = Color.green;
        flashTimer = flashDuration;
    }

    public void TakeDamage(int damage)
    {
        if (invincibility)
            return;

        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        health -= damage;
        FlashRed();
        CalculateHealth();

        if (health <= 0)
        {
            Die();
        }

        invincibility = true;
        invincibilityTimer = 0.1f;
    }

    public void Heal(float healingAmount)
    {
        health += Mathf.RoundToInt(healingAmount);
        FlashGreen();

        if (health > maxHealth)
            health = maxHealth;

        CalculateHealth();
    }

    void Die()
    {
        GetComponent<Gun>().gunSprite.gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = knockdownSprite;
        knocked = true;
        animator.SetBool("isKnocked", true);
        shooting.enabled = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        if (AreBothPlayersKnocked())
        {
            Gamemanager.instance.GameLost();
        }

        reviveBarUI.SetActive(true);
    }

    public void AttemptRevive(Movement otherPlayer)
    {
        float distance = Vector2.Distance(transform.position, otherPlayer.transform.position);

        if (distance <= reviveDistance)
        {
            reviveCounter += Time.deltaTime;
            reviveSlider.value = reviveCounter / reviveTime;
            reviveBarUI.SetActive(true);

            if (reviveCounter >= reviveTime)
            {
                if(playerNumber == 1)
                    Gamemanager.instance.UpdatePlayerStats(2, 0, 0, 1, 0);

                if (playerNumber == 2)
                    Gamemanager.instance.UpdatePlayerStats(1, 0, 0, 1, 0);

                Revive();
            }
        }
        else
        {
            reviveCounter = 0f;
            reviveSlider.value = 0f;
            reviveBarUI.SetActive(false);
        }
    }

    IEnumerator DashEffect()
    {
        SpriteRenderer playerSprite = gameObject.GetComponentInChildren<SpriteRenderer>();

        while (dashing)
        {
            GameObject clone = Instantiate(playerClone, playerSprite.gameObject.transform.position, Quaternion.identity);
            clone.GetComponent<SpriteRenderer>().sprite = playerSprite.sprite;
            clone.GetComponent<SpriteRenderer>().flipX = playerSprite.flipX;
            Destroy(clone, 0.2f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Revive()
    {
        GetComponent<Gun>().gunSprite.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = true;
        animator.SetBool("isKnocked", false);
        shooting.enabled = true;
        rb.isKinematic = false;
        health = maxHealth / 4;
        knocked = false;
        reviveCounter = 0f;
        spriteRenderer.sprite = mainSprite;
        CalculateHealth();
        reviveBarUI.SetActive(false);
    }

    void CalculateHealth()
    {
        healthCount.text = health.ToString() + "/" + maxHealth.ToString();
        healthbar.value = ((float)health / (float)maxHealth);
    }

    Movement FindOtherPlayer()
    {
        foreach (Movement player in FindObjectsOfType<Movement>())
        {
            if (player != this && !player.knocked)
            {
                return player;
            }
        }
        return null;
    }

    bool AreBothPlayersKnocked()
    {
        foreach (Movement player in FindObjectsOfType<Movement>())
        {
            if (!player.knocked)
            {
                return false;
            }
        }
        return true;
    }
}
