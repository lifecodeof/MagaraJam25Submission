using UnityEngine;

[RequireComponent(typeof(PlayerEquippedWeapon))]
class PlayerWeaponManager : MonoBehaviour
{
    public PlayerEquippedWeapon EquippedWeapon { get; private set; }

    void Awake()
    {
        EquippedWeapon = GetComponent<PlayerEquippedWeapon>();
    }
}
