using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRandomScript3 : MonoBehaviour
{
    [Header("Referências do UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image iconImage;

    [Header("Objetos que serão ativados/upgrade")]
    public List<GameObject> itemPrefabs;

    [Header("Banco de Itens")]
    public List<string> itemTitles = new List<string>()
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
    }

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
        else
        {
            ActivateItem(itemTitles[lastIndex]);
        }

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
            case "Machado Giratório":
                obj.GetComponent<MachadoGir>().Upgrade();
                break;

            case "Cajado Solar":
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
        CardUI.SetActive(false);
        Time.timeScale = 1f;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
    }
}
