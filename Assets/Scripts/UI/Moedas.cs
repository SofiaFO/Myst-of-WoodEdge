using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Moedas : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText; // ou TMP_Text se usar TextMeshPro

    private PlayerStats _playerStats;

    void Awake()
    {

        _playerStats = FindObjectOfType<PlayerStats>();
        if (_playerStats == null)
            Debug.LogError(" Nenhum PlayerStats encontrado na cena!");
    }

    void Update()
    {
        scoreText.text = _playerStats.Money.ToString();
    }
}
