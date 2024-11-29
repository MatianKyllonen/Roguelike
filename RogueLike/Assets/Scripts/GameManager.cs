using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager instance;

    private int level = 0;
    private float nextlevelXp = 100;
    private int currentXp = 0;
    private float xpMultiplier = 1.3f;

    private UpgradeManager upgradesManager;

    public Slider levelBar;
    public TextMeshProUGUI levelCount;

    public TextMeshProUGUI timer;
    public GameObject music;

    private float elapsedTime = 0f; // Keeps track of time

    public GameObject gameOverScreen;
    public GameObject gameOverStatsPlayer1;
    public GameObject gameOverStatsPlayer2;

    public GameObject fade;
    private bool gameLost;

    // Stats tracking for two players
    private List<int[]> playerStats = new List<int[]>
    {
        new int[4], // Player 1 stats: [Kills, Damage, Revives, Healing]
        new int[4]  // Player 2 stats: [Kills, Damage, Revives, Healing]
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        upgradesManager = FindFirstObjectByType<UpgradeManager>();
    }

    private void Update()
    {
        // Update the timer
        UpdateTimer();


        if (Input.GetButtonDown("Submit1") && gameLost)
        {
            StartCoroutine(GameRestart());
        }
        if (Input.GetKeyDown(KeyCode.L) && !upgradesManager.shopOpen)
        {
            LevelUp();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
           GameLost();
        }
    }

    public void IncreaseXp(int amount)
    {
        currentXp += amount;

        CalculateXp();

        if (currentXp >= nextlevelXp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        level += 1;
        currentXp = 0;

        nextlevelXp *= xpMultiplier;

        CalculateXp();

        upgradesManager.OpenShop();
    }

    void CalculateXp()
    {
        levelCount.text = "Level: " + level;
        levelBar.value = ((float)currentXp / nextlevelXp);
    }

    void UpdateTimer()
    {
        if (FindFirstObjectByType<UpgradeManager>().shopOpen == true)
        {
            return;
        }

        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        // Format and display the timer text
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameLost()
    {
        // Stop the music
        music.GetComponent<AudioSource>().Stop();

        StartCoroutine(GamelostDelay());
        
    }

    private IEnumerator GamelostDelay()
    {
        yield return new WaitForSeconds(2f); // Delay before showing Game Over screen
        gameOverScreen.SetActive(true); // Enable Game Over screen
        yield return new WaitForSeconds(1.2f); // Additional delay before showing stats
        DisplayGameOverStats(); // Show stats for Player 1 and Player 2
        gameLost = true;
    }

    private void DisplayGameOverStats()
    {
        // Display stats for Player 1
        StartCoroutine(ActivateStatsWithDelay(gameOverStatsPlayer1, playerStats[0]));

        // Display stats for Player 2
        StartCoroutine(ActivateStatsWithDelay(gameOverStatsPlayer2, playerStats[1]));
    }

    private IEnumerator ActivateStatsWithDelay(GameObject statsPanel, int[] stats)
    {
        // Ensure the panel is disabled initially
        statsPanel.SetActive(false);

        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Activate the panel and update stats
        statsPanel.SetActive(true);
        UpdateGameOverStatsUI(statsPanel, stats);
    }


    private IEnumerator GameRestart()
    {
        gameLost = false;
        fade.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(1f);      
        SceneManager.LoadScene(0);
    }

    private void UpdateGameOverStatsUI(GameObject statsPanel, int[] stats)
    {
        if (statsPanel == null)
        {
            Debug.LogError("Stats panel not set for Game Over screen!");
            return;
        }

        // Look for children named "Kills", "Damage", "Revives", and "Healing" under the given stats panel
        Transform killsObj = statsPanel.transform.Find("Kills");
        Transform damageObj = statsPanel.transform.Find("Damage");
        Transform revivesObj = statsPanel.transform.Find("Revives");
        Transform healingObj = statsPanel.transform.Find("Healing");

        // Update the text for each stat
        if (killsObj != null)
        {
            killsObj.GetComponentInChildren<TextMeshProUGUI>().text = "Kills: " + stats[0].ToString(); // Update Kills
        }
        if (damageObj != null)
        {
            damageObj.GetComponentInChildren<TextMeshProUGUI>().text = "Damage: " + stats[1].ToString(); // Update Damage
        }
        if (revivesObj != null)
        {
            revivesObj.GetComponentInChildren<TextMeshProUGUI>().text = "Revives: " + stats[2].ToString(); // Update Revives
        }
        if (healingObj != null)
        {
            healingObj.GetComponentInChildren<TextMeshProUGUI>().text = "Healing: " + stats[3].ToString(); // Update Healing
        }
    }

    // Update player stats
    public void UpdatePlayerStats(int playerIndex, int kills, int damage, int revives, int healing)
    {
        playerIndex -= 1;

        if (playerIndex < 0 || playerIndex > 2)
        {
            Debug.LogError("Invalid player index!");
            return;
        }

        playerStats[playerIndex][0] += kills;   // Update Kills
        playerStats[playerIndex][1] += damage; // Update Damage
        playerStats[playerIndex][2] += revives; // Update Revives
        playerStats[playerIndex][3] += healing; // Update Healing
    }

    // Display player stats for debugging
    public void DisplayPlayerStats()
    {
        for (int i = 0; i < playerStats.Count; i++)
        {
            Debug.Log($"Player {i + 1} Stats - Kills: {playerStats[i][0]}, Damage: {playerStats[i][1]}, Revives: {playerStats[i][2]}, Healing: {playerStats[i][3]}");
        }
    }
}
