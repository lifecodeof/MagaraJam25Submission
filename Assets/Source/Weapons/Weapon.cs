using NaughtyAttributes;
using UnityEngine;

abstract class Weapon : MonoBehaviour
{
    public abstract float Cooldown { get; }

    public float LastFireTime { get; private set; } = 0f;

    public void Fire()
    {
        LastFireTime = Time.time;
        OnFire(transform.rotation);
    }

    // based on cooldown
    public virtual bool CanFire() => Time.time - LastFireTime >= Cooldown;

    protected abstract void OnFire(Quaternion direction);

    // Inspector buttons
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void Equip()
    {
        Helpers.FindRequired<PlayerInputManager>()
            .GetComponent<PlayerWeaponEquipment>()
            .EquipWeapon(Instantiate(this));
    }
}
