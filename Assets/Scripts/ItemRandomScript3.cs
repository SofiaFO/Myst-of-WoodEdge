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
        "Machado Giratório",
        "Cajado Solar",
        //"Adaga Sombria",
        //"Escudo de Pedra Rúnica",
    };

    [SerializeField]
    private List<string> itemDescriptions = new List<string>()
    {
        "Gera um machado que orbita ao redor do jogador causando dano contínuo.",
        "Projetos de luz causam dano aumentado e queimam inimigos atingidos.",
        //"Aumenta muito a chance de crítico e permite ataques pelas costas mais fortes.",
        //"Reduz dano recebido em 25% e gera um pequeno escudo a cada 10 segundos.",
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
        bool alreadyOwned = PlayerPrefs.GetInt($"ITEM_{title}", 0) == 1;

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
        bool alreadyOwned = PlayerPrefs.GetInt($"ITEM_{itemName}", 0) == 1;

        // Se já tem → chamar upgrade
        if (alreadyOwned)
        {
            ApplyUpgrade(itemName);
            CloseCardUI();
            return;
        }

        // Se ainda não tem → ativar primeira vez
        PlayerPrefs.SetInt($"ITEM_{itemName}", 1);
        PlayerPrefs.Save();

        // Tenta achar objeto existente

        CloseCardUI();
    }

    // -----------------------------------------------------------
    // DESCRIÇÃO DO UPGRADE
    // -----------------------------------------------------------
    private string GetUpgradeDescription(string title)
    {
        switch (title)
        {
            case "Machado Giratório":
                return "Aumenta o dano e a velocidade do machado.";
        }

        return "Upgrade aplicado.";
    }

    // -----------------------------------------------------------
    // APLICAR UPGRADE DO ITEM
    // -----------------------------------------------------------
    private void ApplyUpgrade(string itemName)
    {
        switch (itemName)
        {
            case "Poção de Cura":
                playerStats.Heal(playerStats.GetHealth()/2);
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