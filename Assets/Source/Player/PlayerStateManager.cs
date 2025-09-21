using System.Linq;
using R3;
using UnityEngine;

// TODO: Handle game over

[RequireComponent(typeof(Health))]
class PlayerStateManager : MonoBehaviour
{
    [field: SerializeField]
    public SerializableReactiveProperty<int> Level { get; private set; } = new(0);

    [field: SerializeField]
    public SerializableReactiveProperty<int> Xp { get; private set; } = new(0);

    [field: SerializeField]
    public SerializableReactiveProperty<int> SpentSkillPoints { get; private set; } = new(0);

    [field: SerializeField]
    public SerializableReactiveProperty<int> Score { get; private set; } = new(0);

    public Observable<bool> CanSpendSkillPoint =>
        Observable.CombineLatest(Level, SpentSkillPoints)
            .Select(t => t[0] > t[1]);

    public static int MaxXpForLevel(int level) => (level + 1) * 100;

    void Start()
    {
        var health = GetComponent<Health>();
        var skillTreeScreen = Helpers.FindRequired<SkillTreeCanvas>();

        health.IsDead.WhereTrue()
            .Subscribe(_ => Debug.Log("game over"))
            .AddTo(this);

        Events.EnemyKilled
            .Do(_ => Score.Value += 1)
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

        var sources = FindObjectsByType<SkillTreeSkillButton>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        )
            .Select(button => button.IsUnlocked)
            .Append(Observable.Return(true)) // in case there are no skills
            .ToList();

        var isEverySkillUnlocked = Observable
            .CombineLatest(sources)
            .Select(states => states.All(s => s));

        Observable.CombineLatest(
            CanSpendSkillPoint, isEverySkillUnlocked,
            (canSpend, allUnlocked) => canSpend && !allUnlocked
        )
            .Subscribe(canSpend => skillTreeScreen.IsOpen.Value = canSpend)
            .AddTo(this);
    }
}
