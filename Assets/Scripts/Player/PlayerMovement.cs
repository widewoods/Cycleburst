using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 input;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    private bool isDashing = false;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            MovePlayer();
        }
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    void MovePlayer()
    {
        if (input.sqrMagnitude > 0.1f)
        {
            moveDirection = Vector2.Lerp(moveDirection, input, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            moveDirection = Vector2.Lerp(moveDirection, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
        rb.linearVelocity = moveDirection * maxSpeed;
    }

    private IEnumerator DashRoutine(Vector2 direction, float distance, float duration)
    {
        isDashing = true;

        float speed = distance / Mathf.Max(0.01f, duration);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            rb.linearVelocity = direction * speed;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }


    public void Dash(Vector2 direction, float distance, float duration)
    {
        direction = direction.normalized;
        StartCoroutine(DashRoutine(direction, distance, duration));
    }

}
