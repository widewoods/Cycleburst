using UnityEngine;

public class EnemyFollowPrototype : MonoBehaviour
{
    private Transform playerTransform;

    void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, playerTransform.position, 1 * Time.deltaTime);
    }
}
