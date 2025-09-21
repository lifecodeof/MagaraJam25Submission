using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using R3;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteRenderer))]
class Enemy : MonoBehaviour
{
    public bool FollowsPlayer = true;
    public float Speed = 1f;
    public int DamageAmount = 1;

    public HashSet<GameObject> KnockBackEffectors = new();

    public Health Health { get; private set; }

    private GameObject playersRoot;

    [field: SerializeField, ReadOnly]
    private float lastAttackTime = 0f;

    [field: SerializeField]
    public float AttackCooldown { get; private set; } = 0.5f;

    public virtual void Awake()
    {
        playersRoot = GameObject.FindGameObjectWithTag("PlayersRoot");
        Health = GetComponent<Health>();

        Health.IsDead.WhereTrue()
            .Subscribe(_ =>
            {
                // lay down
                transform.Rotate(0, 0, 90);
                // disable collider
                if (TryGetComponent<Collider2D>(out var collider))
                    collider.enabled = false;

                Events.EnemyKilled.OnNext(Unit.Default);

                // dotween fade out and destroy
                var sr = GetComponent<SpriteRenderer>();

                DOTween.ToAlpha(
                    () => sr.color, c => sr.color = c, 0, 1f
                ).OnComplete(() => Destroy(gameObject));
            })
            .AddTo(this);
    }

    public virtual void Update()
    {
        if (Health.IsDeadValue) return;

        if (FollowsPlayer)
        {
            var direction = (playersRoot.transform.position - transform.position).normalized;

            var hasKnockbackEffector = KnockBackEffectors.Any(effector => effector != null);
            if (hasKnockbackEffector) direction *= -4;

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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (
            collision.gameObject.CompareTag("Player") &&
            collision.gameObject.TryGetComponent<Health>(out var playerHealth)
        )
        {
            if (Time.time - lastAttackTime >= AttackCooldown)
            {
                lastAttackTime = Time.time;
                playerHealth.Current.Value -= DamageAmount;
            }
        }
    }
}
