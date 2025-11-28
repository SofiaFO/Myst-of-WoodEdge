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

    private bool isSoftPushed = false;
    private float softPushDuration = 0.15f;

    private bool touchingPlayer = false;
    [SerializeField] private float softPushForce = 4f;
    [SerializeField] private float autoNudgeForce = 1.5f;

    private float currentHealth;
    private float lastAttackTime = 0f;

    private Rigidbody2D _rb;
    private Transform _player;
    [SerializeField] private GameObject playerSerie;
    private PlayerStats _playerStats;

    private Animator _anim;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private bool damaging = false;
    private bool _isDead = false;

    private bool isKnockback = false;
    private float knockbackDuration = 0.2f;

    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip damageClip;

    [SerializeField] private GameObject xpPrefab;
    [SerializeField] private List<GameObject> moneyPrefab;

    private HashSet<Collider2D> alreadyHit = new HashSet<Collider2D>();


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb.mass = 0.1f;
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
        else if (playerSerie != null)
        {
            _player = playerSerie.transform;
            _playerStats = playerSerie.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("Player não atribuído no Inspector!");
        }
    }


    private void Update()
    {
        if (_isDead) return;

        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _player = playerObj.transform;
                _playerStats = playerObj.GetComponent<PlayerStats>();
            }
            else
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }
        }

        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        float distance = Vector2.Distance(_player.position, _collider.bounds.center);

        if (!touchingPlayer)
        {
            if (distance > attackRange)
                FollowPlayer();
            else
                TryAttack();
        }
        else if (!isSoftPushed)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }


    private void Flipar(Vector2 direction)
    {
        if (direction.x != 0)
            _spriteRenderer.flipX = direction.x > 0;
    }


    private void FollowPlayer()
    {
        if (_isDead || isKnockback || isSoftPushed) return;

        Vector2 enemyCenter = _collider.bounds.center;
        Vector2 playerPos = _player.position;

        float distance = Vector2.Distance(playerPos, enemyCenter);
        float stopDistance = attackRange * 0.9f;

        if (distance <= stopDistance)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = (playerPos - enemyCenter).normalized;
        _rb.linearVelocity = dir * moveSpeed;
        Flipar(dir);
    }


    private void TryAttack()
    {
        _rb.linearVelocity = Vector2.zero;
        if (Time.time - lastAttackTime < attackRate) return;

        lastAttackTime = Time.time;
    }


    // ============================================================
    //                TAKE DAMAGE + KNOCKBACK SUAVE
    // ============================================================

    public void TakeDamage(float dmg)
    {
        if (_isDead) return;

        float finalDamage = Mathf.Max(1f, dmg - defense);
        currentHealth -= finalDamage;

        damaging = true;
        if (_anim != null) _anim.SetBool("isDamage", true);
        if (damageClip != null) AudioSource.PlayClipAtPoint(damageClip, transform.position);

        // calcula direção do knockback (inimigo ← ataque)
        Vector2 knockDir = ((Vector2)transform.position - (Vector2)_player.position).normalized;

        // Inicia knockback leve
        StartCoroutine(DoKnockback(knockDir, 0.8f));

        if (currentHealth <= 0)
            Die();
    }


    private IEnumerator DoKnockback(Vector2 dir, float force)
    {
        if (_isDead) yield break;

        isKnockback = true;
        _rb.linearVelocity = Vector2.zero;

        _rb.AddForce(dir * force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
        damaging = false;

        if (_anim != null) _anim.SetBool("isDamage", false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Attack")) return;

        if (alreadyHit.Contains(collision)) return;
        alreadyHit.Add(collision);
        if (_playerStats == null) {
            GameObject playerObjx = GameObject.FindGameObjectWithTag("Player");
            if (playerObjx != null)
            {
                _playerStats = playerObjx.GetComponent<PlayerStats>();
            }
        }
        float dmg = _playerStats.GetAttack();
        TakeDamage(dmg);
    }



    // ============================================================
    //                 COLISÃO PLAYER → SOFT PUSH SUAVE
    // ============================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (_rb.linearVelocity.sqrMagnitude < 0.01f) return;

        touchingPlayer = true;

        Vector2 enemyC = _collider.bounds.center;
        Vector2 playerC = collision.collider.bounds.center;

        Vector2 delta = enemyC - playerC;
        float dx = Mathf.Abs(delta.x);
        float dy = Mathf.Abs(delta.y);

        Vector2 pushDir;

        if (dx > dy)
            pushDir = new Vector2(0, Mathf.Sign(delta.y));
        else
            pushDir = new Vector2(Mathf.Sign(delta.x), 0);

        StartCoroutine(SoftPush(pushDir));
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            touchingPlayer = false;
    }


    private IEnumerator SoftPush(Vector2 dir)
    {
        isSoftPushed = true;

        float f = 0.4f;
        _rb.AddForce(dir.normalized * f, ForceMode2D.Impulse);

        yield return new WaitForSeconds(softPushDuration);
        isSoftPushed = false;
    }


    // ============================================================

    private void Die()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (_anim != null) _anim.SetTrigger("Destroy");
        if (deathClip != null) AudioSource.PlayClipAtPoint(deathClip, transform.position);

        foreach (var c in GetComponents<Collider2D>())
            c.enabled = false;

        float chance = Random.value;

        if (chance < 0.7f)
            Instantiate(xpPrefab, transform.position, Quaternion.identity);
        else
            Instantiate(moneyPrefab[Random.Range(0, moneyPrefab.Count)], transform.position, Quaternion.identity);

        StartCoroutine(DestroyAfterDelay(0.5f));
    }


    private IEnumerator DestroyAfterDelay(float d)
    {
        yield return new WaitForSeconds(d);
        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
