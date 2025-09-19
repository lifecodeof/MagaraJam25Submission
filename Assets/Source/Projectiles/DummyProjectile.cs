using UnityEngine;

class DummyProjectile : Projectile
{
    public float Speed = 10f;

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.right);
    }
}
