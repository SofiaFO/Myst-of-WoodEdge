using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopItem[] items;
    [SerializeField] private Button backButton;

    private VidaLojinha vidaLojinha;
    private MultiplicadorLojinha multLojinha;
    private DefesaLojinha defesaLojinha;
    private MoveSpeedLojinha moveSpeedLojinha;
    private DamageLojinha danoLojinha;
    private AttackSpeedLojinha attackSpeedLojinha;

    private const float MAX_HEALTH = 200f;
    private const float MAX_MONEY_MULT = 3f;
    private const float MAX_DEFENSE = 50f;
    private const float MAX_SPEED = 10f;
    private const float MAX_DAMAGE = 50f;
    private const float MAX_ATTACK_SPEED = 0.30f;

    void Awake()
    {
        vidaLojinha = FindObjectOfType<VidaLojinha>();
        multLojinha = FindObjectOfType<MultiplicadorLojinha>();
        defesaLojinha = FindObjectOfType<DefesaLojinha>();
        moveSpeedLojinha = FindObjectOfType<MoveSpeedLojinha>();
        danoLojinha = FindObjectOfType<DamageLojinha>();
        attackSpeedLojinha = FindObjectOfType<AttackSpeedLojinha>();

        // Debug para verificar
        Debug.Log($"[ShopUI Awake] multLojinha: {(multLojinha != null ? "OK" : "NULL")}");
    }

    private void Start()
    {
        // Buscar novamente se estiver null
        if (multLojinha == null)
        {
            Debug.LogWarning("[ShopUI] multLojinha estava NULL, buscando novamente...");
            multLojinha = FindObjectOfType<MultiplicadorLojinha>();
        }

        foreach (var item in items)
            item.buyButton.onClick.AddListener(() => TryBuyItem(item));

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToPreviousScene);

        updateHUD();
        UpdateButtonStates();
    }

    private void updateHUD()
    {
        // Vida
        float currentHealth = PlayerPrefs.GetFloat("PlayerHealth", 100f);
        bool healthMax = currentHealth >= MAX_HEALTH;
        vidaLojinha?.AtualizarVida(currentHealth, healthMax);

        // Multiplicador
        float currentMult = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f);
        bool multMax = currentMult >= MAX_MONEY_MULT;
        multLojinha?.AtualizarMult(currentMult, multMax);

        // Defesa
        float currentDefense = PlayerPrefs.GetFloat("PlayerDefense", 5f);
        bool defenseMax = currentDefense >= MAX_DEFENSE;
        defesaLojinha?.AtualizarDefesa(currentDefense, defenseMax);

        // Velocidade
        float currentSpeed = PlayerPrefs.GetFloat("PlayerMoveSpeedBonus", 0f);
        bool speedMax = currentSpeed >= MAX_SPEED;
        moveSpeedLojinha?.AtualizarMoveSpeed(currentSpeed, speedMax);

        // Dano
        float currentDamage = PlayerPrefs.GetFloat("PlayerDamageBonus", 0f);
        bool damageMax = currentDamage >= MAX_DAMAGE;
        danoLojinha?.AtualizarDano(currentDamage, damageMax);

        // Attack Speed
        float currentAtkSpeed = PlayerPrefs.GetFloat("PlayerAttackSpeedBonus", 0f);
        bool atkSpeedMax = currentAtkSpeed >= MAX_ATTACK_SPEED;
        attackSpeedLojinha?.AtualizarAttackSpeed(currentAtkSpeed, atkSpeedMax);
    }

    private void UpdateButtonStates()
    {
        foreach (var item in items)
        {
            bool isMaxed = IsItemMaxed(item.type);

            if (item.buyButton != null)
            {
                item.buyButton.interactable = !isMaxed;

                // Opcional: mudar a cor do botão quando está no máximo
                var colors = item.buyButton.colors;
                colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                item.buyButton.colors = colors;
            }
        }
    }

    private bool IsItemMaxed(ShopItem.ItemType type)
    {
        switch (type)
        {
            case ShopItem.ItemType.Vida:
                return PlayerPrefs.GetFloat("PlayerHealth", 100f) >= MAX_HEALTH;

            case ShopItem.ItemType.BonusDinheiro:
                return PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f) >= MAX_MONEY_MULT;

            case ShopItem.ItemType.Defesa:
                return PlayerPrefs.GetFloat("PlayerDefense", 5f) >= MAX_DEFENSE;

            case ShopItem.ItemType.MoveSpeed:
                return PlayerPrefs.GetFloat("PlayerMoveSpeedBonus", 0f) >= MAX_SPEED;

            case ShopItem.ItemType.AttackDamage:
                return PlayerPrefs.GetFloat("PlayerDamageBonus", 0f) >= MAX_DAMAGE;

            case ShopItem.ItemType.AttackSpeed:
                return PlayerPrefs.GetFloat("PlayerAttackSpeedBonus", 0f) >= MAX_ATTACK_SPEED;

            default:
                return false;
        }
    }

    private void TryBuyItem(ShopItem item)
    {
        if (item == null)
        {
            Debug.LogError("❌ item está nulo!");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("❌ GameManager.Instance está nulo!");
            return;
        }

        // Verificar se já está no máximo
        if (IsItemMaxed(item.type))
        {
            Debug.Log($"⚠️ {item.itemName} já está no MÁXIMO!");
            return;
        }

        bool success = GameManager.Instance.SpendCoins(item.price);

        if (!success)
        {
            Debug.Log("💸 Moedas insuficientes!");
            return;
        }

        // ===== APLICAR O UPGRADE =====
        switch (item.type)
        {
            case ShopItem.ItemType.Vida:
                float currentHealth = PlayerPrefs.GetFloat("PlayerHealth", 100f);
                currentHealth = Mathf.Min(currentHealth + 10f, MAX_HEALTH);
                PlayerPrefs.SetFloat("PlayerHealth", currentHealth);

                if (GameManager.Instance.playerStats != null)
                    GameManager.Instance.playerStats.SetMaxHealth(currentHealth);
                break;

            case ShopItem.ItemType.BonusDinheiro:
                float currentMult = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f);
                currentMult = Mathf.Min(currentMult + 0.2f, MAX_MONEY_MULT);
                PlayerPrefs.SetFloat("PlayerMoneyMultiplier", currentMult);

                if (GameManager.Instance.playerStats != null)
                    GameManager.Instance.playerStats.SetMoneyMultiplier(currentMult);
                break;

            case ShopItem.ItemType.Defesa:
                float currentDefense = PlayerPrefs.GetFloat("PlayerDefesa", 5f);
                currentDefense = Mathf.Min(currentDefense + 2f, MAX_DEFENSE);
                PlayerPrefs.SetFloat("PlayerDefense", currentDefense);

                if (GameManager.Instance.playerStats != null)
                    GameManager.Instance.playerStats.SetDefense(currentDefense);
                break;

            case ShopItem.ItemType.MoveSpeed:
                float moveBonus = PlayerPrefs.GetFloat("PlayerMoveSpeedBonus", 0f);
                moveBonus = Mathf.Min(moveBonus + 0.3f, MAX_SPEED);
                PlayerPrefs.SetFloat("PlayerMoveSpeedBonus", moveBonus);
                break;

            case ShopItem.ItemType.AttackDamage:
                float bonusDamage = PlayerPrefs.GetFloat("PlayerDamageBonus", 0f);
                bonusDamage = Mathf.Min(bonusDamage + 2f, MAX_DAMAGE);
                PlayerPrefs.SetFloat("PlayerDamageBonus", bonusDamage);
                break;

            case ShopItem.ItemType.AttackSpeed:
                float atkSpeedBonus = PlayerPrefs.GetFloat("PlayerAttackSpeedBonus", 0f);
                atkSpeedBonus = Mathf.Min(atkSpeedBonus + 0.05f, MAX_ATTACK_SPEED);
                PlayerPrefs.SetFloat("PlayerAttackSpeedBonus", atkSpeedBonus);
                break;
        }

        PlayerPrefs.Save();
        updateHUD();
        UpdateButtonStates(); // Atualizar botões após compra
        Debug.Log($"✅ Comprou {item.itemName}!");
    }

    private void ReturnToPreviousScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}