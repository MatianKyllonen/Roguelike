using System.Collections;
using System.Collections.Generic;
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
    public GameObject explosiveDrone;
    public GameObject projectileShield;

    private int player1Index = 0;
    private int player2Index = 0;
    private Upgrade[] currentUpgrades1;
    private Upgrade[] currentUpgrades2;
    private GameObject[] player1Items;
    private GameObject[] player2Items;

    private float inputCooldown = 0.3f;
    private float player1CooldownTimer = 0;
    private float player2CooldownTimer = 0;

    private bool player1Selected = false;
    private bool player2Selected = false;

    public bool shopOpen = false;
    private bool canSelect;

    private AudioSource audioSource;
    public AudioClip selectSFX;
    public AudioClip navigateSFX;


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
        currentUpgrades1 = new Upgrade[3];
        currentUpgrades2 = new Upgrade[3];
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

        audioSource = GetComponent<AudioSource>();
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
                player1Index = (player1Index - 1 + currentUpgrades1.Length) % currentUpgrades1.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
                audioSource.PlayOneShot(navigateSFX, 0.5f);
            }
            else if (player1Input < -0.1f)
            {
                player1Index = (player1Index + 1) % currentUpgrades1.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
                audioSource.PlayOneShot(navigateSFX, 0.5f);
            }
        }

        float player2Input = Input.GetAxis("Vertical2");
        if (player2CooldownTimer <= 0)
        {
            if (player2Input > 0.1f)
            {
                player2Index = (player2Index - 1 + currentUpgrades2.Length) % currentUpgrades2.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
                audioSource.PlayOneShot(navigateSFX, 0.5f);
            }
            else if (player2Input < -0.1f)
            {
                player2Index = (player2Index + 1) % currentUpgrades2.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
                audioSource.PlayOneShot(navigateSFX, 0.5f);

            }
        }

        if (Input.GetButtonDown("Submit1") && !player1Selected && canSelect)
        {
            ApplyUpgrade(currentUpgrades1[player1Index], 1);
            player1Selected = true;
            player1ShopUI.GetComponent<Animator>().SetTrigger("CloseShop");
            StartCoroutine(CloseShopDelay(player1ShopUI));
            CheckBothPlayersSelected();
            audioSource.PlayOneShot(selectSFX, 0.2f);
        }

        if (Input.GetButtonDown("Submit2") && !player2Selected && canSelect)
        {
            ApplyUpgrade(currentUpgrades2[player2Index], 2);
            player2Selected = true;
            player2ShopUI.GetComponent<Animator>().SetTrigger("CloseShop");
            StartCoroutine(CloseShopDelay(player2ShopUI));
            CheckBothPlayersSelected();
            audioSource.PlayOneShot(selectSFX, 0.2f);

        }

    }

    private IEnumerator CloseShopDelay(GameObject shop)
    {
        yield return new WaitForSeconds(0.5f);
        shop.SetActive(false);
    }

    private void UpdateSelectorPosition(GameObject selector, GameObject[] items, int index)
    {
        selector.transform.position = new Vector2(items[index].transform.position.x - 150, items[index].transform.position.y);
    }

    private Upgrade[] GetRandomUpgrades(int count, int playerNumber)
    {
        // Filter upgrades that the player hasn't maxed out
        List<Upgrade> availableUpgrades = new List<Upgrade>();
        foreach (var upgrade in upgrades)
        {
            if (playerUpgradeLevels[playerNumber][upgrade.name] < upgrade.maxlevel)
            {
                availableUpgrades.Add(upgrade);
            }
        }

        // If no upgrades are available, return an empty array
        if (availableUpgrades.Count == 0)
        {
            Debug.Log($"Player {playerNumber} has no available upgrades.");
            return new Upgrade[0];
        }

        // Shuffle the available upgrades list
        for (int i = 0; i < availableUpgrades.Count; i++)
        {
            int randomIndex = Random.Range(i, availableUpgrades.Count);
            // Swap current element with the randomly chosen element
            var temp = availableUpgrades[i];
            availableUpgrades[i] = availableUpgrades[randomIndex];
            availableUpgrades[randomIndex] = temp;
        }

        // Select the first 'count' upgrades
        int selectedCount = Mathf.Min(count, availableUpgrades.Count);
        Upgrade[] randomUpgrades = new Upgrade[selectedCount];
        for (int i = 0; i < selectedCount; i++)
        {
            randomUpgrades[i] = availableUpgrades[i];
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
                gun.damageMultiplier *= 1.3f;
                break;

            case "Fire Rate":
                gun.fireRateMultiplier *= 1.2f;
                break;

            case "Move Speed":
                movement.moveSpeedMultiplier *= 1.1f;
                break;

            case "Shooting Drone":

                if (level > 0)
                    UpgradeDroneSprite(playerNumber, "shootingDrone", level);



                if (level == 0)
                {
                    // 35 Damage
                    // 0.80 Fire Rate
                    SpawnUpgrade(shootingDrone, FindPlayer(playerNumber).transform, playerNumber);
                }
                else if (level == 1)
                {
                    // 52 Damage
                    IncreaseDroneDamage(playerNumber, 1.50f);
                }
                else if (level == 2)
                {
                    // 78 Damage
                    // 1.04 Fire Rate
                    IncreaseDroneDamage(playerNumber, 1.50f);
                    IncreaseFireRate(playerNumber, 1.30f);

                }
                else if (level == 3)
                {
                    // 1.56 Fire Rate
                    IncreaseFireRate(playerNumber, 1.50f);

                }
                else if (level == 4)
                {
                    // 102 Damage
                    IncreaseDroneDamage(playerNumber, 1.30f);

                }
                else if (level == 5)
                {
                    // 122 Damage
                    // 2.34 Fire Rate
                    IncreaseDroneDamage(playerNumber, 1.20f);
                    IncreaseFireRate(playerNumber, 1.50f);
                }

                break;

            case "Healing Drone":

                if (level > 0)
                    UpgradeDroneSprite(playerNumber, "healingDrone", level);
                if (level == 0)
                {
                    // Healing 10
                    // Healing Rate 15s
                    // Speed 1
                    SpawnUpgrade(healingDrone, FindPlayer(playerNumber).transform, playerNumber);
                }
                else if (level == 1)
                {
                    // Healing Rate 13.5s
                    IncreaseDroneHealingRate(playerNumber, 1.10f);
                }
                else if (level == 2)
                {
                    // Healing 13
                    // Healing Rate 11.3s
                    IncreaseDroneHealing(playerNumber, 1.3f);
                    IncreaseDroneHealingRate(playerNumber, 1.20f);
                }
                else if (level == 3)
                {
                    // Healing 16
                    IncreaseDroneHealing(playerNumber, 1.2f);
                }
                else if (level == 4)
                {
                    // Healing 18
                    // Speed 1.2
                    IncreaseDroneHealing(playerNumber, 1.1f);
                    IncreaseHealingDroneSpeed(playerNumber, 1.2f);
                }
                else if (level == 5)
                {
                    // Healing 20
                    // Healing Rate 9.4s
                    IncreaseDroneHealing(playerNumber, 1.1f);
                    IncreaseDroneHealingRate(playerNumber, 1.30f);
                }

                break;
            case "Explosive Drone":


                if (level > 0)
                    UpgradeDroneSprite(playerNumber, "explosiveShotDrone", level);

                if (level == 0)
                {
                    // Explosion Damage 20
                    // Fire Rate 0.5
                    // Speed 0.5
                    SpawnUpgrade(explosiveDrone, FindPlayer(playerNumber).transform, playerNumber);
                }
                else if (level == 1)
                {
                    // Explosion Damage 30

                    IncreaseDroneExplosionDamage(playerNumber, 1.50f);
                }
                else if (level == 2)
                {
                    // Explosion Damage 38
                    // Fire Rate 0.6
                    IncreaseDroneExplosionDamage(playerNumber, 1.25f);
                    IncreaseExposionDroneFireRate(playerNumber, 1.2f);

                }
                else if (level == 3)
                {
                    // Explosion Damage 42
                    // Speed 0.62

                    IncreaseDroneExplosionDamage(playerNumber, 1.1f);
                    IncreaseExposionDroneSpeed(playerNumber, 1.25f);

                }
                else if (level == 4)
                {
                    // Explosion Damage 53
                    IncreaseDroneExplosionDamage(playerNumber, 1.25f);
                }
                else if (level == 5)
                {
                    // Explosion Damage 80
                    // Fire Rate 0.78
                    // Speed 0.75

                    IncreaseDroneExplosionDamage(playerNumber, 1.50f);
                    IncreaseExposionDroneFireRate(playerNumber, 1.30f);
                    IncreaseExposionDroneSpeed(playerNumber, 1.25f);
                }

                break;
            case "Projectile Shield":

                if (level == 0)
                {
                    // Explosion Damage 20
                    // Fire Rate 0.5
                    // Speed 0.5
                    SpawnUpgrade(projectileShield, FindPlayer(playerNumber).transform, playerNumber, true);
                }
                else if (level == 1)
                {
                    // Explosion Damage 30

                    IncreaseDroneExplosionDamage(playerNumber, 1.50f);
                }
                else if (level == 2)
                {
                    // Explosion Damage 38
                    // Fire Rate 0.6
                    IncreaseDroneExplosionDamage(playerNumber, 1.25f);
                    IncreaseExposionDroneFireRate(playerNumber, 1.2f);

                }
                else if (level == 3)
                {
                    // Explosion Damage 42
                    // Speed 0.62

                    IncreaseDroneExplosionDamage(playerNumber, 1.1f);
                    IncreaseExposionDroneSpeed(playerNumber, 1.25f);

                }
                else if (level == 4)
                {
                    // Explosion Damage 53
                    IncreaseDroneExplosionDamage(playerNumber, 1.25f);
                }
                else if (level == 5)
                {
                    // Explosion Damage 80
                    // Fire Rate 0.78
                    // Speed 0.75

                    IncreaseDroneExplosionDamage(playerNumber, 1.50f);
                    IncreaseExposionDroneFireRate(playerNumber, 1.30f);
                    IncreaseExposionDroneSpeed(playerNumber, 1.25f);
                }
                break;

            default:
                break;
        }

        playerUpgradeLevels[playerNumber][upgrade.name]++;
    }



    public void OpenShop()
    {
        currentUpgrades1 = GetRandomUpgrades(3, 1);
        if (currentUpgrades1.Length > 0)
        {
            player1Selected = false;
            player1Items = InitializeShop(player1ShopContent, currentUpgrades1, 1);
            player1Index = 0;
            UpdateSelectorPosition(player1Selector, player1Items, player1Index);
            player1ShopUI.SetActive(true);
        }
        else
        {
            player1ShopUI.SetActive(false);
            Debug.Log("Player 1 has no upgrades to choose from.");
        }

        currentUpgrades2 = GetRandomUpgrades(3, 2);
        if (currentUpgrades2.Length > 0)
        {
            player2Selected = false;
            player2Items = InitializeShop(player2ShopContent, currentUpgrades2, 2);
            player2Index = 0;
            UpdateSelectorPosition(player2Selector, player2Items, player2Index);
            player2ShopUI.SetActive(true);
        }
        else
        {
            player2ShopUI.SetActive(false);
            Debug.Log("Player 2 has no upgrades to choose from.");
        }

        upgradeScreen.SetActive(player1ShopUI.activeSelf || player2ShopUI.activeSelf);
        shopOpen = player1ShopUI.activeSelf || player2ShopUI.activeSelf;

        if (shopOpen)
        {
            StartCoroutine(OpenDelay());
            canSelect = false;
        }
    }



    private IEnumerator OpenDelay()
    {
        player1ShopUI.GetComponent<Animator>().SetTrigger("OpenShop");
        player2ShopUI.GetComponent<Animator>().SetTrigger("OpenShop");
        yield return new WaitForSeconds(0.5f);
        canSelect = true;
    }


    private GameObject[] InitializeShop(Transform shopContent, Upgrade[] upgradesToDisplay, int playerNumber)
    {
        // Clear previous items
        foreach (Transform child in shopContent)
        {
            Destroy(child.gameObject);
        }

        // If no upgrades to display, return an empty array
        if (upgradesToDisplay.Length == 0)
        {
            return new GameObject[0];
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
                return $"Level {upgradeLevel + 1}: Damage + {30 * (upgradeLevel + 1)}%";

            case "Fire Rate":
                return $"Level {upgradeLevel + 1}: Fire Rate + {20 * (upgradeLevel + 1)}%";

            case "Move Speed":
                return $"Level {upgradeLevel + 1}: Move Speed + {10 * (upgradeLevel + 1)}%";

            case "Shooting Drone":
                switch (upgradeLevel)
                {
                    case 0: return "Unlock:  Summon a shooting drone";
                    case 1: return "Level 1: Damage + 50%";
                    case 2: return "Level 2: Damage + 50%, Fire Rate + 30%";
                    case 3: return "Level 3: Fire Rate + 50%";
                    case 4: return "Level 4: Damage + 30%";
                    case 5: return "Level 5: Damage + 20%, Fire Rate + 50%";
                    default: return "No upgrade available";
                }

            case "Healing Drone":
                switch (upgradeLevel)
                {
                    case 0: return "Unlock:  Summon a healing drone";
                    case 1: return "Level 1: Healing Rate + 10%";
                    case 2: return "Level 2: Healing + 30%, Healing Rate + 20%";
                    case 3: return "Level 3: Healing + 20%";
                    case 4: return "Level 4: Healing + 10, Move Speed + 20%";
                    case 5: return "Level 5: Healing Rate + 25%, Healing Amount + 30%";
                    default: return "No upgrade available.";
                }

            case "Explosive Drone":
                switch (upgradeLevel)
                {
                    case 0: return "Unlock:  Summon an explosive drone";
                    case 1: return "Level 1: Explosion Damage + 50%";
                    case 2: return "Level 2: Explosion Damage + 25%, Fire Rate + 20%";
                    case 3: return "Level 3: Explosion Damage + 10%, Speed + 25%";
                    case 4: return "Level 4: Explosion Damage + 25%";
                    case 5: return "Level 5: Explosion Damage + 50%, Fire Rate + 30%, Speed + 25%";
                    default: return "No upgrade available.";
                }

            default:
                return "No description available.";
        }

    }


    private void UpgradeDroneSprite(int playerNumber, string type, int level)
    {
        GameObject drone = null;

        switch (type)
        {
            case "shootingDrone":
                drone = FindItemInInventory<ShootingDrone>(playerNumber)?.gameObject;
                break;
            case "healingDrone":
                drone = FindItemInInventory<HealingDrone>(playerNumber)?.gameObject;
                break;
            case "explosiveShotDrone":
                drone = FindItemInInventory<ExplosiveShotDrone>(playerNumber)?.gameObject;
                break;
            default:
                Debug.LogError($"Invalid drone type: {type}");
                return;
        }


        if (drone == null)
        {
            Debug.LogError($"Drone of type {type} not found for player {playerNumber}");
            return;
        }

        Transform root = drone.transform.Find("Root");
        if (root == null)
        {
            Debug.LogError($"Root not found in drone {drone.name}");
            return;
        }

        for (int i = 0; i <= 5; i++)
        {
            Transform levelTransform = root.Find($"Lvl{i}");
            if (levelTransform != null)
            {
                levelTransform.gameObject.SetActive(i == level);
                if (i == level)
                    drone.gameObject.GetComponent<DroneBasic>().spriteRenderer = levelTransform.gameObject.GetComponent<SpriteRenderer>();
            }
            else
            {
                Debug.LogWarning($"Lvl{i} not found in {root.name}");
            }
        }
    }



    private void CheckBothPlayersSelected()
    {
        if (player1Selected && player2Selected)
        {
            StartCoroutine(CloseDelay());
        }
    }

    private IEnumerator CloseDelay()
    {

        yield return new WaitForSeconds(0.3f);
        upgradeScreen.SetActive(false);
        shopOpen = false;
    }

    private GameObject FindPlayer(int playerNumber)
    {
        if (playerNumber == 1)
            return player1;
        else if (playerNumber == 2)
            return player2;
        return null;
    }

    private void SpawnUpgrade(GameObject itemToSpawn, Transform droneTarget, int playerNumber, bool attached = false)
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
            drone.damage = Mathf.RoundToInt(drone.damage * percentage);
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
            drone.fireRate = (drone.fireRate * percentage);
        }

    }

    //HEALING DRONE

    private void IncreaseDroneHealing(int playerNumber, float percentage)
    {
        HealingDrone drone = FindItemInInventory<HealingDrone>(playerNumber);

        if (drone != null)
        {
            drone.healingAmount = Mathf.RoundToInt(drone.healingAmount * percentage);
        }
    }
    private void IncreaseDroneHealingRate(int playerNumber, float percentage)
    {
        HealingDrone drone = FindItemInInventory<HealingDrone>(playerNumber);

        if (drone != null)
        {
            drone.healingInterval = (drone.healingInterval / percentage);
        }

    }

    private void IncreaseHealingDroneSpeed(int playerNumber, float percentage)
    {
        HealingDrone drone = FindItemInInventory<HealingDrone>(playerNumber);

        if (drone != null)
        {
            DroneBasic basicDrone = drone.gameObject.GetComponent<DroneBasic>();
            basicDrone.followSpeed = basicDrone.followSpeed * percentage;
        }

    }

    //Explosive Drone

    private void IncreaseDroneExplosionDamage(int playerNumber, float percentage)
    {
        ExplosiveShotDrone drone = FindItemInInventory<ExplosiveShotDrone>(playerNumber);

        if (drone != null)
        {
            drone.explosionDamage = Mathf.RoundToInt(drone.explosionDamage * percentage);

        }

    }
    private void IncreaseExposionDroneFireRate(int playerNumber, float percentage)
    {
        ExplosiveShotDrone drone = FindItemInInventory<ExplosiveShotDrone>(playerNumber);

        if (drone != null)
        {
            drone.fireRate = (drone.fireRate * percentage);
        }

    }

    private void IncreaseExposionDroneSpeed(int playerNumber, float percentage)
    {
        ExplosiveShotDrone drone = FindItemInInventory<ExplosiveShotDrone>(playerNumber);

        if (drone != null)
        {
            DroneBasic basicDrone = drone.gameObject.GetComponent<DroneBasic>();
            basicDrone.followSpeed = basicDrone.followSpeed * percentage;
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

