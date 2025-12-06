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
        "Cajado Solar"
    };

    public List<string> itemDescriptions = new List<string>()
    {
        "Gera um machado que orbita ao redor do jogador causando dano contínuo.",
        "Projéteis de luz causam dano aumentado e queimam inimigos atingidos."
    };

    public List<Sprite> itemSprites = new List<Sprite>();

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
        lastIndex = Random.Range(0, itemTitles.Count);

        titleText.text = itemTitles[lastIndex];
        descriptionText.text = itemDescriptions[lastIndex];
        iconImage.sprite = itemSprites[lastIndex];

        PauseGame();
    }

    public void ApplyItemEffect()
    {
        if (lastIndex < 0) return;

        GameObject obj = itemPrefabs[lastIndex];

        if (obj.activeSelf)
        {
            ApplyUpgrade(itemTitles[lastIndex]);
        }

        // Se ainda não tem → ativar primeira vez
        PlayerPrefs.SetInt($"ITEM_{itemName}", 1);
        PlayerPrefs.Save();

        // Tenta achar objeto existente

        CloseCardUI();
    }

    private void ActivateItem(string itemName)
    {
        GameObject obj = itemPrefabs[lastIndex];
        obj.SetActive(true);

        switch (itemName)
        {
            case "Machado Giratório":
                break;

            case "Cajado Solar":
                break;
        }
    }

    private void ApplyUpgrade(string itemName)
    {
        GameObject obj = itemPrefabs[lastIndex];

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

    private void PauseGame()
    {
        Time.timeScale = 0f;
        Physics2D.simulationMode = SimulationMode2D.Script;
    }

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
