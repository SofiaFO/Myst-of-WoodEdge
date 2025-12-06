using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaPrefab : MonoBehaviour
{
    [Header("Configuração")]
    public float damage = 15f;
    public float lifetime = 3f;             // Tempo de vida antes de se destruir
    public float damageInterval = 0.5f;     // Intervalo para aplicar dano

    private HashSet<Collider2D> enemiesInArea = new HashSet<Collider2D>();
    private bool isActive = true;

    void Start()
    {
        // Inicia a corrotina de dano contínuo
        StartCoroutine(DamageRoutine());

        // Destrói após o tempo de vida
        Destroy(gameObject, lifetime);
    }

    private IEnumerator DamageRoutine()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(damageInterval);

            // Cria uma cópia para evitar erro de modificação durante iteração
            List<Collider2D> enemiesCopy = new List<Collider2D>(enemiesInArea);

            // Aplica dano em todos os inimigos na área
            foreach (Collider2D enemy in enemiesCopy)
            {
                if (enemy == null) continue;

                ApplyDamage(enemy);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            enemiesInArea.Add(collision);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Garante que o inimigo está na lista
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            enemiesInArea.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            enemiesInArea.Remove(collision);
        }
    }

    private void ApplyDamage(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemyController = collision.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }
        }
        else if (collision.CompareTag("Boss"))
        {
            Boss bossController = collision.GetComponent<Boss>();
            if (bossController != null)
            {
                bossController.TakeDamage(damage);
            }
        }
    }

    void OnDestroy()
    {
        isActive = false;
        enemiesInArea.Clear();
    }
}