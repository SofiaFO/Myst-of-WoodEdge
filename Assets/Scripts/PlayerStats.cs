using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Status Base")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float attack = 20f;
    [SerializeField] private float defense;
    [SerializeField] private int level = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 10f;
    [SerializeField] private int money = 0;
    [SerializeField] private float moneyMultiplier;

    [Header("Progressão de XP")]
    [Tooltip("Multiplicador de XP a cada nível (1.20 = 20% a mais, 1.30 = 30% a mais)")]
    [SerializeField] private float xpScalingPerLevel = 1.3f; // 🔥 Aumentado de 1.15 para 1.25

    private XpBar _xpBar;
    ItemRandomScript1 itemRandom1;
    ItemRandomScript2 itemRandom2;
    ItemRandomScript3 itemRandom3;

    GameObject Card1;
    GameObject Card2;
    GameObject Card3;
    GameObject CardUI;

    private float currentHealth;

    [Header("Referências de UI (opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text moneyText;

    public event Action OnDeath;

    public static PlayerStats Instance;

    private void Awake()
    {
        Debug.Log("🟦 [PlayerStats] Awake iniciado.");

        CardUI = GameObject.FindWithTag("CardUI");
        Card1 = GameObject.FindWithTag("Card1");
        Card2 = GameObject.FindWithTag("Card2");
        Card3 = GameObject.FindWithTag("Card3");

        if (CardUI != null)
            CardUI.SetActive(false);

        // Singleton
        PlayerStats[] players = FindObjectsOfType<PlayerStats>();
        if (players.Length > 1)
            Debug.LogError("❌ Mais de um PlayerStats encontrado na cena!");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _xpBar = FindObjectOfType<XpBar>();

        // PlayerPrefs load
        if (!PlayerPrefs.HasKey("PlayerHealth")) PlayerPrefs.SetFloat("PlayerHealth", 100f);
        maxHealth = PlayerPrefs.GetFloat("PlayerHealth");

        if (!PlayerPrefs.HasKey("PlayerDefense")) PlayerPrefs.SetFloat("PlayerDefense", 5f);
        defense = PlayerPrefs.GetFloat("PlayerDefense");

        if (!PlayerPrefs.HasKey("PlayerMoneyMultiplier")) PlayerPrefs.SetFloat("PlayerMoneyMultiplier", 1f);
        moneyMultiplier = PlayerPrefs.GetFloat("PlayerMoneyMultiplier");

        PlayerPrefs.Save();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // === VIDA ===
    public void TakeDamage(float damage)
    {
        float damageReduction = defense / (defense + 50f);
        float realDamage = damage * (1 - damageReduction);
        currentHealth -= Mathf.Max(1f, realDamage);
        UpdateUI();
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateUI();
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void SetDefense(float amount)
    {
        defense = amount;
    }

    public void SetMoneyMultiplier(float amount)
    {
        moneyMultiplier = amount;
    }

    public float GetAttack() => attack;
    public float GetDefense() => defense;
    public float GetMoneyMultiplier() => moneyMultiplier;
    public float GetHealth() => maxHealth;

    public void IncreaseHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
    }

    public void IncreaseAttack(float amount)
    {
        attack += amount;
    }

    public void IncreaseDefense(float amount)
    {
        defense += amount;
    }

    public void IncreaseMoneyMultiplier(float amount)
    {
        moneyMultiplier += amount;
        Debug.Log($"Novo multiplicador de dinheiro: {moneyMultiplier}");
    }

    public void AddMoneyFromKill(int baseAmount)
    {
        int total = Mathf.RoundToInt(baseAmount * moneyMultiplier);
        AddMoney(total);
    }

    // === XP / LEVEL ===
    public void GainXP(float amount)
    {
        currentXP += amount;
        _xpBar.increaseXP(amount);

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        Debug.Log("🟦 [PlayerStats] Entrou em LevelUp()");

        level++;

        // 🔥 XP aumenta exponencialmente a cada nível
        xpToNextLevel *= xpScalingPerLevel;
        
        Debug.Log($"📊 Nível {level} | XP necessário: {xpToNextLevel:F0} (x{xpScalingPerLevel})");
        
        _xpBar.levelUp(xpToNextLevel);

        maxHealth += 15f;
        attack += 3f;
        defense += 1.5f;
        currentHealth = maxHealth;

        // Mostra cartas
        if (CardUI != null)
            CardUI.SetActive(true);
        else
            Debug.LogError("❌ CardUI está NULL no LevelUp!");

        // 🛑 PAUSA O JOGO AQUI
        Time.timeScale = 0f;

        // Pega scripts dos cards
        itemRandom1 = Card1?.GetComponent<ItemRandomScript1>();
        itemRandom2 = Card2?.GetComponent<ItemRandomScript2>();
        itemRandom3 = Card3?.GetComponent<ItemRandomScript3>();

        itemRandom1?.DrawRandomItem();
        itemRandom2?.DrawRandomItem();
        itemRandom3?.DrawRandomItem();

        Debug.Log($"🎉 Subiu para o nível {level}!");
    }

    // === DINHEIRO ===
    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    // === UI ===
    private void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (levelText != null)
            levelText.text = $"Lv. {level}";

        if (xpText != null)
            xpText.text = $"{currentXP:0}/{xpToNextLevel:0} XP";

        if (moneyText != null)
            moneyText.text = $"$ {money}";
    }

    // === FUNÇÃO PARA DESPAUSAR APÓS ESCOLHER UMA CARTA ===
    public void EscolheuCarta()
    {
        if (CardUI != null)
            CardUI.SetActive(false);

        Time.timeScale = 1f;
    }

    // getters
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float defenseValue => defense;
    public int Level => level;
    public int Money => money;
    public float xpToNextLevelValue => xpToNextLevel;
    public float CurrentXP => currentXP;
}