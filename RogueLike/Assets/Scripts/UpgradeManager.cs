using System.Collections.Generic;
using System.Linq;
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
    public GameObject healingDrone;

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

    private Dictionary<int, Dictionary<string, int>> playerUpgradeLevels;


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
        playerUpgradeLevels = new Dictionary<int, Dictionary<string, int>>()
    {
        { 1, new Dictionary<string, int>() },
        { 2, new Dictionary<string, int>() }
    };

        // Initialize all upgrades for each player with level 0
        foreach (var upgrade in upgrades)
        {
            playerUpgradeLevels[1][upgrade.name] = 0;
            playerUpgradeLevels[2][upgrade.name] = 0;
        }
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

        if (playerUpgradeLevels[playerNumber][upgrade.name] >= upgrade.maxlevel)
        {
            Debug.Log($"Player {playerNumber} has already maxed out the {upgrade.name} upgrade.");
            return;
        }

        int level = playerUpgradeLevels[playerNumber][upgrade.name];

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

                if (level == 0)
                {
                    SpawnUpgrade(shootingDrone, FindPlayer(playerNumber).transform, playerNumber);
                }
                else if(level == 1)
                {
                    IncreaseDroneDamage(playerNumber, 1.50f);
                }
                else if (level == 2)
                {
                    IncreaseFireRate(playerNumber, 1.30f);
                    IncreaseDroneDamage(playerNumber, 1.50f);

                }
                else if (level == 3)
                {
                    IncreaseFireRate(playerNumber, 1.30f);

                }
                else if (level == 4)
                {
                    IncreaseDroneDamage(playerNumber, 1.20f);

                }
                else if (level == 5)
                {
                    IncreaseFireRate(playerNumber, 1.20f);
                    IncreaseDroneDamage(playerNumber, 1.20f);
                }

                break;

            case "Healing Drone":

                if (level == 0)
                {
                    SpawnUpgrade(healingDrone, FindPlayer(playerNumber).transform, playerNumber);
                }
                else if (level == 1)
                {
                    IncreaseDroneHealingRate(playerNumber, 1.10f);
                }
                else if (level == 2)
                {
                    IncreaseDroneHealingRate(playerNumber, 1.10f);
                    IncreaseDroneHealing(playerNumber, 1.25f);

                }
                else if (level == 3)
                {
                    IncreaseDroneHealing(playerNumber, 1.25f);

                }
                else if (level == 4)
                {
                    IncreaseDroneHealing(playerNumber, 1.50f);

                }
                else if (level == 5)
                {
                    IncreaseDroneHealing(playerNumber, 1.25f);
                    IncreaseDroneHealingRate(playerNumber, 1.30f);
                }

                break;

            default:
                break;
        }

        playerUpgradeLevels[playerNumber][upgrade.name]++;
    }



    public void OpenShop()
    {
        currentUpgrades = GetRandomUpgrades(3);

        player1Selected = false;
        player2Selected = false;

        player1Items = InitializeShop(player1ShopContent, currentUpgrades, 1);
        player2Items = InitializeShop(player2ShopContent, currentUpgrades, 2);

        player1Index = 0;
        player2Index = 0;

        UpdateSelectorPosition(player1Selector, player1Items, player1Index);
        UpdateSelectorPosition(player2Selector, player2Items, player2Index);

        upgradeScreen.SetActive(true);
        player1ShopUI.SetActive(true);
        player2ShopUI.SetActive(true);
        shopOpen = true;
    }

    private GameObject[] InitializeShop(Transform shopContent, Upgrade[] upgradesToDisplay, int playerNumber)
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
                    // Update description based on player-specific upgrade level
                    textMeshPro.text = GetUpgradeDescription(upgrade, playerNumber); // Pass player number
                }
                if (child.gameObject.name == "Icon")
                {
                    child.gameObject.GetComponent<Image>().sprite = upgrade.icon;
                }
            }
        }
        return items;
    }


    private string GetUpgradeDescription(Upgrade upgrade, int playerNumber)
    {
        int upgradeLevel = playerUpgradeLevels[playerNumber][upgrade.name];

        switch (upgrade.name)
        {
            case "Damage":
                return $"Increase your damage by {20 * (upgradeLevel + 1)}%.";
            case "Fire Rate":
                return $"Increase your fire rate by {20 * (upgradeLevel + 1)}%.";
            case "Move Speed":
                return $"Increase your movement speed by {10 * (upgradeLevel + 1)}%.";
            case "Shooting Drone":
                switch (upgradeLevel)
                {
                    case 0: return "Summon a shooting drone to help in battle.";
                    case 1: return "Increase drone damage by 50%.";
                    case 2: return "Increase drone fire rate by 30% and damage by 50%.";
                    case 3: return "Increase drone fire rate by 30%.";
                    case 4: return "Increase drone damage by 20%.";
                    case 5: return "Increase drone fire rate by 20% and damage by 20%.";
                    default: return "No upgrade available.";
                }
            case "Healing Drone":
                switch (upgradeLevel)
                {
                    case 0:
                        return "Summon a healing drone to assist in battle by healing players."; // Basic drone summon
                    case 1:
                        return "Increase the drone's healing rate by 10%"; // Increase healing rate at level 1
                    case 2:
                        return "Increase the drone's healing rate by 10% and healing amount by 25%."; // Level 2, both rate and amount
                    case 3:
                        return "Increase the drone's healing rate by 10%."; // Level 3, improve healing rate further
                    case 4:
                        return "Increase the drone's healing amount by 50%."; // Level 4, boost healing amount
                    case 5:
                        return "Increase the drone's healing rate by 25% and healing amount by 30%."; // Level 5, balanced boost to both
                    default:
                        return "No upgrade available."; // Default case for invalid levels
                }
            default:
                return "No description available.";
        }
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

    private void SpawnUpgrade(GameObject itemToSpawn, Transform droneTarget, int playerNumber)
    {
        GameObject spawnable = Instantiate(itemToSpawn, droneTarget.position, Quaternion.identity);

        FindPlayer(playerNumber).GetComponent<Inventory>().items.Add(spawnable.transform);
        spawnable.GetComponent<DroneBasic>().target = droneTarget;
    }

    private T FindItemInInventory<T>(int playerNumber) where T : Component
    {
        GameObject player = FindPlayer(playerNumber);
        Inventory playerInventory = player.GetComponent<Inventory>();

        if (playerInventory != null)
        {
            foreach (var item in playerInventory.items)
            {
                T component = item.GetComponent<T>();
                if (component != null)
                {
                    return component; 
                }
            }
        }
        return null; 
    }


    //SHOOTING DRONE

    private void IncreaseDroneDamage(int playerNumber, float percentage)
    {
        ShootingDrone drone = FindItemInInventory<ShootingDrone>(playerNumber);

        if (drone != null)
        {
            drone.damageMultiplier *= percentage;
            Debug.Log($"Player {playerNumber}'s drone damage increased!");
        }
        else
        {
            Debug.Log("No drone found in inventory.");
        }
    }

    private void IncreaseFireRate(int playerNumber, float percentage)
    {
        ShootingDrone drone = FindItemInInventory<ShootingDrone>(playerNumber);

        if (drone != null)
        {
            drone.fireRateMultiplier *= percentage;
            Debug.Log($"Player {playerNumber}'s drone damage increased!");
        }
        else
        {
            Debug.Log("No drone found in inventory.");
        }
    }

    //HEALING DRONE

    private void IncreaseDroneHealing(int playerNumber, float percentage)
    {
        HealingDrone drone = FindItemInInventory<HealingDrone>(playerNumber);

        if (drone != null)
        {
            drone.healingAmount = Mathf.RoundToInt(drone.healingAmount * percentage);
            Debug.Log($"Player {playerNumber}'s drone healing increased!");
        }
        else
        {
            Debug.Log("No drone found in inventory.");
        }
    }

    private void IncreaseDroneHealingRate(int playerNumber, float percentage)
    {
        HealingDrone drone = FindItemInInventory<HealingDrone>(playerNumber);

        if (drone != null)
        {
            drone.healingInterval = (drone.healingInterval / percentage);
            Debug.Log($"Player {playerNumber}'s drone healing increased!");
        }
        else
        {
            Debug.Log("No drone found in inventory.");
        }
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

