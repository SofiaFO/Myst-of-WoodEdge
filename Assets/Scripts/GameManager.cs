using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TMP_Text coinText; // arrasta o texto da UI aqui no Inspector

    private int coins = 0;

    private void Awake()
    {
        // Garante que só exista um GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadCoins();
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
}