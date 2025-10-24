using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite icon;
    public int price;
    public Button buyButton;
    public Image iconImage;
    public Text itemNameText;
    public Text priceText;
    public enum ItemType { Vida, Ataque, Defesa }
    public ItemType type;
}
=======
using UnityEngine.SceneManagement;
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopItem[] items;
    [SerializeField] private PlayerStats playerStats;
<<<<<<< HEAD

    private void Start()
    {
        // Inicializa os botões e textos
        foreach (var item in items)
        {
            item.iconImage.sprite = item.icon;
            item.itemNameText.text = item.itemName;
            item.priceText.text = item.price.ToString() + " 💰";

            item.buyButton.onClick.AddListener(() => TryBuyItem(item));
        }
    }

    void TryBuyItem(ShopItem item)
    {
=======
    [SerializeField] private Button backButton; 


    private void Start()
    {
        foreach (var item in items)
        {
            item.buyButton.onClick.AddListener(() => TryBuyItem(item));
        }

        if (backButton != null)
            backButton.onClick.AddListener(ReturnToPreviousScene);
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

        if (playerStats == null)
        {
            Debug.LogError("❌ playerStats está nulo!");
            return;
        }

        Debug.Log($"Tentando comprar: {item.itemName}");
    
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
        bool success = GameManager.Instance.SpendCoins(item.price);

        if (success)
        {
            switch (item.type)
            {
                case ShopItem.ItemType.Vida:
                    playerStats.IncreaseHealth(10);
                    break;
                case ShopItem.ItemType.Ataque:
                    playerStats.IncreaseAttack(2);
                    break;
                case ShopItem.ItemType.Defesa:
                    playerStats.IncreaseDefense(2);
                    break;
            }

<<<<<<< HEAD
            Debug.Log($"🛍️ Comprou {item.itemName}!");
        }
        else
        {
            Debug.Log("❌ Coins insuficientes!");
        }
    }
}
=======
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
>>>>>>> af165bf5ec1353f0b3db43aca4f1e26936bb0197
