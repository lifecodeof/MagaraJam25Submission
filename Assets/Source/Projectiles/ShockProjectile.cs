using System.Collections.Generic;
using UnityEngine;

class ShockProjectile : Projectile
{
    public float Lifetime = 10f;
    public float Speed = 15f;
    public int DamageAmount = 1;

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(new Damage(DamageAmount));
            Destroy(gameObject);
        }
    }
}
