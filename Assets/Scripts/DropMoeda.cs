using UnityEngine;

public class DropMoeda : MonoBehaviour
{
    private PlayerStats _playerStats;
    private Transform _player;

    [SerializeField] private AudioClip pickupSound; // som ao pegar moeda
    [SerializeField] private int baseMinCoin = 3;  // Moeda mínima base
    [SerializeField] private int baseMaxCoin = 8; // Moeda máxima base
    [SerializeField] private float levelScaling = 0.5f; // Moedas crescem 50% por nível do player

    private int coinValue;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }

        // Escalar moedas baseado no nível do player
        int playerLevel = _playerStats != null ? _playerStats.Level : 1;
        int scaledMinCoin = Mathf.RoundToInt(baseMinCoin * (1 + (playerLevel - 1) * levelScaling));
        int scaledMaxCoin = Mathf.RoundToInt(baseMaxCoin * (1 + (playerLevel - 1) * levelScaling));
        
        // gerar valor aleatório de moedas escalado
        coinValue = Random.Range(scaledMinCoin, scaledMaxCoin + 1);
    }

    private void Update()
    {
        if (_player == null) return;
        float distance = Vector2.Distance(transform.position, _player.position);
        if (distance < 1f)
            transform.position = Vector2.MoveTowards(transform.position, _player.position, 3f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _playerStats != null)
        {
            _playerStats.AddMoneyFromKill(coinValue); // aplica XP
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject);           // remove o drop ap�s pegar
        }
    }
}
