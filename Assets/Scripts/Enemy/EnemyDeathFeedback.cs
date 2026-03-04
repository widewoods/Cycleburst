using UnityEngine;

public class EnemyDeathFeedback : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private GameObject deathParticlePrefab;
    private CameraFeedback feedback;

    void Awake()
    {
        feedback = FindFirstObjectByType<CameraFeedback>();
        if (enemyHealth == null) enemyHealth = GetComponentInChildren<EnemyHealth>();
    }

    void OnEnable()
    {
        enemyHealth.OnDeath += HandleEnemyDeath;
    }

    void OnDisable()
    {
        enemyHealth.OnDeath -= HandleEnemyDeath;
    }


    private void HandleEnemyDeath(EnemyHealth health)
    {
        Instantiate(deathParticlePrefab, health.transform.position, Quaternion.identity);
        feedback.CameraShake();
    }
}
