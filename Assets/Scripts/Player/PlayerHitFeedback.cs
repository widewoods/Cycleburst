using UnityEngine;

public class PlayerHitFeedback : MonoBehaviour
{
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private PlayerHealth playerHealth;
    private CameraFeedback cameraFeedback;

    void Awake()
    {
        cameraFeedback = FindFirstObjectByType<CameraFeedback>();
    }

    void OnEnable()
    {
        playerHealth.OnPlayerDeath += HandleDeath;
        playerHealth.OnPlayerHit += HandleHit;
    }

    void OnDisable()
    {
        playerHealth.OnPlayerDeath -= HandleDeath;
        playerHealth.OnPlayerHit -= HandleHit;
    }


    private void HandleDeath()
    {
        Instantiate(deathParticlePrefab, transform.position, Quaternion.identity);
        cameraFeedback.CameraShake();
    }

    private void HandleHit(float damage)
    {
        cameraFeedback.CameraShake();
        cameraFeedback.PlayerHitVignette();
    }
}
