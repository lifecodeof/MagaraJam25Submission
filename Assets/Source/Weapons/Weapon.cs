using NaughtyAttributes;
using UnityEngine;

abstract class Weapon : MonoBehaviour
{
    public abstract float Cooldown { get; }

    public float LastFireTime { get; private set; } = 0f;

    public void Fire(Vector2 direction)
    {
        LastFireTime = Time.time;
        OnFire(direction);
    }

    // based on cooldown
    public virtual bool CanFire() => Time.time - LastFireTime >= Cooldown;

    protected abstract void OnFire(Vector2 direction);

    // Inspector buttons
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void Equip()
    {
        Helpers.FindRequired<PlayerInputManager>()
            .GetComponent<PlayerWeaponEquipment>()
            .EquipWeapon(Instantiate(this));
    }
}
