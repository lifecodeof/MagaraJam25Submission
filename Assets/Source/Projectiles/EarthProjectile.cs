using System.Collections.Generic;
using UnityEngine;

class EarthProjectile : Projectile
{
    public float Lifetime = 10f;
    public float Speed = 5f;
    public int DamageAmount = 15;
    public float DamageCooldown = 1f;

    // With timestamps
    private Dictionary<Enemy, float> affectedEnemies = new();

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            if (affectedEnemies.TryGetValue(enemy, out var lastHitTime))
            {
                if (Time.time - lastHitTime < DamageCooldown)
                {
                    return; // Still in cooldown
                }
            }

            enemy.TakeDamage(new Damage(DamageAmount));
            affectedEnemies[enemy] = Time.time;
        }
    }
}
