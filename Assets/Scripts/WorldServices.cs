using UnityEngine;

public class WorldServices : MonoBehaviour
{
    [SerializeField] private MovementService movement;
    public MovementService Movement => movement;
}
