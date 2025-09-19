using UnityEngine;

[RequireComponent(typeof(PlayerWeaponEquipment))]
class PlayerWeaponManager : MonoBehaviour
{
    public PlayerWeaponEquipment EquippedWeapon { get; private set; }

    void Awake()
    {
        EquippedWeapon = GetComponent<PlayerWeaponEquipment>();
    }
}
