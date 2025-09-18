using R3;
using UnityEngine;

// This class is not responsible for creating/destroying weapon objects.
[RequireComponent(typeof(PlayerMovement))]
class PlayerEquippedWeapon : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<float> WeaponHoldPointOffset { get; private set; } = new(1f);

    [field: SerializeField]
    public SerializableReactiveProperty<Vector2> WeaponDirection { get; private set; } = new(Vector2.right);

    /// <summary> nullable </summary>
    [field: SerializeField]
    public SerializableReactiveProperty<Weapon> EquippedWeapon { get; private set; } = new(null);

    // Weapon hold point with flipping taken into account.
    public Observable<Vector2> WeaponHoldPointRelative =>
        Observable.CombineLatest( // Whichever changes first
            WeaponHoldPointOffset, playerMovement.IsFlipped, WeaponDirection,
            (offset, isFlipped, direction) => (offset, isFlipped, direction)
        ).Select(
            t => t.offset * new Vector2(
                t.direction.normalized.x * (t.isFlipped ? -1 : 1),
                t.direction.normalized.y
            )
        );


    private PlayerMovement playerMovement;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        // Handle relative weapon position updates
        Observable.CombineLatest(
            WeaponHoldPointRelative, EquippedWeapon,
            (position, weapon) => (position, weapon)
        )
            .Where(t => t.weapon != null)
            .Subscribe(t => t.weapon.transform.localPosition = t.position)
            .AddTo(this);
    }
}
