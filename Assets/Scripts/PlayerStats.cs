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
    [SerializeField] private float xpToNextLevel = 100f;
    [SerializeField] private int money = 0;
    [SerializeField] private float moneyMultiplier;
    private XpBar _xpBar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private float currentHealth;

    [Header("Referências de UI (opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text moneyText;

    public event Action OnDeath; // para PlayerController reagir quando morrer

    private void Awake()
    {
        PlayerStats[] players = FindObjectsOfType<PlayerStats>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        _xpBar = FindObjectOfType<XpBar>();

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
        PlayerPrefs.Save(); // garante que qualquer valor padrão criado seja persistido
    }


    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // === VIDA ===
    public void TakeDamage(float damage)
    {
        float realDamage = damage - (defense * damage /100); // reduz dano pela defesa
        currentHealth -= realDamage;
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
        level++;
        xpToNextLevel *= 1.25f; // aumenta XP necessária a cada nível
        _xpBar.levelUp(xpToNextLevel);
        maxHealth += 10f;
        attack += 2f;
        defense += 1f;
        currentHealth = maxHealth;

        Debug.Log($"Subiu para o nível {level}!");
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