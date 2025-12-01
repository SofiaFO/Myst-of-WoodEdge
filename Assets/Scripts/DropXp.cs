using UnityEngine;

public class DropXP : MonoBehaviour
{
    private PlayerStats _playerStats;
    private Transform _player;

    [SerializeField] private AudioClip pickupSound; // som ao pegar XP
    [SerializeField] private int baseMinXP = 5;  // XP mínimo base
    [SerializeField] private int baseMaxXP = 15; // XP máximo base
    [SerializeField] private float levelScaling = 0.5f; // XP cresce 50% por nível do player

    private int xpValue;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }

        // Escalar XP baseado no nível do player
        int playerLevel = _playerStats != null ? _playerStats.Level : 1;
        int scaledMinXP = Mathf.RoundToInt(baseMinXP * (1 + (playerLevel - 1) * levelScaling));
        int scaledMaxXP = Mathf.RoundToInt(baseMaxXP * (1 + (playerLevel - 1) * levelScaling));
        
        // gerar valor aleatório de XP escalado
        xpValue = Random.Range(scaledMinXP, scaledMaxXP + 1);

        // ajustar escala visual proporcional ao XP
        float scale = 1f + (xpValue - scaledMinXP) / Mathf.Max(1f, (float)(scaledMaxXP - scaledMinXP)); // escala entre 1 e 2
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
            Destroy(gameObject);           // remove o drop ap�s pegar
        }
    }
}
