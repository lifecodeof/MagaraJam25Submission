using System.Linq;
using UnityEngine;

class PlayerWeaponManager : MonoBehaviour
{
    public SinglePlayerWeaponManager[] WeaponManagers { get; private set; }

    [SerializeField]
    private Weapon startingWeapon;

    void Start()
    {
        WeaponManagers = FindObjectsByType<SinglePlayerWeaponManager>(FindObjectsSortMode.None);
        if (startingWeapon != null) EquipAndUpgradeWeapon(startingWeapon);
    }

    public bool HasWeapon(Weapon weapon) => WeaponManagers.Any(m => m.EquippedWeapons.Contains(weapon));

    public void EquipAndUpgradeWeapon(Weapon weapon)
    {
        weapon.Tier += 1;
        if (HasWeapon(weapon)) return;

        var managerWithLeastWeapons = WeaponManagers
            .OrderBy(m => m.EquippedWeapons.Count)
            .First();

        managerWithLeastWeapons.EquipWeapon(weapon);
    }
}
