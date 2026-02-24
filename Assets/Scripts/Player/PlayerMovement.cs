using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Vector2 input;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        MovePlayer();
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
}
