using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

// This class is not responsible for creating/destroying weapon objects.
class SinglePlayerWeaponManager : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<Vector2> WeaponsTarget { get; private set; } = new(Vector2.right);

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
            WeaponsTarget, equippedWeaponsChanged,
            (target, _) => target
        )
            .Subscribe(target =>
                equippedWeapons.ForEach(weapon => 
                {
                    Vector2 direction = (target - (Vector2)weapon.transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                    weapon.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                })
            )
            .AddTo(this);
    }

    void Update()
    {
        var enemy = arena.FindClosestEnemy(transform.position);
        if (enemy != null)
        {
            WeaponsTarget.Value = enemy.transform.position;
            foreach (var weapon in equippedWeapons.Where(w => w.CanFire()))
            {
                weapon.Fire();
            }
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
