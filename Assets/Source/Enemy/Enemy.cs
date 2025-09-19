using R3;
using UnityEngine;

// TODO: Handle death

[RequireComponent(typeof(Health))]
abstract class Enemy : MonoBehaviour
{
    public virtual bool FollowsPlayer { get; protected set; } = true;
    public virtual float Speed { get; protected set; } = 1f;

    public virtual Damage Damage => new Damage.Physical(1, new DamageSource.Enemy(this));

    private GameObject player;
    protected Health health;

    public virtual void Awake()
    {
        player = Helpers.FindRequired<PlayerInputManager>().gameObject;
        health = GetComponent<Health>();

        health.IsDead.Where(b => b)
            .Subscribe(_ => Debug.Log($"{name} died"))
            .AddTo(this);
    }

    public virtual void Update()
    {
        if (FollowsPlayer)
        {
            var direction = (player.transform.position - transform.position).normalized;
            transform.Translate(Speed * Time.deltaTime * direction);
        }
    }

    public void TakeDamage(Damage damage)
    {
        OnTakeDamage(damage);
    }

    protected virtual void OnTakeDamage(Damage damage)
    {
        health.Current.Value -= damage.Amount;
    }
}
