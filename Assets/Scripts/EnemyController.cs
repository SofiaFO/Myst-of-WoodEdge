using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float attack = 10f;
    [SerializeField] private float defense = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 1.5f;
    [SerializeField] private float xpDrop;
    [SerializeField] private int moneyDrop;

    private float currentHealth;
    private float lastAttackTime = 0f;
    private Rigidbody2D _rb;
    private Transform _player;
    private PlayerStats _playerStats;
    private Animator _anim;
    private Collider2D _collider;
    private bool damaging = false;
    private SpriteRenderer _spriteRenderer;


    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private List <GameObject> moneyPrefab;

    private bool _isDead = false;
    private bool isKnockback = false;
    private float knockbackDuration = 0.2f; // tempo de empurrão

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip damageClip;

    // Rastreia quais ataques já atingiram o inimigo
    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerStats = playerObj.GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (_isDead || _player == null) return;

        if (_collider == null) return;
        float distance = Vector2.Distance((Vector2)_player.position, (Vector2)_collider.bounds.center);

        if (distance > attackRange)
            FollowPlayer();
        else
            TryAttack();
    }
    
    private void Flipar(Vector2 direction)
    {
        if (direction.x != 0)
            _spriteRenderer.flipX = direction.x > 0;
    }

    private void FollowPlayer()
    {
        if (_isDead || isKnockback) return;

        Vector2 direction = ((Vector2)_player.position - (Vector2)_collider.bounds.center).normalized;
        _rb.linearVelocity = direction * moveSpeed;

        Flipar(direction);
    }

    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime < attackRate) return;

        lastAttackTime = Time.time;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        float realDamage = Mathf.Max(1f, damage - defense);
        currentHealth -= realDamage;
        damaging = true;

        _anim.SetBool("isDamage", true);

        AudioSource.PlayClipAtPoint(damageClip, transform.position);
        Vector2 knockDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;
        float knockForce = 5f;
        StartCoroutine(DoKnockback(knockDir, knockForce));

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator DoKnockback(Vector2 direction, float force)
    {
        isKnockback = true;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
        _anim.SetBool("isDamage", false);
        damaging = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack") && _playerStats != null && !alreadyHit.Contains(collision))
        {
            alreadyHit.Add(collision); // marca que este ataque já causou dano
            TakeDamage(_playerStats.GetAttack());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack") && alreadyHit.Contains(collision))
        {
            alreadyHit.Remove(collision); // permite que o ataque cause dano novamente se colidir futuramente
        }

        if (collision.CompareTag("Player")){
            AudioSource.PlayClipAtPoint(attackClip, transform.position);
        }
    }

    private void Die()
    {
        GameManager.Instance.AddCoins(moneyDrop);
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim != null) _anim.SetTrigger("Destroy");
        if (deathClip != null) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;

        float chance = Random.value; // 0.0 a 1.0

        int index = Random.Range(0, moneyPrefab.Count);
        GameObject prefabToSpawn = moneyPrefab[index];

        if (chance < 0.7f)
        {
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        }

        StartCoroutine(DestroyAfterDelay(1.5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}