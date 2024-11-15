using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("Upgrades")]
    [SerializeField] public Upgrade[] upgrades;

    [Header("UI References")]
    [SerializeField] public GameObject upgradeScreen;
    [SerializeField] public GameObject player1ShopUI;
    [SerializeField] public GameObject player2ShopUI;
    [SerializeField] public Transform player1ShopContent;
    [SerializeField] public Transform player2ShopContent;
    [SerializeField] public GameObject itemPrefab;
    [SerializeField] public GameObject player1Selector;
    [SerializeField] public GameObject player2Selector;

    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("Spawnables")]
    public GameObject shootingDrone;

    private int player1Index = 0;
    private int player2Index = 0;
    private Upgrade[] currentUpgrades;
    private GameObject[] player1Items;
    private GameObject[] player2Items;

    private float inputCooldown = 0.2f;
    private float player1CooldownTimer = 0;
    private float player2CooldownTimer = 0;

    private bool player1Selected = false;
    private bool player2Selected = false;

    public bool shopOpen = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentUpgrades = new Upgrade[3];
    }

    private void Update()
    {
        if (!shopOpen) return;

        if (player1Selected && player2Selected) return;

        player1CooldownTimer -= Time.deltaTime;
        player2CooldownTimer -= Time.deltaTime;

        float player1Input = Input.GetAxis("Vertical1");
        if (player1CooldownTimer <= 0)
        {
            if (player1Input > 0.1f)
            {
                player1Index = (player1Index - 1 + currentUpgrades.Length) % currentUpgrades.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
            }
            else if (player1Input < -0.1f)
            {
                player1Index = (player1Index + 1) % currentUpgrades.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
            }
        }

        float player2Input = Input.GetAxis("Vertical2");
        if (player2CooldownTimer <= 0)
        {
            if (player2Input > 0.1f)
            {
                player2Index = (player2Index - 1 + currentUpgrades.Length) % currentUpgrades.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
            }
            else if (player2Input < -0.1f)
            {
                player2Index = (player2Index + 1) % currentUpgrades.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
            }
        }

        if (Input.GetButtonDown("Submit1") && !player1Selected)
        {
            ApplyUpgrade(currentUpgrades[player1Index], 1);
            player1Selected = true;
            player1ShopUI.SetActive(false);
            CheckBothPlayersSelected();
        }

        if (Input.GetButtonDown("Submit2") && !player2Selected)
        {
            ApplyUpgrade(currentUpgrades[player2Index], 2);
            player2Selected = true;
            player2ShopUI.SetActive(false);
            CheckBothPlayersSelected();
        }
    }

    private void UpdateSelectorPosition(GameObject selector, GameObject[] items, int index)
    {
        selector.transform.position = new Vector2(items[index].transform.position.x - 150, items[index].transform.position.y);
    }

    private Upgrade[] GetRandomUpgrades(int count)
    {
        Upgrade[] randomUpgrades = new Upgrade[count];
        System.Collections.Generic.List<int> chosenIndices = new System.Collections.Generic.List<int>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, upgrades.Length);
            } while (chosenIndices.Contains(randomIndex));

            chosenIndices.Add(randomIndex);
            randomUpgrades[i] = upgrades[randomIndex];
        }

        return randomUpgrades;
    }

    public void ApplyUpgrade(Upgrade upgrade, int playerNumber)
    {
        Gun gun = FindPlayer(playerNumber).gameObject.GetComponent<Gun>();
        Movement movement = FindPlayer(playerNumber).gameObject.GetComponent<Movement>();
        switch (upgrade.name)
        {
            case "Damage":
                gun.damageMultiplier *= 1.2f;
                break;

            case "Fire Rate":
                gun.fireRateMultiplier *= 1.2f;
                break;

            case "Move Speed":
                movement.moveSpeedMultiplier *= 1.2f;
                break;
            case "Shooting Drone":
                SpawnUpgrade(shootingDrone, FindPlayer(playerNumber).gameObject.transform);
                break;

            default:
                break;
        }
    }

    public void OpenShop()
    {
        currentUpgrades = GetRandomUpgrades(3);

        player1Selected = false;
        player2Selected = false;

        player1Items = InitializeShop(player1ShopContent, currentUpgrades);
        player2Items = InitializeShop(player2ShopContent, currentUpgrades);

        player1Index = 0;
        player2Index = 0;

        UpdateSelectorPosition(player1Selector, player1Items, player1Index);
        UpdateSelectorPosition(player2Selector, player2Items, player2Index);

        upgradeScreen.SetActive(true);
        player1ShopUI.SetActive(true);
        player2ShopUI.SetActive(true);
        shopOpen = true;
    }

    private GameObject[] InitializeShop(Transform shopContent, Upgrade[] upgradesToDisplay)
    {
        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

        GameObject[] items = new GameObject[upgradesToDisplay.Length];
        for (int i = 0; i < upgradesToDisplay.Length; i++)
        {
            Upgrade upgrade = upgradesToDisplay[i];
            GameObject item = Instantiate(itemPrefab, shopContent);
            items[i] = item;

            foreach (Transform child in item.transform)
            {
                if (child.gameObject.name == "Name")
                {
                    TextMeshProUGUI textMeshPro = child.gameObject.GetComponent<TextMeshProUGUI>();
                    textMeshPro.text = upgrade.name;
                }
                if (child.gameObject.name == "Description")
                {
                    TextMeshProUGUI textMeshPro = child.gameObject.GetComponent<TextMeshProUGUI>();
                    textMeshPro.text = upgrade.description;
                }
                if (child.gameObject.name == "Icon")
                {
                    child.gameObject.GetComponent<Image>().sprite = upgrade.icon;
                }
            }
        }
        return items;
    }

    private void CheckBothPlayersSelected()
    {
        if (player1Selected && player2Selected)
        {
            upgradeScreen.SetActive(false);
            shopOpen = false;
        }
    }

    private GameObject FindPlayer(int playerNumber)
    {
        if (playerNumber == 1)
            return player1;
        else if (playerNumber == 2)
            return player2;
        return null;
    }

    private void SpawnUpgrade(GameObject itemToSpawn, Transform droneTarget)
    {
        GameObject spawnable = Instantiate(itemToSpawn, droneTarget.position, Quaternion.identity);
        spawnable.GetComponent<DroneBasic>().target = droneTarget;
    }

}

[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public int maxlevel;
    public Sprite icon;
    [HideInInspector] public GameObject itemRef;
}
