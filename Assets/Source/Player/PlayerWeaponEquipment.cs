using R3;
using UnityEngine;

// This class is not responsible for creating/destroying weapon objects.
// TODO: Handle multiple weapons.
[RequireComponent(typeof(PlayerInputManager))]
class PlayerWeaponEquipment : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<float> WeaponHoldPointOffset { get; private set; } = new(1f);

    [field: SerializeField]
    public SerializableReactiveProperty<Vector2> WeaponDirection { get; private set; } = new(Vector2.right);

    /// <summary>nullable</summary>
    [field: SerializeField]
    public SerializableReactiveProperty<Weapon> EquippedWeapon { get; private set; } = new(null);

    // Weapon hold point with flipping taken into account.
    public Observable<Vector2> WeaponHoldPointRelative =>
        Observable.CombineLatest( // Whichever changes first
            WeaponHoldPointOffset, WeaponDirection,
            (offset, direction) => (offset, direction)
        ).Select(t => t.offset * t.direction);

    private Arena arena;
    private float lastFireTime;

    void Awake()
    {
        arena = Helpers.FindRequired<Arena>();

        // Handle relative weapon position updates
        Observable.CombineLatest(
            WeaponHoldPointRelative, EquippedWeapon,
            (position, weapon) => (position, weapon)
        )
            .Where(t => t.weapon != null)
            .Subscribe(t => t.weapon.transform.localPosition = t.position)
            .AddTo(this);

        // Handle weapon direction updates
        Observable.CombineLatest(
            WeaponDirection, EquippedWeapon,
            (direction, weapon) => (direction, weapon)
        )
            .Where(t => t.weapon != null)
            .Subscribe(t =>
                t.weapon.transform.localRotation = Quaternion.FromToRotation(Vector2.right, t.direction)
            )
            .AddTo(this);
    }

    void Update()
    {
        var enemy = arena.FindClosestEnemy(transform.position);
        if (enemy != null)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            WeaponDirection.Value = direction;
        }
    }

    public void TryFireWeapon()
    {
        var weapon = EquippedWeapon.Value;
        if (weapon != null)
        {
            if (Time.time - lastFireTime >= weapon.Cooldown)
            {
                weapon.Fire(WeaponDirection.Value);
                lastFireTime = Time.time;
            }
        }
    }
}
