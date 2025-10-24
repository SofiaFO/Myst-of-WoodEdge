using UnityEngine;

public class DropMoeda : MonoBehaviour
{
    private PlayerStats _playerStats;
    private Transform _player;

    [SerializeField] private AudioClip pickupSound; // som ao pegar XP
    [SerializeField] private int minCoin = 3;  // Coin mínimo
    [SerializeField] private int maxCoin = 5; // Coin máximo

    private int coinValue;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }

        // gerar valor aleatório de XP
        coinValue = Random.Range(minCoin, maxCoin + 1);
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
            _playerStats.AddMoney(coinValue); // aplica XP
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject);           // remove o drop após pegar
        }
    }
}
