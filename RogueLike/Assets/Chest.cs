using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI elements like sliders

public class Chest : MonoBehaviour
{
    [Header("Chest Settings")]
    public float openTime = 2.0f; // Time required to open the chest
    public GameObject[] lootPrefabs; // Array of loot GameObjects to drop
    public Transform lootSpawnPoint; // Point where loot will spawn
    public Slider interactionSlider; // UI slider to show progress
    public GameObject interactionUI; // UI for interaction
    public Sprite chestOpenSprite; // Sprite for opened chest

    private float interactionCounter = 0f;
    private bool isOpened = false;
    private bool isPlayerNearby = false; // Tracks if player is in the trigger zone
    private Transform playerTransform; // Stores the player's transform

    void Start()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void Update()
    {
        // Handle interaction progress if the player is nearby
        if (isPlayerNearby && !isOpened)
        {
            interactionCounter += Time.deltaTime;
            if (interactionSlider != null)
            {
                interactionSlider.value = interactionCounter / openTime;
            }

            if (interactionUI != null)
            {
                interactionUI.SetActive(true);
            }

            if (interactionCounter >= openTime)
            {
                OpenChest();
            }
        }
    }

    private void OpenChest()
    {
        isOpened = true;

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }

        interactionCounter = 0f;

        // Spawn loot between 1 and 3 items
        int lootCount = Random.Range(1, 4); // Range is inclusive, so 1 to 3
        for (int i = 0; i < lootCount; i++)
        {
            if (lootPrefabs.Length > 0)
            {
                GameObject loot = Instantiate(lootPrefabs[Random.Range(0, lootPrefabs.Length)], lootSpawnPoint.position, Quaternion.identity);
                loot.transform.position += Random.insideUnitSphere * 0.5f; // Add slight randomness to the spawn position
            }
        }

        // Change the chest's sprite to the opened version
        GetComponentInChildren<SpriteRenderer>().sprite = chestOpenSprite;

        // Optionally destroy the chest after some time
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = collision.transform; // Store the player's transform
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerTransform = null; // Clear the stored player transform
            ResetInteraction();
        }
    }

    private void ResetInteraction()
    {
        interactionCounter = 0f;

        if (interactionSlider != null)
        {
            interactionSlider.value = 0f;
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}
