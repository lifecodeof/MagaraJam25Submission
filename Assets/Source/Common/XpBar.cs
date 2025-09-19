using R3;
using UnityEngine;
using UnityEngine.UI;

class XpBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    void Awake()
    {
        var playerStateManager = Helpers.FindRequired<PlayerStateManager>();

        playerStateManager.Level
            .Select(PlayerStateManager.MaxXpForLevel)
            .Subscribe(maxXp => slider.maxValue = maxXp)
            .AddTo(this);

        playerStateManager.Xp
            .Subscribe(xp => slider.value = xp)
            .AddTo(this);
    }
}
