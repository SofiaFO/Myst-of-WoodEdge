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

    private const float MAX_HEALTH = 200f;
    private const float MAX_ATTACK_MULT = 3f;
    private const float MAX_DEFENSE = 50f;
    
    void Awake()
    {
        vidaLojinha = FindObjectOfType<VidaLojinha>();
        multLojinha = FindObjectOfType<MultiplicadorLojinha>();
        defesaLojinha = FindObjectOfType<DefesaLojinha>();
    }


    private void Start()
    {
        foreach (var item in items)
        {
            item.buyButton.onClick.AddListener(() => TryBuyItem(item));
        }

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToPreviousScene);

        updateHUD();
    }

    private void updateHUD()
    {
        if (vidaLojinha != null)
            vidaLojinha.AtualizarVida(PlayerPrefs.GetFloat("PlayerHealth"));
        if (multLojinha != null)
            multLojinha.AtualizarMult(PlayerPrefs.GetFloat("PlayerMoneyMultiplier"));
        if (defesaLojinha != null)
            defesaLojinha.AtualizarDefesa(PlayerPrefs.GetFloat("PlayerDefense"));
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

    Debug.Log($"Tentando comprar: {item.itemName}");

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

            if (currentHealth >= MAX_HEALTH)
            {
                Debug.Log("🚫 Vida já está no máximo!");
                return;
            }

            currentHealth += 10f;

            if (currentHealth > MAX_HEALTH)
                currentHealth = MAX_HEALTH;

            PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
            break;

        case ShopItem.ItemType.Ataque:
            float currentMultiplier = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f);

            if (currentMultiplier >= MAX_ATTACK_MULT)
            {
                Debug.Log("🚫 Multiplicador já está no máximo!");
                return;
            }

            currentMultiplier += 0.2f;

            if (currentMultiplier > MAX_ATTACK_MULT)
                currentMultiplier = MAX_ATTACK_MULT;

            PlayerPrefs.SetFloat("PlayerMoneyMultiplier", currentMultiplier);
            break;

        case ShopItem.ItemType.Defesa:
            float currentDefense = PlayerPrefs.GetFloat("PlayerDefense", 0f);

            if (currentDefense >= MAX_DEFENSE)
            {
                Debug.Log("🚫 Defesa já está no máximo!");
                return;
            }

            currentDefense += 2f;

            if (currentDefense > MAX_DEFENSE)
                currentDefense = MAX_DEFENSE;

            PlayerPrefs.SetFloat("PlayerDefense", currentDefense);
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