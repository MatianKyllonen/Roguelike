using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> enemyPrefabs;    // List of enemy prefabs to spawn
    [SerializeField] private TextMeshProUGUI waveCounter;      // UI Text to display the wave count
    [SerializeField] private List<Transform> spawnPoints;      // List of potential spawn points

    [Header("Attributes")]
    [SerializeField] private int baseWaveWeight = 50;          // Total "weight" of enemies to spawn per wave
    [SerializeField] private float spawnInterval = 1f;         // Time between enemy spawns
    [SerializeField] private float timeBetweenWaves = 5f;      // Time between each wave

    [SerializeField] private float spawnIntervalDecrement = 0.05f; // How much the spawn interval decreases per wave
    [SerializeField] private float minSpawnInterval = 0.3f;         // Minimum spawn interval

    private int currentWave = 0;            // The current wave number
    private int waveWeight;                 // The weight limit of enemies for the current wave
    private int enemiesAlive = 0;           // Number of enemies currently alive
    private bool isSpawning = false;        // Whether the spawner is actively spawning enemies
    private int remainingWeight = 0;
    private void Start()
    {
        StartCoroutine(StartWave()); // Start the first wave
    }

    private void Update()
    {
        // If enemies are still alive or spawning, don't end the wave
        if (enemiesAlive <= 0 && !isSpawning)
        {
            StartCoroutine(StartWave()); // Start the next wave
        }
    }

    private IEnumerator StartWave()
    {
        currentWave += 1;
        isSpawning = true;

        // Increase the weight limit per wave based on a difficulty curve
        waveWeight = baseWaveWeight + Mathf.FloorToInt(currentWave * 10f);  // Adjust wave weight per wave
        waveCounter.text = "Wave: " + currentWave;    // Update wave counter UI

        yield return new WaitForSeconds(timeBetweenWaves); // Wait before starting the wave

        // Decrease the spawn interval as the game progresses
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrement); // Prevent going below a threshold

        remainingWeight = waveWeight; // Track remaining weight for the current wave

        // Loop until the wave's weight is filled
        while (remainingWeight > 0)
        {
            GameObject enemyToSpawn = SelectEnemy(remainingWeight); // Select an eligible enemy
            if (enemyToSpawn == null) break; // Break if no enemies fit the remaining weight

            // Spawn the selected enemy at a random spawn point
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(enemyToSpawn, randomSpawnPoint.position, Quaternion.identity);

            remainingWeight -= enemyToSpawn.GetComponent<EnemyInfo>().weight; // Deduct enemy weight from remaining weight         
            yield return new WaitForSeconds(spawnInterval); // Wait before spawning the next enemy
        }

        isSpawning = false;             // Wave spawning finished
    }

    // Select an eligible enemy based on the remaining weight and current wave number
    private GameObject SelectEnemy(int remainingWeight)
    {
        List<GameObject> eligibleEnemies = new List<GameObject>();

        // Filter eligible enemies based on minWaveToSpawn and weight
        foreach (GameObject enemy in enemyPrefabs)
        {
            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>();
            if (enemyInfo.minWaveToSpawn <= currentWave && enemyInfo.weight <= remainingWeight)
            {
                eligibleEnemies.Add(enemy);
            }
        }

        // Return a random eligible enemy, or null if none are available
        if (eligibleEnemies.Count > 0)
        {
            return eligibleEnemies[Random.Range(0, eligibleEnemies.Count)];
        }
        return null;
    }

    public void EnemySpawned()
    {
        enemiesAlive++;
    }

    public void EnemyDestroyed()
    {
        enemiesAlive--;                 // Reduce enemies alive count when an enemy is destroyed
    }
}
