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

    private UpgradeManager upgradesManager;

    public Slider levelBar;
    public TextMeshProUGUI levelCount;
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
        nextlevelXp = Mathf.RoundToInt(nextlevelXp * 1.5f);

        CalculateXp();

        upgradesManager.OpenShop();
    }

    void CalculateXp()
    {
        levelCount.text = currentXp.ToString() + "/" + nextlevelXp.ToString();
        levelBar.value = ((float)currentXp / (float)nextlevelXp);
    }

    public void GameLost()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
