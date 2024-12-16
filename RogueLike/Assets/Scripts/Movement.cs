using System.Collections;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
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
    public float damageTakenMultiplier = 1f;
    public float detectionRadius = 5f;

    private Vector2 movementInput;
    private float horizontalMove;
    private float verticalMove;
    private SpriteRenderer spriteRenderer;
    private Sprite mainSprite;
    public Sprite knockdownSprite;
    Rigidbody2D rb;

    public bool greedyCollector;

    public Slider healthbar;
    public Slider damageTookSlider;
    public TextMeshProUGUI healthCount;
    private Gun shooting;

    public float reviveDistance = 1.5f;
    public float reviveTime = 3f;
    private float reviveCounter = 0f;
    private float originalMoveSpeedMultiplier;

    

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

    private AudioSource audioSource;
    public AudioClip dashSound;
    public AudioClip hurtSound;

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
        audioSource = GetComponent<AudioSource>();
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
                moveSpeedMultiplier = originalMoveSpeedMultiplier; // Reset move speed multiplier
                dashBarUI.SetActive(true); // Re-enable dash U
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

        originalMoveSpeedMultiplier = moveSpeedMultiplier;
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
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(dashSound, 0.45f);
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
        flashTimer = flashDuration + 0.1f;
    }


    private void FlashGreen()
    {
        spriteRenderer.color = Color.green;
        flashTimer = flashDuration + 0.2f;
    }

    public void TakeDamage(int damage)
    {
        if (invincibility)
            return;

        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        health -= (Mathf.RoundToInt(damage * damageTakenMultiplier));

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(hurtSound, 0.5f);

        FindObjectOfType<ScreenShake>()?.TriggerShake();
        FlashRed();     
        CalculateHealth();
        StartCoroutine(ShowcaseDamage(healthbar.value));


        if (health <= 0)
        {
            Die();
        }

        invincibility = true;
        invincibilityTimer = 0.1f;
    }

    public void Heal(float healingAmount, bool isGem = false)
    {
        if (health < maxHealth)
        {
            if (!greedyCollector)
            {
                health += Mathf.RoundToInt(healingAmount);
            }
            else if (greedyCollector && isGem)
            {
                Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, 0, 0, Mathf.RoundToInt(healingAmount));
                health += Mathf.RoundToInt(healingAmount);
            }

            FlashGreen();
        }

        if (health > maxHealth)
        {
            health = maxHealth;
        }

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
        dashCooldownTimer = 0;

        health = 0;
        CalculateHealth();

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

    IEnumerator ShowcaseDamage(float Newvalue)
    {
        yield return new WaitForSeconds(0.4f);

        damageTookSlider.value = Newvalue;
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

        invincibility = true;
        invincibilityTimer = 1f;
    }

    void CalculateHealth()
    {
        healthCount.text = health.ToString();
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
