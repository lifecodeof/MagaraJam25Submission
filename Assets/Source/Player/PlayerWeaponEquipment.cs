using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

// This class is not responsible for creating/destroying weapon objects.
// TODO: Handle multiple weapons.
[RequireComponent(typeof(PlayerInputManager))]
class PlayerWeaponEquipment : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<Vector2> WeaponsDirection { get; private set; } = new(Vector2.right);

    [SerializeField]
    private Weapon startingWeapon;

    // I would usually use reactive collections but its way more complex to set up.
    private readonly List<Weapon> equippedWeapons = new();
    public IReadOnlyList<Weapon> EquippedWeapons => equippedWeapons;

    // This is the reactive equivalent of an event.
    private readonly Subject<Unit> equippedWeaponsChanged = new();

    private Arena arena;

    void Awake()
    {
        arena = Helpers.FindRequired<Arena>();

        // Handle weapon direction updates
        Observable.CombineLatest(
            WeaponsDirection, equippedWeaponsChanged,
            (direction, _) => direction
        )
            .Subscribe(direction =>
                equippedWeapons.ForEach(weapon =>
                    weapon.transform.localRotation = Quaternion.FromToRotation(Vector2.right, direction)
                )
            )
            .AddTo(this);

        if (startingWeapon != null) EquipWeapon(startingWeapon);

        // TODO: Remove this
        // For demo, equip weapon on every 3 enemy kill
        Helpers.FindRequired<PlayerStateManager>()
            .Level.Skip(1)
            .Subscribe(_ => EquipWeapon(Instantiate(startingWeapon)))
            .AddTo(this);
    }

    void Update()
    {
        var enemy = arena.FindClosestEnemy(transform.position);
        if (enemy != null)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            WeaponsDirection.Value = direction;
            foreach (var weapon in equippedWeapons.Where(w => w.CanFire()))
                weapon.Fire(WeaponsDirection.Value);
        }
    }

    private IEnumerable<Vector2> DistributePointsIn2Columns(int count)
    {
        int half = (count + 1) / 2;
        float spacing = 0.5f; // Spacing between weapons
        for (int i = 0; i < count; i++)
        {
            int column = i < half ? -1 : 1; // Left or right column
            int indexInColumn = i < half ? i : i - half;
            float yOffset = (indexInColumn - (half - 1) / 2f) * spacing;
            yield return new Vector2(column * 0.5f, yOffset);
        }
    }

    void RepositionWeapons()
    {
        var pairs = Enumerable.Zip(
            DistributePointsIn2Columns(equippedWeapons.Count),
            equippedWeapons,
            (point, weapon) => (point, weapon)
        );
        foreach (var (point, weapon) in pairs)
            weapon.transform.localPosition = point;
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (equippedWeapons.Contains(weapon)) return;

        equippedWeapons.Add(weapon);
        weapon.transform.SetParent(transform);

        RepositionWeapons();

        // Fire equippedWeaponsChanged event
        equippedWeaponsChanged.OnNext(Unit.Default);
    }

    public void UnequipWeapon(Weapon weapon)
    {
        if (!equippedWeapons.Contains(weapon)) return;

        equippedWeapons.Remove(weapon);
        weapon.transform.SetParent(null);

        RepositionWeapons();

        // Fire equippedWeaponsChanged event
        equippedWeaponsChanged.OnNext(Unit.Default);
    }
}
