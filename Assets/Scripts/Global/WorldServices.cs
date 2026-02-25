using UnityEngine;

public class WorldServices : MonoBehaviour
{
    [SerializeField] private MovementService movement;
    [SerializeField] private DamageService damage;

    public MovementService Movement => movement;
    public DamageService Damage => damage;
}
