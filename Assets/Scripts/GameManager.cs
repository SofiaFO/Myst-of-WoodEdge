using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerStats playerStats;

    private TMP_Text coinText; // arrasta o texto da UI aqui no Inspector

    private int coins = 0;

    private float maxHealth;
    private float defense;
    private float moneyMultiplier;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
                Debug.LogError(" Nenhum PlayerStats encontrado na cena!");
        }

        coinText = GameObject.FindWithTag("Coin")?.GetComponent<TMP_Text>();
        coinText.text = coins.ToString();

    }

    void Start()
    {
        LoadCoins();
        LoadHealth();
        LoadDefense();
        LoadMoneyMultiplier();
        UpdateCoinUI();
    }

    // Adiciona coins e salva
    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        UpdateCoinUI();
    }

    // Gasta coins (usado na loja)
    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            UpdateCoinUI();
            return true;
        }
        else
        {
            Debug.Log("💸 Não há coins suficientes!");
            return false;
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
    }

    private void LoadCoins()
    {
        coins = PlayerPrefs.GetInt("Coins", 0);
    }

    private void LoadHealth()
    {
        if (!PlayerPrefs.HasKey("PlayerHealth"))
            PlayerPrefs.SetFloat("PlayerHealth", 100f);

        maxHealth = PlayerPrefs.GetFloat("PlayerHealth");
    }

    private void LoadDefense()
    {
        if (!PlayerPrefs.HasKey("PlayerDefense"))
            PlayerPrefs.SetFloat("PlayerDefense", 5f);

        defense = PlayerPrefs.GetFloat("PlayerDefense");
        if (playerStats != null)
            playerStats.SetDefense(defense);
    }

    private void LoadMoneyMultiplier()
    {
        if (!PlayerPrefs.HasKey("PlayerMoneyMultiplier"))
            PlayerPrefs.SetFloat("PlayerMoneyMultiplier", 1f);

        moneyMultiplier = PlayerPrefs.GetFloat("PlayerMoneyMultiplier");
        if (playerStats != null)
            playerStats.SetMoneyMultiplier(moneyMultiplier);
    }
}
