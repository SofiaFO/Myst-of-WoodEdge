using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRandomScript3 : MonoBehaviour
{
    [Header("Referências do UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    PlayerStats playerStats;
    PlayerController playerController;

    [Header("Banco de Itens")]
    [SerializeField]
    private List<string> itemTitles = new List<string>()
    {
        "Poção de Cura",
        "Armadura Medieval",
        "Chapéu mágico",
    };

    [SerializeField]
    private List<string> itemDescriptions = new List<string>()
    {
        "Frasco curativo que cura metade da vida do usuário",
        "Lendária armadura, confere ao usuário mais resistência contra todas as fontes",
        "Cobiçado por todos os feiticeiros, gera mais um projétil de ataque e dano base",
    };

    [SerializeField] private List<Sprite> itemSprites = new List<Sprite>();

    private int lastIndex = -1;

    private GameObject CardUI;

    void Awake()
    {
        CardUI = GameObject.FindWithTag("CardUI");
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerStats = player.GetComponent<PlayerStats>();
        }
        playerController = player.GetComponent<PlayerController>();
    }

    // -----------------------------------------------------------
    // SORTEAR ITEM E APLICAR NO UI
    // -----------------------------------------------------------
    public void DrawRandomItem()
    {
        if (itemTitles.Count == 0 ||
            itemDescriptions.Count == 0 ||
            itemSprites.Count == 0 ||
            itemTitles.Count != itemDescriptions.Count ||
            itemTitles.Count != itemSprites.Count)
        {
            Debug.LogError("As listas de itens estão vazias ou com tamanhos diferentes.");
            return;
        }

        int index = Random.Range(0, itemTitles.Count);
        lastIndex = index;

        string title = itemTitles[lastIndex];
        
        // Apenas o Chapéu mágico tem sistema de upgrade
        bool alreadyOwned = (title == "Chapéu mágico") && PlayerPrefs.GetInt($"ITEM_{title}", 0) == 1;

        titleText.text = alreadyOwned ? title + " (UPGRADE)" : title;
        descriptionText.text = alreadyOwned
            ? GetUpgradeDescription(title)
            : itemDescriptions[lastIndex];

        iconImage.sprite = itemSprites[lastIndex];
    }


    // -----------------------------------------------------------
    // ATIVAR OBJETO RELACIONADO AO ITEM
    // -----------------------------------------------------------
    public void ApplyItemEffect()
    {
        if (lastIndex < 0)
        {
            Debug.LogWarning("Nenhum item foi sorteado ainda.");
            return;
        }

        string itemName = itemTitles[lastIndex];
        
        // Apenas o Chapéu mágico tem sistema de upgrade
        bool alreadyOwned = (itemName == "Chapéu mágico") && PlayerPrefs.GetInt($"ITEM_{itemName}", 0) == 1;

        // Se é o Chapéu e já tem → chamar upgrade
        if (alreadyOwned)
        {
            ApplyUpgrade(itemName);
            CloseCardUI();
            return;
        }

        // Se é o Chapéu e ainda não tem → marcar como obtido
        if (itemName == "Chapéu mágico")
        {
            PlayerPrefs.SetInt($"ITEM_{itemName}", 1);
            PlayerPrefs.Save();
        }

        // Aplicar efeito do item (primeira vez ou sempre para Poção/Armadura)
        ApplyItemEffect_Internal(itemName);
        
        CloseCardUI();
    }

    // -----------------------------------------------------------
    // APLICAR EFEITO DO ITEM (primeira vez ou sempre)
    // -----------------------------------------------------------
    private void ApplyItemEffect_Internal(string itemName)
    {
        switch (itemName)
        {
            case "Poção de Cura":
                playerStats.Heal(playerStats.GetHealth() / 2);
                break;
            case "Armadura Medieval":
                playerStats.IncreaseDefense(4);
                break;
            case "Chapéu mágico":
                playerController.UpgradeAttack();
                break;
        }
    }

    // -----------------------------------------------------------
    // DESCRIÇÃO DO UPGRADE (apenas para Chapéu mágico)
    // -----------------------------------------------------------
    private string GetUpgradeDescription(string title)
    {
        switch (title)
        {
            case "Chapéu mágico":
                return "Aumenta ainda mais o poder dos ataques mágicos do usuário";
            default:
                return itemDescriptions[lastIndex]; // Não deveria acontecer
        }
    }

    // -----------------------------------------------------------
    // APLICAR UPGRADE DO ITEM (apenas para Chapéu mágico)
    // -----------------------------------------------------------
    private void ApplyUpgrade(string itemName)
    {
        switch (itemName)
        {
            case "Chapéu mágico":
                playerController.UpgradeAttack();
                break;
        }
    }

    // -----------------------------------------------------------
    // FECHAR UI E DESPAUSAR JOGO
    // -----------------------------------------------------------
    private void CloseCardUI()
    {
        // Chama a função de transição da câmera no PlayerStats
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.StartCameraTransition();
        }
        else
        {
            // Fallback caso não encontre o PlayerStats
            Time.timeScale = 1f;
            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }

        CardUI.SetActive(false);
    }
}