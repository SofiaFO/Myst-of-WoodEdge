using UnityEngine.UI;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public string description;
    public int price;
    public ItemType type;
    public Button buyButton;     
    public enum ItemType { Vida, Ataque, Defesa, BonusDinheiro }
}