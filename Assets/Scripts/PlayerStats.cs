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
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float attack = 20f;
    [SerializeField] private float defense = 5f;
    [SerializeField] private int level = 1;
    [SerializeField] private float currentXP = 0f;
    [SerializeField] private float xpToNextLevel = 100f;
    [SerializeField] private int money = 0;

    private float currentHealth;

    [Header("Referências de UI (opcional)")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text levelText;
    [SerializeField] private Text xpText;
    [SerializeField] private Text moneyText;

    public event Action OnDeath; // para PlayerController reagir quando morrer

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

    // === ATAQUE / DEFESA ===
    public float GetAttack()
    {
        return attack;
    }

    public float GetDefense()
    {
        return defense;
    }

    public void IncreaseAttack(float amount)
    {
        attack += amount;
    }

    public void IncreaseDefense(float amount)
    {
        defense += amount;
    }

    // === XP / LEVEL ===
    public void GainXP(float amount)
    {
        currentXP += amount;

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
            healthBar.value = currentHealth / maxHealth;

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
}
