using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardScript : MonoBehaviour
{
    /*
     [Header("Referências de UI")]
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;

    [Header("Objeto com atributos do player")]
    public PlayerStats playerStats;

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        [TextArea] public string description;
        public Sprite icon;

        public UpgradeType type;

        // atributos genéricos
        public float value = 1f;

        // se for um item/arma
        public GameObject unlockPrefab;
    }

    public enum UpgradeType
    {
        AddDamage,
        AddSpeed,
        AddMaxHealth,
        UnlockItem,
        AddAttackSpeed,
        AddRange
    }

    [Header("Lista de upgrades possíveis")]
    public Upgrade[] upgradePool;

    private Upgrade currentUpgrade; // upgrade que caiu para esta carta

    // ----------------------------------------------------------------------

    public void SetupRandomCard()
    {
        currentUpgrade = upgradePool[Random.Range(0, upgradePool.Length)];

        // Atualiza UI da carta
        titleText.text = currentUpgrade.name;
        descriptionText.text = currentUpgrade.description;
        iconImage.sprite = currentUpgrade.icon;
    }

    // ----------------------------------------------------------------------
    // CHAMADO AO CLICAR NO BOTÃO DA CARTA
    // ----------------------------------------------------------------------
    public void ChooseThisCard()
    {
        ApplyUpgrade(currentUpgrade);

        // esconde UI de cartas e despausa o jogo
        LevelUpUI.instance.HideCards();
    }

    // ----------------------------------------------------------------------
    // APLICA O UPGRADE
    // ----------------------------------------------------------------------
    void ApplyUpgrade(Upgrade up)
    {
        switch (up.type)
        {
            case UpgradeType.AddDamage:
                playerStats.damage += up.value;
                break;

            case UpgradeType.AddSpeed:
                playerStats.moveSpeed += up.value;
                break;

            case UpgradeType.AddMaxHealth:
                playerStats.maxHealth += up.value;
                playerStats.currentHealth += up.value;
                break;

            case UpgradeType.AddAttackSpeed:
                playerStats.attackSpeed += up.value;
                break;

            case UpgradeType.AddRange:
                playerStats.attackRange += up.value;
                break;

            case UpgradeType.UnlockItem:
                // se já tiver o item → aumenta level
                if (playerStats.HasItem(up.unlockPrefab.name))
                {
                    playerStats.LevelUpItem(up.unlockPrefab.name);
                }
                else
                {
                    playerStats.UnlockItem(up.unlockPrefab);
                }
                break;
        }
    }
    */
}

