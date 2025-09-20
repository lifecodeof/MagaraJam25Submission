using UnityEngine;

class DummyProjectile : Projectile
{
    public float Lifetime = 5f;
    public float Speed = 10f;
    private Damage Damage => new(10);

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
        Lifetime -= Time.deltaTime;
        if (Lifetime <= 0f) Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
            enemy.TakeDamage(Damage);
        Destroy(gameObject);
    }
}
