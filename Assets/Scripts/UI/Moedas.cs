using TMPro;
using UnityEngine;

public class Moedas : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    private PlayerStats _playerStats;

    void Awake()
    {
        _playerStats = FindObjectOfType<PlayerStats>();

        if (_playerStats == null)
            Debug.LogError("Nenhum PlayerStats encontrado na cena!");
    }

    void Update()
    {
        // Proteção contra referência nula
        if (scoreText == null)
            return;

        // Se não existir PlayerStats, mostra 0
        if (_playerStats == null)
        {
            scoreText.text = "0";
            return;
        }

        // Garante que nunca fique vazio
        int money = _playerStats.Money;

        if (money < 0)
            money = 0;

        scoreText.text = money.ToString();
    }
}
