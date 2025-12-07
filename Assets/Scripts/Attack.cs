using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    [SerializeField] float xSpeed = 5f;
    [SerializeField] float moveTime = 2f;

    Rigidbody2D _rb;
    private float directionMultiplier = 1f; // Nova variável

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Método para definir a direção ANTES de OnEnable
    public void SetDirection(float direction)
    {
        directionMultiplier = direction;
    }

    private void OnEnable()
    {
        StartCoroutine(MoveForwardAndStop());
    }

    IEnumerator MoveForwardAndStop()
    {
        // Aplica velocidade com a direção correta
        _rb.linearVelocity = new Vector2(xSpeed * directionMultiplier, 0f);

        yield return new WaitForSeconds(moveTime);

        _rb.linearVelocity = Vector2.zero;
        Destroy(gameObject);
    }
}