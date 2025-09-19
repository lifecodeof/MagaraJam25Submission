using R3;
using TMPro;
using UnityEngine;

class SkillPointIndicator : MonoBehaviour
{
    private readonly static string template = "{0} yetenek puanı var. Yetenek ağacını açmak için 'E' tuşuna basın.";

    [SerializeField]
    private TMP_Text text;

    void Awake()
    {
        var playerStateManager = Helpers.FindRequired<PlayerStateManager>();

        Observable.CombineLatest(
            playerStateManager.Level,
            playerStateManager.SpentSkillPoints,
            (level, points) => level - points
        ) // Remaining skill points
            .Do(remaining => text.enabled = remaining > 0)
            .Select(remaining => string.Format(template, remaining.ToString()))
            .Subscribe(s => text.text = s)
            .AddTo(this);
    }
}
