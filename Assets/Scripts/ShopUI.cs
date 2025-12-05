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
    private const float MAX_ATTACK_SPEED = 3f;

    void Awake()
    {
        vidaLojinha = FindObjectOfType<VidaLojinha>();
        multLojinha = FindObjectOfType<MultiplicadorLojinha>();
        defesaLojinha = FindObjectOfType<DefesaLojinha>();

        moveSpeedLojinha = FindObjectOfType<MoveSpeedLojinha>();
        danoLojinha = FindObjectOfType<DamageLojinha>();
        attackSpeedLojinha = FindObjectOfType<AttackSpeedLojinha>();
    }

    private void Start()
    {
        foreach (var item in items)
            item.buyButton.onClick.AddListener(() => TryBuyItem(item));

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToPreviousScene);

        updateHUD();
    }

    private void updateHUD()
    {
        vidaLojinha?.AtualizarVida(PlayerPrefs.GetFloat("PlayerHealth"));
        multLojinha?.AtualizarMult(PlayerPrefs.GetFloat("PlayerMoneyMultiplier"));
        defesaLojinha?.AtualizarDefesa(PlayerPrefs.GetFloat("PlayerDefense"));

        moveSpeedLojinha?.AtualizarMoveSpeed(PlayerPrefs.GetFloat("PlayerMoveSpeed"));
        danoLojinha?.AtualizarDano(PlayerPrefs.GetFloat("PlayerDamageBonus"));
        attackSpeedLojinha?.AtualizarAttackSpeed(PlayerPrefs.GetFloat("PlayerAttackSpeed"));
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

        bool success = GameManager.Instance.SpendCoins(item.price);

        if (!success)
        {
            Debug.Log("Moedas insuficientes!");
            return;
        }

        switch (item.type)
        {
            case ShopItem.ItemType.Vida:
                float currentHealth = PlayerPrefs.GetFloat("PlayerHealth", 100f);
                currentHealth = Mathf.Min(currentHealth + 10f, MAX_HEALTH);
                PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
                break;

            case ShopItem.ItemType.BonusDinheiro:
                float currentMult = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f);
                currentMult = Mathf.Min(currentMult + 0.2f, MAX_MONEY_MULT);
                PlayerPrefs.SetFloat("PlayerMoneyMultiplier", currentMult);
                break;

            case ShopItem.ItemType.Defesa:
                float currentDefense = PlayerPrefs.GetFloat("PlayerDefense", 0f);
                currentDefense = Mathf.Min(currentDefense + 2f, MAX_DEFENSE);
                PlayerPrefs.SetFloat("PlayerDefense", currentDefense);
                break;

            // NOVOS ITENS DA LOJA ------------------------

            case ShopItem.ItemType.MoveSpeed:
                float currentSpeed = PlayerPrefs.GetFloat("PlayerMoveSpeed", 5f);
                currentSpeed = Mathf.Min(currentSpeed + 0.3f, MAX_SPEED);
                PlayerPrefs.SetFloat("PlayerMoveSpeed", currentSpeed);
                break;

            case ShopItem.ItemType.AttackDamage:
                float bonusDamage = PlayerPrefs.GetFloat("PlayerDamageBonus", 0f);
                bonusDamage = Mathf.Min(bonusDamage + 2f, MAX_DAMAGE);
                PlayerPrefs.SetFloat("PlayerDamageBonus", bonusDamage);
                break;

            case ShopItem.ItemType.AttackSpeed:
                float atkSpeed = PlayerPrefs.GetFloat("PlayerAttackSpeed", 1f);
                atkSpeed = Mathf.Min(atkSpeed + 0.1f, MAX_ATTACK_SPEED);
                PlayerPrefs.SetFloat("PlayerAttackSpeed", atkSpeed);
                break;
        }

        PlayerPrefs.Save();
        updateHUD();

        Debug.Log($"Comprou {item.itemName}!");
    }

    private void ReturnToPreviousScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
