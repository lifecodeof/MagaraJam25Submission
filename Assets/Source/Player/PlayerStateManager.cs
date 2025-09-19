using R3;
using UnityEngine;

// TODO: Handle game over

[RequireComponent(typeof(Health))]
class PlayerStateManager : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<int> Level { get; private set; } = new(1);

    [field: SerializeField]
    public SerializableReactiveProperty<int> Xp { get; private set; } = new(0);

    public static int MaxXpForLevel(int level) => level * 100;

    void Awake()
    {
        var health = GetComponent<Health>();
        health.IsDead.WhereTrue()
            .Subscribe(_ => Debug.Log("game over"))
            .AddTo(this);

        Events.EnemyKilled
            .Subscribe(_ => Xp.Value += 10)
            .AddTo(this);

        Xp
            .Where(xp => xp >= MaxXpForLevel(Level.Value))
            .Subscribe(_ =>
            {
                Level.Value += 1;
                Xp.Value = 0;
            })
            .AddTo(this);
    }
}
