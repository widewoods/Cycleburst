using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 mousePosition;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Aim();
    }

    void OnAim(InputValue value)
    {
        mousePosition = value.Get<Vector2>();
        Debug.Log(mousePosition);
    }

    void Aim()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        mouseWorldPosition.z = 0f;

        Vector3 directionToTarget = mouseWorldPosition - transform.position;
        directionToTarget.Normalize();

        float angleRadians = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angleDegrees);
    }
}
