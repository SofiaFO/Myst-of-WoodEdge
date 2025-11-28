using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRandomScript2 : MonoBehaviour
{
    [Header("Referências do UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private List<GameObject> itemPrefabs;

    [Header("Banco de Itens")]
    [Header("Banco de Itens")]
    [SerializeField]
    private List<string> itemTitles = new List<string>()
    {
        "Machado Giratório",
        //"Amuleto da Serpente Dourada",
        //"Martelo do Trovão",
        //"Coroa do Rei Caído",
    };

        [SerializeField]
        private List<string> itemDescriptions = new List<string>()
    {
        "Gera um machado que orbita ao redor do jogador causando dano contínuo.",
        //"Aumenta o dano contra inimigos envenenados e aplica veneno ao atacar.",
        //"Golpes pesados criam ondas de choque que empurram e atordoam inimigos.",
        //"Aumenta vida máxima em 40% e concede regeneração acelerada em batalha.",
    };

    [SerializeField] private List<Sprite> itemSprites = new List<Sprite>();

    private int lastIndex = -1;

    private GameObject CardUI;

    void Awake()
    {
        CardUI = GameObject.FindWithTag("CardUI");
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
        itemPrefabs[lastIndex].SetActive(true);

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
            case "Machado Giratório":
                itemPrefabs[lastIndex].GetComponent<MachadoGir>().Upgrade();
                break;
        }
    }

    // -----------------------------------------------------------
    // FECHAR UI E DESPAUSAR JOGO
    // -----------------------------------------------------------
    private void CloseCardUI()
    {
        CardUI.SetActive(false);
        Time.timeScale = 1f;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }
}