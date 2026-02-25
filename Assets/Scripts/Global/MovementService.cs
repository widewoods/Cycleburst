using UnityEngine;

public class MovementService : MonoBehaviour
{
    public void Dash(Transform caster, Vector2 direction, float distance, float duration)
    {
        var mover = caster.GetComponent<PlayerMovement>();
        if (mover == null) return;
        mover.Dash(direction, distance, duration);
    }
}
