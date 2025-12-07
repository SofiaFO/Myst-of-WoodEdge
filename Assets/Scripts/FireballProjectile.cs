using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyController e = col.GetComponent<EnemyController>();
            if (e != null)
                e.TakeDamage(damage);

            Destroy(gameObject);
        }else if (col.CompareTag("Boss"))
        {
            Boss b = col.GetComponent<Boss>();
            if (b != null)
                b.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}