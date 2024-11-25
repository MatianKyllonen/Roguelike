using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealingDrone : MonoBehaviour
{
    public float healingRange = 5f; // The radius in which the drone heals
    public int healingAmount = 5; // The amount of health restored
    public float healingInterval = 15f; // How often the drone heals (in seconds)
    public float healAmountMultiplier = 1f; // Allows for upgradeable healing amount (via upgrades)

    private float nextHealingTime = 0f; // Timer for when to heal next
    public Slider healingSlider;
    public int playerNumber;

    private void Start()
    {
        playerNumber = GetComponent<DroneBasic>().target.GetComponent<Movement>().playerNumber;
    }
    void Update()
    {
        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        healingSlider.value = (Time.time - nextHealingTime + healingInterval) / healingInterval;



        if (Time.time > nextHealingTime)
        {
            HealPlayerInRange();
            nextHealingTime = Time.time + healingInterval;
        }
    }

    void HealPlayerInRange()
    {
        // Find all players within the healing range
        Collider2D[] playersInRange = Physics2D.OverlapCircleAll(transform.position, healingRange);

        foreach (Collider2D playerCollider in playersInRange)
        {
            Gamemanager.instance.UpdatePlayerStats(playerNumber, 0, 0, 0, healingAmount);
            // Check if the object is a player (you can adjust this check based on how your game identifies the player)
            Movement player = playerCollider.GetComponent<Movement>();
            if (player != null && !player.knocked)
            {
                // Heal the player
                player.Heal(healingAmount * healAmountMultiplier);
            }
        }
    }
}
