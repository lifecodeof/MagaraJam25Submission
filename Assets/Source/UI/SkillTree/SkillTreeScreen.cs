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
    public GameObject GameOverText { get; private set; }

    public Observable<bool> IsOpen { get; private set; }

    private RectTransform panelRectTransform;

    void Start()
    {
        var toggleSkillTreeAction = InputSystem.actions.FindAction("Toggle Skill Tree");
        toggleSkillTreeAction.Enable();
        var isOpenFromKey = Observable.FromEvent<InputAction.CallbackContext>(
            h => toggleSkillTreeAction.performed += h,
            h => toggleSkillTreeAction.performed -= h
        )
            .Scan(false, (isOpen, _) => !isOpen)
            .Prepend(false);

        var playerDeathConditions = GameObject.FindGameObjectsWithTag("Player")
            .Select(p => p.GetComponent<Health>())
            .Where(h => h != null)
            .Select(h => h.IsDead)
            .ToArray();

        var isGameOver = Observable
          .CombineLatest(playerDeathConditions)
          .Select(states => states.Any(s => s));

        panelRectTransform = canvas.transform.GetChild(0).GetComponent<RectTransform>();

        var psm = Helpers.FindRequired<PlayerStateManager>();

        isGameOver.Subscribe(ig => Debug.Log($"Game Over: {ig}")).AddTo(this);

        Observable.CombineLatest(
            isOpenFromKey, psm.CanSpendSkillPoint,
            psm.IsEverySkillUnlocked, isGameOver,
            (
                isOpenFromKey, canSpend,
                allUnlocked, gameOver
            ) => isOpenFromKey || gameOver || (canSpend && !allUnlocked))
            .Do(isOpen => Debug.Log($"Skill Tree Open: {isOpen}"))
            .DistinctUntilChanged()
            .Do(isOpen => Debug.Log($"Skill Tree Open2: {isOpen}"))
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

        isGameOver
            .DistinctUntilChanged()
            .Subscribe(ig => GameOverText.SetActive(ig))
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
