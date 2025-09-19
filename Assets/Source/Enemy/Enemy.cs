using R3;
using UnityEngine;

// TODO: Handle death

[RequireComponent(typeof(Health))]
abstract class Enemy : MonoBehaviour
{
    public virtual bool FollowsPlayer { get; protected set; } = true;
    public virtual float Speed { get; protected set; } = 1f;

    public virtual Damage Damage => new Damage.Physical(1, new DamageSource.Enemy(this));

    public Health Health { get; private set; }

    private GameObject player;

    public virtual void Awake()
    {
        player = Helpers.FindRequired<PlayerInputManager>().gameObject;
        Health = GetComponent<Health>();

        Health.IsDead.WhereTrue()
            .Subscribe(_ =>
            {
                // lay down
                transform.Rotate(0, 0, 90);
                // disable collider
                if (TryGetComponent<Collider2D>(out var collider))
                    collider.enabled = false;
            })
            .AddTo(this);
    }

    public virtual void Update()
    {
        if (Health.IsDeadValue) return;

        if (FollowsPlayer)
        {
            var direction = (player.transform.position - transform.position).normalized;
            transform.Translate(Speed * Time.deltaTime * direction);
        }
    }

    public void TakeDamage(Damage damage)
    {
        DamageIndicator.IndicateDamage(gameObject, damage);
        OnTakeDamage(damage);
    }

    protected virtual void OnTakeDamage(Damage damage)
    {
        Health.Current.Value -= damage.Amount;
    }
}
