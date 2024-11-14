using TMPro;
using UnityEngine;

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

    private int player1Index = 0;
    private int player2Index = 0;
    private Upgrade[] player1Upgrades;
    private Upgrade[] player2Upgrades;
    private GameObject[] player1Items;
    private GameObject[] player2Items;



    // Cooldown variables
    private float inputCooldown = 0.2f;  // Time in seconds between inputs
    private float player1CooldownTimer = 0;
    private float player2CooldownTimer = 0;

    // Track if each player has selected an upgrade
    private bool player1Selected = false;
    private bool player2Selected = false;

    // Track if the shop is open
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
        player1Upgrades = GetRandomUpgrades(3);
        player2Upgrades = GetRandomUpgrades(3);

        player1Items = new GameObject[player1Upgrades.Length];
        for (int i = 0; i < player1Upgrades.Length; i++)
        {
            Upgrade upgrade = player1Upgrades[i];
            GameObject item1 = Instantiate(itemPrefab, player1ShopContent);
            player1Items[i] = item1;
            upgrade.itemRef = item1;

            foreach (Transform child in item1.transform)
            {
                if (child.gameObject.name == "Name")
                {
                    TextMeshProUGUI textMeshPro = child.gameObject.GetComponent<TextMeshProUGUI>();
                    textMeshPro.text = upgrade.name;
                }
            }
        }

        player2Items = new GameObject[player2Upgrades.Length];
        for (int i = 0; i < player2Upgrades.Length; i++)
        {
            Upgrade upgrade = player2Upgrades[i];
            GameObject item2 = Instantiate(itemPrefab, player2ShopContent);
            player2Items[i] = item2;
            upgrade.itemRef = item2;

            foreach (Transform child in item2.transform)
            {
                if (child.gameObject.name == "Name")
                {
                    TextMeshProUGUI textMeshPro = child.gameObject.GetComponent<TextMeshProUGUI>();
                    textMeshPro.text = upgrade.name;
                }
            }
        }

        UpdateSelectorPosition(player1Selector, player1Items, player1Index);
        UpdateSelectorPosition(player2Selector, player2Items, player2Index);
    }

    private void Update()
    {

        if (!shopOpen) return;  // Only handle input if shop is open

        if (player1Selected && player2Selected) return;  // Stop if both players have selected

        player1CooldownTimer -= Time.deltaTime;
        player2CooldownTimer -= Time.deltaTime;

        float player1Input = Input.GetAxis("Vertical1");
        if (player1CooldownTimer <= 0)
        {
            if (player1Input > 0.1f)
            {
                player1Index = (player1Index - 1 + player1Upgrades.Length) % player1Upgrades.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
            }
            else if (player1Input < -0.1f)
            {
                player1Index = (player1Index + 1) % player1Upgrades.Length;
                UpdateSelectorPosition(player1Selector, player1Items, player1Index);
                player1CooldownTimer = inputCooldown;
            }
        }

        float player2Input = Input.GetAxis("Vertical2");
        if (player2CooldownTimer <= 0)
        {
            if (player2Input > 0.1f)
            {
                player2Index = (player2Index - 1 + player2Upgrades.Length) % player2Upgrades.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
            }
            else if (player2Input < -0.1f)
            {
                player2Index = (player2Index + 1) % player2Upgrades.Length;
                UpdateSelectorPosition(player2Selector, player2Items, player2Index);
                player2CooldownTimer = inputCooldown;
            }
        }

        if (Input.GetButtonDown("Submit1") && !player1Selected)
        {
            ApplyUpgrade(player1Upgrades[player1Index], 1);
            player1Selected = true;
            player1ShopUI.SetActive(false);  // Close Player 1's shop panel
            CheckBothPlayersSelected();
        }

        if (Input.GetButtonDown("Submit2") && !player2Selected)
        {
            ApplyUpgrade(player2Upgrades[player2Index], 2);
            player2Selected = true;
            player2ShopUI.SetActive(false);  // Close Player 2's shop panel
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

            default:

                break;

            
        }

        Debug.Log("Applying upgrade: " + upgrade.name + " for Player " + playerNumber);
    }

    public void OpenShop()
    {
        upgradeScreen.SetActive(true);
        player1ShopUI.SetActive(true);
        player2ShopUI.SetActive(true);
        player1Selected = false;
        player2Selected = false;
        shopOpen = true;  // Flag the shop as open to restrict player movement
    }

    private void CheckBothPlayersSelected()
    {
        if (player1Selected && player2Selected)
        {
            upgradeScreen.SetActive(false);
            shopOpen = false;  // Reset the shop open flag to allow gameplay to continue
        }
    }

    private GameObject FindPlayer(int playerNumber)
    {
        if (playerNumber == 1)
            return player1;

        else if (playerNumber == 2)
            return player2;

        else return null;
    }
}


    [System.Serializable]
    public class Upgrade
    {
        public string name;
        [HideInInspector] public GameObject itemRef;
    }
