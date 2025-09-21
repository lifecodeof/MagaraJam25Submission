using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using R3;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

class SkillTreeCanvas : MonoBehaviour
{
    [SerializeField, Required]
    private Canvas canvas;

    [field: SerializeField]
    public float AnimationDuration { get; private set; } = 0.5f;

    [field: SerializeField]
    public SerializableReactiveProperty<bool> IsOpen { get; private set; }

    [field: SerializeField]
    public GameObject GameOverText { get; private set; }

    private RectTransform panelRectTransform;

    void Start()
    {
        var toggleSkillTreeAction = InputSystem.actions.FindAction("Toggle Skill Tree");
        toggleSkillTreeAction.Enable();
        Observable.FromEvent<InputAction.CallbackContext>(
            h => toggleSkillTreeAction.performed += h,
            h => toggleSkillTreeAction.performed -= h
        )
            .Subscribe(_ => IsOpen.Value = !IsOpen.Value)
            .AddTo(this);

        var playerDeathConditions = GameObject.FindGameObjectsWithTag("Player")
            .Select(p => p.GetComponent<Health>())
            .Where(h => h != null)
            .Select(h => h.IsDead)
            .ToArray();

        var isGameOver = Observable
          .CombineLatest(playerDeathConditions)
          .Select(states => states.Any(s => s));

        panelRectTransform = canvas.transform.GetChild(0).GetComponent<RectTransform>();

        Observable.CombineLatest(
            IsOpen, isGameOver,
            (isOpen, gameOver) => isOpen || gameOver
        )
            .DistinctUntilChanged()
            .Subscribe(isOpen =>
            {
                if (isOpen)
                    CloseToOpen();
                else
                    OpenToClose();

                // Pause the game when the skill tree is open
                Time.timeScale = isOpen ? 0f : 1f;
            })
            .AddTo(this);
    }

    private void CloseToOpen()
    {
        panelRectTransform.gameObject.SetActive(true);
        panelRectTransform
            .DOScale(Vector3.one, AnimationDuration)
            .SetEase(Ease.OutBack)
            .From(Vector3.zero, true)
            .SetUpdate(true); // Ignore timeScale
    }

    private void OpenToClose()
    {
        panelRectTransform
            .DOScale(Vector3.zero, AnimationDuration)
            .SetEase(Ease.InBack)
            .From(Vector3.one, true)
            .SetUpdate(true) // Ignore timeScale
            .OnComplete(() => panelRectTransform.gameObject.SetActive(false));
    }
}
