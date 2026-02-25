using UnityEngine;

public class EnemySpawnerPrototype : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    private float timer = 0f;
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1f)
        {
            Instantiate(enemyPrefabs[0]);
            timer = 0f;
        }
    }
}
