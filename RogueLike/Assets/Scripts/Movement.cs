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

    private AudioSource audioSoure;
    public AudioClip dashSound;

    private void Start()
    {
        audioSoure = GetComponent<AudioSource>();
        shooting = GetComponent<Gun>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
        mainSprite = spriteRenderer.sprite;
        reviveBarUI.SetActive(false);
        animator.SetInteger("PlayerNumber", playerNumber);
    }

    void Update()
    {
        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            animator.SetBool("isDashing", false);
            return;
        }

        if (knocked)
        {
            Movement otherPlayer = FindOtherPlayer();
            if (otherPlayer != null)
            {
                AttemptRevive(otherPlayer);
            }
            animator.SetBool("isMoving", false);
            animator.SetBool("isDashing", false);
            return;
        }

        float horizontalMove = Input.GetAxisRaw("Horizontal" + playerNumber);
        float verticalMove = Input.GetAxisRaw("Vertical" + playerNumber);

        Vector2 movement = new Vector2(horizontalMove, verticalMove).normalized;

        if (!dashing)
        {
            rb.velocity = movement * (moveSpeed * moveSpeedMultiplier);
        }
        else
        {
            rb.velocity = movement * (dashSpeed * moveSpeedMultiplier);
        }

        if (horizontalMove != 0)
        {
            spriteRenderer.flipX = horizontalMove < 0;
        }

        animator.SetBool("isMoving", movement.magnitude > 0 && !dashing);
        animator.SetBool("isDashing", dashing);

        if (dashCooldownTimer > 0)
        {
            dashSlider.value = dashCooldownTimer / dashCooldown;
            dashCooldownTimer -= Time.deltaTime;
        }
        else
            dashBarUI.SetActive(false);
        if ((Input.GetButtonDown("Submit" + playerNumber)) && dashCooldownTimer <= 0)
        {
            Dash();
        }
    }

    void Dash()
    {
        audioSoure.PlayOneShot(dashSound, 0.2f);
        dashing = true;
        StartCoroutine(ResetVelocityAfterDash(dashTime));
    }

    IEnumerator ResetVelocityAfterDash(float dashDuration)
    {
        yield return new WaitForSeconds(dashDuration);
        dashCooldownTimer = dashCooldown;
        dashBarUI.SetActive(true);
        rb.velocity = Vector2.zero;
        dashing = false;
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

    void Revive()
    {
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
