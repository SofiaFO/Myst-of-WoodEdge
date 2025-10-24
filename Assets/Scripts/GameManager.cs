using UnityEngine;
<<<<<<< HEAD
using UnityEngine.UI;
=======
using TMPro;
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
<<<<<<< HEAD

    [SerializeField] private Text coinText; // arrasta o texto da UI aqui no Inspector
=======
    public PlayerStats playerStats;

    [SerializeField] private TMP_Text coinText; // arrasta o texto da UI aqui no Inspector
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197

    private int coins = 0;

    private void Awake()
    {
<<<<<<< HEAD
        // Garante que só exista um GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre cenas
        }
        else
        {
            Destroy(gameObject);
=======
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
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
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
<<<<<<< HEAD
}
=======
}
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
