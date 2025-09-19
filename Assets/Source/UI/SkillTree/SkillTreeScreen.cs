using NaughtyAttributes;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

class SkillTreeCanvas : MonoBehaviour
{
    [SerializeField, Required]
    private Canvas canvas;

    public ReadOnlyReactiveProperty<bool> IsOpen { get; private set; }

    void Awake()
    {
        var toggleAction = InputSystem.actions.FindAction("Toggle Skill Tree");
        toggleAction.Enable();

        IsOpen = Observable.FromEvent<InputAction.CallbackContext>(
            h => toggleAction.performed += h,
            h => toggleAction.performed -= h
        )
            .Scan(false, (isOpen, _) => !isOpen) // Toggle
            .Prepend(false)
            .ToReadOnlyReactiveProperty();

        IsOpen.Subscribe(isOpen =>
        {
            canvas.gameObject.SetActive(isOpen);
            // TODO: Tween
        });
    }
}
