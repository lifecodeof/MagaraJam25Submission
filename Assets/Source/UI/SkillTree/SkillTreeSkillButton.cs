using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
class SkillTreeSkillButton : MonoBehaviour
{
    // Below 2 fields do not react to playtime changes in the inspector. Because I'm too lazy to make it reactive rn

    [SerializeField]
    private List<SkillTreeSkillButton> prerequisites = new();
    public IReadOnlyList<SkillTreeSkillButton> Prerequisites => prerequisites;

    [field: SerializeField]
    public int Cost { get; private set; } = 1;

    [field: SerializeField]
    public SerializableReactiveProperty<bool> IsUnlocked { get; private set; } = new(false);

    void Awake()
    {
        var playerStateManager = Helpers.FindRequired<PlayerStateManager>();
        var button = GetComponent<Button>();

        var notUnlocked = IsUnlocked.Select(b => !b);

        // Can interact 
        // if all prerequisites are unlocked
        // && player can spend skill point
        // && this skill is not unlocked
        Observable.CombineLatest(
            prerequisites
                .Select(p => p.IsUnlocked as Observable<bool>)
                .Append(playerStateManager.CanSpendSkillPoint)
                .Append(notUnlocked)
        )
            .Select(states => states.All(s => s))
            .Subscribe(i => button.interactable = i)
            .AddTo(this);

        // On click, unlock skill
        button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                IsUnlocked.Value = true;
                playerStateManager.SpentSkillPoints.Value += Cost;
                Events.SkillUnlocked.OnNext(this);
            })
            .AddTo(this);

        // Change color if unlocked
        var colors = button.colors;
        IsUnlocked
            .Subscribe(unlocked =>
            {
                var c = colors;
                c.normalColor = unlocked ? Color.green : colors.normalColor;
                c.highlightedColor = unlocked ? Color.green : colors.highlightedColor;
                c.pressedColor = unlocked ? Color.green : colors.pressedColor;
                c.disabledColor = unlocked ? Color.green : colors.disabledColor;
                c.selectedColor = unlocked ? Color.green : colors.selectedColor;
                button.colors = c;
            })
            .AddTo(this);
    }
}
