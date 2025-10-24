using UnityEngine;
using UnityEngine.UI;

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

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopItem[] items;
    [SerializeField] private PlayerStats playerStats;

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

            Debug.Log($"🛍️ Comprou {item.itemName}!");
        }
        else
        {
            Debug.Log("❌ Coins insuficientes!");
        }
    }
}
