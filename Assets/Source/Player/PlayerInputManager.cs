using R3;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(SinglePlayerWeaponManager))]
class PlayerInputManager : MonoBehaviour
{
    public float Speed = 5f;
    [field: SerializeField]

    // aka isFacingLeft
    public SerializableReactiveProperty<bool> IsFlipped { get; private set; } = new(false);

    private InputAction moveAction;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.Enable();

        var spriteRenderer = GetComponent<SpriteRenderer>();

        // Handle flipping
        IsFlipped
            .Subscribe(flipped => spriteRenderer.flipX = flipped)
            .AddTo(this);
    }

    void Update()
    {
        // Handle movement
        var direction = moveAction.ReadValue<Vector2>();
        if (direction != Vector2.zero)
        {
            var translation = Speed * Time.deltaTime * direction;
            transform.Translate(translation);
            var isFlipped = translation.x < 0;
            if (isFlipped != IsFlipped.Value)
                IsFlipped.Value = isFlipped;
        }
    }
}
