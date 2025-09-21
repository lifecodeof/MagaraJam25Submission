using R3;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
class ScoreText : MonoBehaviour
{
    void Awake()
    {
        var textElement = GetComponent<TextMeshProUGUI>();
        var playerStateManager = Helpers.FindRequired<PlayerStateManager>();
        playerStateManager.Score
            .Select(score => score.ToString())
            .Subscribe(text => textElement.text = text)
            .AddTo(this);
    }
}
