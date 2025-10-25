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

        if (success)
        {
            switch (item.type)
            {
                case ShopItem.ItemType.Vida:
                    float currentHealth = PlayerPrefs.GetFloat("PlayerHealth", 100f); // 100f é valor padrão caso não exista
                    currentHealth += 10f; // adiciona 20
                    PlayerPrefs.SetFloat("PlayerHealth", currentHealth);
                    PlayerPrefs.Save(); // garante que o valor seja salvo
                    updateHUD();
                    break;
                case ShopItem.ItemType.Ataque:
                    float currentMultiplier = PlayerPrefs.GetFloat("PlayerMoneyMultiplier", 1f); // 1f é valor padrão caso não exista
                    currentMultiplier += 0.2f; // adiciona 0.5
                    PlayerPrefs.SetFloat("PlayerMoneyMultiplier", currentMultiplier);
                    PlayerPrefs.Save(); // garante que o valor seja salvo
                    updateHUD();
                    break;
                case ShopItem.ItemType.Defesa:
                    float currentDefense = PlayerPrefs.GetFloat("PlayerDefense", 0f); // 0f é valor padrão caso não exista
                    currentDefense += 2f; // adiciona 5
                    PlayerPrefs.SetFloat("PlayerDefense", currentDefense);
                    PlayerPrefs.Save(); // garante que o valor seja salvo
                    updateHUD();
                    break;
            }

            Debug.Log($"Comprou {item.itemName}!");
        }
        else
        {
            Debug.Log("Moedas insuficientes!");
        }
    }
    
    private void ReturnToPreviousScene()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}