using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerStats playerStats;
    private TMP_Text coinText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[GameManager] Singleton criado");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[GameManager] Evento sceneLoaded registrado");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[GameManager] Evento sceneLoaded removido");
    }

    void Start()
    {
        RefreshPlayerStatsReference();
        RefreshCoinTextReference();
        LoadUpgrades();
        UpdateCoinUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Cena carregada: {scene.name}");

        // Atualizar referências quando a cena muda
        StartCoroutine(DelayedRefresh());
    }

    System.Collections.IEnumerator DelayedRefresh()
    {
        // Pequeno delay para garantir que tudo foi instanciado
        yield return new WaitForEndOfFrame();

        RefreshPlayerStatsReference();
        RefreshCoinTextReference();
        LoadUpgrades();
        UpdateCoinUI();
    }

    // ===== ATUALIZAR REFERÊNCIA DO PLAYERSTATS =====
    public void RefreshPlayerStatsReference()
    {
        if (PlayerStats.Instance != null)
        {
            playerStats = PlayerStats.Instance;
            Debug.Log("[GameManager] ✅ PlayerStats referência atualizada via Singleton!");
        }
        else
        {
            // Fallback: tentar encontrar na cena
            playerStats = FindObjectOfType<PlayerStats>();

            if (playerStats != null)
                Debug.Log("[GameManager] ✅ PlayerStats encontrado via FindObjectOfType");
            else
                Debug.LogWarning("[GameManager] ⚠️ PlayerStats não encontrado!");
        }
    }

    // ===== ATUALIZAR REFERÊNCIA DO TEXTO DE MOEDAS =====
    private void RefreshCoinTextReference()
    {
        GameObject coinObj = GameObject.FindWithTag("Coin");
        if (coinObj != null)
        {
            coinText = coinObj.GetComponent<TMP_Text>();
            if (coinText != null)
                Debug.Log("[GameManager] ✅ CoinText referência atualizada!");
            else
                Debug.LogWarning("[GameManager] ⚠️ Tag 'Coin' encontrada mas sem TMP_Text!");
        }
        else
        {
            Debug.LogWarning("[GameManager] ⚠️ GameObject com tag 'Coin' não encontrado!");
        }
    }

    // ===== GETTER SEGURO PARA PLAYERSTATS =====
    private PlayerStats GetPlayerStats()
    {
        if (playerStats == null)
        {
            RefreshPlayerStatsReference();
        }
        return playerStats;
    }

    // ===== SISTEMA DE MOEDAS =====
    public void AddCoins(int amount)
    {
        PlayerStats stats = GetPlayerStats();
        if (stats != null)
        {
            stats.AddMoney(amount);
            Debug.Log($"[GameManager] Adicionou {amount} moedas. Total: {stats.Money}");
        }
        else
        {
            Debug.LogError("[GameManager] ❌ PlayerStats não encontrado em AddCoins!");
        }

        UpdateCoinUI();
    }

    public bool SpendCoins(int amount)
    {
        PlayerStats stats = GetPlayerStats();
        if (stats == null)
        {
            Debug.LogError("[GameManager] ❌ PlayerStats não encontrado em SpendCoins!");
            return false;
        }

        bool success = stats.SpendMoney(amount);

        if (success)
            Debug.Log($"[GameManager] ✅ Gastou {amount} moedas. Restante: {stats.Money}");
        else
            Debug.Log($"[GameManager] ❌ Moedas insuficientes! Tentou gastar {amount}, tem {stats.Money}");

        UpdateCoinUI();
        return success;
    }

    // ===== ATUALIZAR UI =====
    private void UpdateCoinUI()
    {
        // Tentar atualizar referência do texto se estiver nula
        if (coinText == null)
        {
            RefreshCoinTextReference();
        }

        PlayerStats stats = GetPlayerStats();

        if (coinText != null && stats != null)
        {
            coinText.text = stats.Money.ToString();
        }
        else
        {
            if (coinText == null)
                Debug.LogWarning("[GameManager] ⚠️ CoinText está nulo em UpdateCoinUI");
            if (stats == null)
                Debug.LogWarning("[GameManager] ⚠️ PlayerStats está nulo em UpdateCoinUI");
        }
    }

    // ===== CARREGAR UPGRADES =====
    private void LoadUpgrades()
    {
        PlayerStats stats = GetPlayerStats();
        if (stats == null)
        {
            Debug.LogWarning("[GameManager] ⚠️ PlayerStats não encontrado em LoadUpgrades");
            return;
        }

        float health = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        stats.SetMaxHealth(health);

        float defense = PlayerPrefs.GetFloat("PlayerDefense", 5f);
        stats.SetDefense(defense);

        float moneyMult = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f);
        stats.SetMoneyMultiplier(moneyMult);

        Debug.Log($"[GameManager] ✅ Upgrades carregados - Vida: {health}, Defesa: {defense}, Mult: {moneyMult}");
    }

    // ===== MÉTODO PÚBLICO PARA FORÇAR ATUALIZAÇÃO (útil para debug) =====
    public void ForceRefreshAll()
    {
        RefreshPlayerStatsReference();
        RefreshCoinTextReference();
        LoadUpgrades();
        UpdateCoinUI();
        Debug.Log("[GameManager] 🔄 Todas as referências foram atualizadas manualmente!");
    }
}