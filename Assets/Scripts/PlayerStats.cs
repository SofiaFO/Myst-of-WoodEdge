using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla os status principais do jogador:
/// Vida, Ataque, Defesa, XP, Nível e Dinheiro.
/// Também atualiza a UI (barras e textos) quando necessário.
/// </summary>
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
    private XpBar _xpBar;
    ItemRandomScript1 itemRandom1;
    ItemRandomScript2 itemRandom2;
    ItemRandomScript3 itemRandom3;
    GameObject Card1;
    GameObject Card2;
    GameObject Card3;
    GameObject CardUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float currentHealth;

    [Header("Referências de UI (opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text moneyText;

    public event Action OnDeath; // para PlayerController reagir quando morrer

    public static PlayerStats Instance;

    private void Awake()
    {
        Debug.Log("🟦 [PlayerStats] Awake iniciado.");

        CardUI = GameObject.FindWithTag("CardUI");
        Card1 = GameObject.FindWithTag("Card1");
        Card2 = GameObject.FindWithTag("Card2");
        Card3 = GameObject.FindWithTag("Card3");

        Debug.Log($"🟩 CardUI encontrado? {CardUI != null}");
        Debug.Log($"🟩 Card1 encontrado? {Card1 != null}");
        Debug.Log($"🟩 Card2 encontrado? {Card2 != null}");
        Debug.Log($"🟩 Card3 encontrado? {Card3 != null}");

        if (CardUI != null)
            CardUI.SetActive(false);

        // Singleton
        PlayerStats[] players = FindObjectsOfType<PlayerStats>();
        if (players.Length > 1)
            Debug.LogError("❌ Mais de um PlayerStats encontrado na cena!");
        // Singleton seguro
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _xpBar = FindObjectOfType<XpBar>();

        // Health
        if (!PlayerPrefs.HasKey("PlayerHealth"))
            PlayerPrefs.SetFloat("PlayerHealth", 100f);
        maxHealth = PlayerPrefs.GetFloat("PlayerHealth");

        // Defense
        if (!PlayerPrefs.HasKey("PlayerDefense"))
            PlayerPrefs.SetFloat("PlayerDefense", 5f);
        defense = PlayerPrefs.GetFloat("PlayerDefense");

        // Money Multiplier
        if (!PlayerPrefs.HasKey("PlayerMoneyMultiplier"))
            PlayerPrefs.SetFloat("PlayerMoneyMultiplier", 1f);
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
        // Fórmula melhorada: defesa tem retorno decrescente, evita redução excessiva
        float damageReduction = defense / (defense + 50f); // máximo de ~50% de redução com defesa muito alta
        float realDamage = damage * (1 - damageReduction);
        currentHealth -= Mathf.Max(1f, realDamage); // sempre causa pelo menos 1 de dano
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

    // === ATAQUE / DEFESA ===
    public float GetAttack()
    {
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public float GetMoneyMultiplier()
    {
        return moneyMultiplier;
    }

    public float GetHealth()
    {
        return maxHealth;
    }

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
        xpToNextLevel *= 1.15f;
        _xpBar.levelUp(xpToNextLevel);

        maxHealth += 15f;
        attack += 3f;
        defense += 1.5f;
        currentHealth = maxHealth;

        // Ativa UI
        Debug.Log("🟨 Ativando CardUI...");
        if (CardUI == null)
            Debug.LogError("❌ CardUI está NULL no LevelUp!");
        else
            CardUI.SetActive(true);

        // Verificando Cards
        Debug.Log($"🟩 Card1 é null? {Card1 == null}");
        Debug.Log($"🟩 Card2 é null? {Card2 == null}");
        Debug.Log($"🟩 Card3 é null? {Card3 == null}");

        // GetComponent com logs
        Debug.Log("🔍 Pegando ItemRandomScript1...");
        if (Card1 == null)
            Debug.LogError("❌ Card1 está null antes do GetComponent!");

        itemRandom1 = Card1?.GetComponent<ItemRandomScript1>();
        Debug.Log($"🔎 itemRandom1 encontrado? {itemRandom1 != null}");

        Debug.Log("🔍 Pegando ItemRandomScript2...");
        if (Card2 == null)
            Debug.LogError("❌ Card2 está null antes do GetComponent!");

        itemRandom2 = Card2?.GetComponent<ItemRandomScript2>();
        Debug.Log($"🔎 itemRandom2 encontrado? {itemRandom2 != null}");

        Debug.Log("🔍 Pegando ItemRandomScript3...");
        if (Card3 == null)
            Debug.LogError("❌ Card3 está null antes do GetComponent!");

        itemRandom3 = Card3?.GetComponent<ItemRandomScript3>();
        Debug.Log($"🔎 itemRandom3 encontrado? {itemRandom3 != null}");

        // Agora tentando sortear
        Debug.Log("🎲 Tentando DrawRandomItem nos 3 cards...");

        if (itemRandom1 == null) Debug.LogError("❌ itemRandom1 está NULL!");
        else itemRandom1.DrawRandomItem();

        if (itemRandom2 == null) Debug.LogError("❌ itemRandom2 está NULL!");
        else itemRandom2.DrawRandomItem();

        if (itemRandom3 == null) Debug.LogError("❌ itemRandom3 está NULL!");
        else itemRandom3.DrawRandomItem();

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

    // getters simples para outros scripts
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float defenseValue => defense;
    public int Level => level;
    public int Money => money;
    public float xpToNextLevelValue => xpToNextLevel;
    public float CurrentXP => currentXP;
}