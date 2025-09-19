using UnityEngine;

abstract class Weapon : MonoBehaviour
{
    public abstract float Cooldown { get; }

    public void Fire(Vector2 direction)
    {
        OnFire(direction);
    }

    protected abstract void OnFire(Vector2 direction);
}
