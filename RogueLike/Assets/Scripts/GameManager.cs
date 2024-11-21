using System.Collections;
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

        if (Input.GetKeyDown(KeyCode.L) && !upgradesManager.shopOpen)
        {
            LevelUp();
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
        music.GetComponent<AudioSource>().Stop();
        StartCoroutine(GamelostDelay());
    }

    private IEnumerator GamelostDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

}
