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
    [SerializeField] private int baseEnemiesPerWave = 10;      // Number of enemies to spawn per wave
    [SerializeField] private float spawnInterval = 1f;         // Time between enemy spawns
    [SerializeField] private float timeBetweenWaves = 5f;      // Time between each wave

    [SerializeField] private float spawnIntervalDecrement = 0.05f; // How much the spawn interval decreases per wave
    [SerializeField] private float minSpawnInterval = 0.3f;         // Minimum spawn interval

    private int currentWave = 0;            // The current wave number
    private int enemiesLeftToSpawn;         // Number of enemies left to spawn in the current wave
    private int enemiesAlive = 0;           // Number of enemies currently alive
    private bool isSpawning = false;        // Whether the spawner is actively spawning enemies

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

        // Increase enemies per wave and the difficulty curve
        enemiesLeftToSpawn = baseEnemiesPerWave + Mathf.FloorToInt(currentWave * 5f);  // Adjust the enemy scaling per wave
        waveCounter.text = "Wave: " + currentWave;    // Update wave counter UI

        yield return new WaitForSeconds(timeBetweenWaves); // Wait before starting the wave

        // Decrease the spawn interval as the game progresses
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - spawnIntervalDecrement); // Prevent going below a threshold

        while (enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();               // Spawn an enemy
            enemiesLeftToSpawn--;       // Decrement count of enemies left to spawn
            yield return new WaitForSeconds(spawnInterval); // Wait before spawning the next enemy
        }

        isSpawning = false;             // Wave spawning finished
    }

    private void SpawnEnemy()
    {
        // Select a random spawn point from the list
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Select a random enemy from the list
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // Instantiate the selected random enemy at the random spawn point
        Instantiate(randomEnemyPrefab, randomSpawnPoint.position, Quaternion.identity);
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
