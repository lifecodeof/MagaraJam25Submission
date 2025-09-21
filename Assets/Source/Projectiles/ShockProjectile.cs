using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ShockProjectile : Projectile
{
    public float Lifetime = 10f;
    public float Speed = 15f;
    public int DamageAmount = 1;
    public float ChainRadius = 3f;
    public int MaxChains = 3;

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(new Damage(DamageAmount));

            if (MaxChains > 0)
            {
                var enemies = Physics2D.CircleCastAll(transform.position, ChainRadius, Vector2.zero)
                    .Select(hit => hit.collider.TryGetComponent<Enemy>(out var enemy) ? enemy : null)
                    .Where(e => e != null && e != enemy)
                    .OrderBy(e => Vector2.SqrMagnitude(e.transform.position - transform.position))
                    .Take(MaxChains)
                    .ToList();

                foreach (var nextEnemies in enemies)
                {
                    Vector2 direction = (nextEnemies.transform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    var chained = Instantiate(gameObject, transform.position, Quaternion.Euler(0, 0, angle));
                    chained.GetComponent<ShockProjectile>().MaxChains = MaxChains - 1;
                }
            }

            Destroy(gameObject);
        }
    }
}
