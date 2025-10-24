using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ShopItem[] items;
    [SerializeField] private PlayerStats playerStats;
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