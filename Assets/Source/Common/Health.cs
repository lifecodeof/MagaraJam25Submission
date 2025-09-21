using UnityEngine;
using R3;

class Health : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<int> Max { get; private set; } = new(10);

    [field: SerializeField]
    public SerializableReactiveProperty<int> Current { get; private set; } = new(10);

    // Death state, must listen to this instead of Current directly
    public ReadOnlyReactiveProperty<bool> IsDead => Current
        .Select(c => c <= 0).DistinctUntilChanged().ToReadOnlyReactiveProperty();

    // Faster access for non-reactive checks
    public bool IsDeadValue => IsDead.CurrentValue;
}
