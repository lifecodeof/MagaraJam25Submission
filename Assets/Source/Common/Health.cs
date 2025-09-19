using UnityEngine;
using R3;

class Health : MonoBehaviour
{
    public SerializableReactiveProperty<int> Max { get; private set; } = new(10);
    public SerializableReactiveProperty<int> Current { get; private set; } = new(10);

    // Death state, must listen to this instead of Current directly
    public ReadOnlyReactiveProperty<bool> IsDead => Current
        .Select(c => c <= 0).DistinctUntilChanged().ToReadOnlyReactiveProperty();

    void Awake()
    {
        // Clamp current health to max health on change
        Max
            .Where(max => Current.Value > max) // Prevent unnecessary changes
            .Subscribe(max => Current.Value = Mathf.Min(Current.Value, max))
            .AddTo(this);
    }
}
