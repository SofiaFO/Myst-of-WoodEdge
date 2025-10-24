using UnityEngine;

public class DropXP : MonoBehaviour
{
    private PlayerStats _playerStats;
    private Transform _player;

    [SerializeField] private AudioClip pickupSound; // som ao pegar XP
    [SerializeField] private int minXP = 5;  // XP mínimo
    [SerializeField] private int maxXP = 20; // XP máximo

    private int xpValue;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }

        // gerar valor aleatório de XP
        xpValue = Random.Range(minXP, maxXP + 1);

        // ajustar escala proporcional ao XP
        float scale = 1f + (xpValue - minXP) / (float)(maxXP - minXP); // escala entre 1 e 2
        transform.localScale = new Vector3(scale, scale, scale);
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
            _playerStats.GainXP(xpValue); // aplica XP
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject);           // remove o drop após pegar
        }
    }
}
