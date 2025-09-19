using NaughtyAttributes;
using R3;
using UnityEngine;

class SkillTreeCanvas : MonoBehaviour
{
    [SerializeField, Required]
    private Canvas canvas;

    [field: SerializeField]
    public SerializableReactiveProperty<bool> IsOpen { get; private set; }

    void Awake()
    {
        IsOpen.Subscribe(isOpen =>
        {
            canvas.gameObject.SetActive(isOpen);
            // TODO: Tween

            // Pause the game when the skill tree is open
            Time.timeScale = isOpen ? 0f : 1f;
        });
    }
}
