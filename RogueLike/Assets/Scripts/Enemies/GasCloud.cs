using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GasCloud : MonoBehaviour
{
    public float initialDamagePercentage = 5f;  // Initial damage percentage
    public float rampUpRate = 2f;               // Damage increase rate over time
    public GameObject cloud;
    public float damageInterval = 1f;           // Time interval for damage application
    public float cloudGrowDuration = 2f;        // Duration for the cloud to grow from zero to full size
    public float cloudShrinkDuration = 2f;      // Duration for the cloud to shrink to zero
    public Light2D gasLight;                    // The light associated with the cloud

    private Dictionary<GameObject, float> entitiesInCloud = new Dictionary<GameObject, float>();
    private bool cloudShrinking = false;

    void Start()
    {
        // Start the cloud growth Coroutine
        StartCoroutine(GrowCloud());
    }

    void Update()
    {
        // Iterate over all entities in the gas cloud
        foreach (var entity in new List<GameObject>(entitiesInCloud.Keys))
        {
            if (entity == null)
            {
                entitiesInCloud.Remove(entity);
                continue;
            }

            // Calculate the ramping up damage
            float elapsedDamageTime = entitiesInCloud[entity];
            float currentDamagePercentage = initialDamagePercentage + rampUpRate * (Time.time - elapsedDamageTime);

            // Deal damage at regular intervals
            if (Time.time >= elapsedDamageTime + damageInterval)
            {
                Movement health = entity.GetComponent<Movement>();  // Assuming Movement has a TakeDamage method

                if (health != null)
                {
                    int damage = Mathf.RoundToInt(health.maxHealth * (currentDamagePercentage / 100f));  // Calculate damage based on max health
                    health.TakeDamage(damage);
                    entitiesInCloud[entity] = Time.time;  // Update the last damage time
                }
            }
        }

        // If the cloud is shrinking and all entities are removed, destroy it
        if (cloudShrinking && entitiesInCloud.Count == 0)
        {
            Destroy(cloud);  // Destroy the cloud GameObject
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If a player enters the gas cloud, start tracking their damage
        if (other.CompareTag("Player") && !entitiesInCloud.ContainsKey(other.gameObject))
        {
            entitiesInCloud[other.gameObject] = Time.time;  // Track when the player entered the cloud
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the player from the tracking list when they exit the cloud
        if (entitiesInCloud.ContainsKey(other.gameObject))
        {
            entitiesInCloud.Remove(other.gameObject);
        }
    }

    // Coroutine to grow the cloud from zero to its normal size
    private IEnumerator GrowCloud()
    {
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = cloud.transform.localScale;  // Assuming the cloud starts at its full size
        float elapsedTime = 0f;

        while (elapsedTime < cloudGrowDuration)
        {
            // Gradually increase light intensity from 0 to 1
            gasLight.intensity = Mathf.Lerp(0, 1, elapsedTime / cloudGrowDuration);
            cloud.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / cloudGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cloud.transform.localScale = targetScale;  // Ensure the cloud reaches its full size
        gasLight.intensity = 1;  // Ensure the light reaches full intensity
    }

    // Coroutine to shrink the cloud from its current size to zero
    private IEnumerator ShrinkCloud()
    {
        Vector3 initialScale = cloud.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float elapsedTime = 0f;

        while (elapsedTime < cloudShrinkDuration)
        {
            // Gradually decrease light intensity from 1 to 0
            gasLight.intensity = Mathf.Lerp(1, 0, elapsedTime / cloudShrinkDuration);
            cloud.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / cloudShrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cloud.transform.localScale = targetScale;  // Ensure the cloud shrinks to zero size
        gasLight.intensity = 0;  // Ensure the light intensity reaches zero
        Destroy(cloud);  // Destroy the cloud once it's fully shrunk
    }

    // Call this method when you want to trigger the shrinking of the cloud (e.g., when it's time to destroy the cloud)
    public void TriggerShrink()
    {
        if (!cloudShrinking)
        {
            cloudShrinking = true;
            StartCoroutine(ShrinkCloud());
        }
    }
}
