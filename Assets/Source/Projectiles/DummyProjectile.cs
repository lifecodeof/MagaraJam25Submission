using R3.Triggers;
using R3;
using UnityEngine;
using System;

class DummyProjectile : Projectile
{
    public float Lifetime = 5f;
    public float Speed = 10f;
    private Damage Damage => new Damage.Physical(10, new DamageSource.TODO());

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
            enemy.TakeDamage(Damage);
        Destroy(gameObject);
    }
}
