using R3;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
class LevelText : MonoBehaviour
{
    void Awake()
    {
        var textElement = GetComponent<TextMeshProUGUI>();
        var playerStateManager = Helpers.FindRequired<PlayerStateManager>();
        playerStateManager.Level
            .Select(level => level.ToString())
            .Subscribe(text => textElement.text = text)
            .AddTo(this);
    }
}
