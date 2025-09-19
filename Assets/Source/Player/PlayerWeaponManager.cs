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
        if (startingWeapon != null) EquipWeapon(startingWeapon);
    }

    public bool HasWeapon(Weapon weapon) => WeaponManagers.Any(m => m.EquippedWeapons.Contains(weapon));

    public void EquipWeapon(Weapon weapon)
    {
        if (HasWeapon(weapon)) return;

        var managerWithLeastWeapons = WeaponManagers
            .OrderBy(m => m.EquippedWeapons.Count)
            .First();

        managerWithLeastWeapons.EquipWeapon(Instantiate(weapon));
    }

    public void UnequipWeapon(Weapon weapon)
    {
        foreach (var manager in WeaponManagers)
        {
            if (manager.EquippedWeapons.Contains(weapon))
            {
                manager.UnequipWeapon(weapon);
            }
        }
    }
}
