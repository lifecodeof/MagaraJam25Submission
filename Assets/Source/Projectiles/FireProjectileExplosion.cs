using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
class FireProjectileExplosion : MonoBehaviour
{
    public int DamageAmount = 10;

    [SerializeField]
    private float MaxRadius = 10f;

    [SerializeField]
    private float ExplosionDuration = 0.3f;

    [SerializeField]
    private float FadeDuration = 0.3f;

    [SerializeField, ReadOnly]
    private float currentRadius = 0f;

    [SerializeField, ReadOnly]
    private List<Enemy> affectedEnemies = new();

    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();

        transform
            .DOScale(Vector3.one * MaxRadius, ExplosionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
                DOTween.ToAlpha(
                    () => sr.color, c => sr.color = c, 0, FadeDuration
                ).OnComplete(() => Destroy(gameObject))
            );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            if (!affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                enemy.TakeDamage(new Damage(DamageAmount));
            }
        }
    }
}
