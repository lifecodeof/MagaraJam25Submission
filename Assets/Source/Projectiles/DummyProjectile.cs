using UnityEngine;

class DummyProjectile : Projectile
{
    public float Speed = 10f;

    public override string SpriteAddress => "Projectiles/Dummy";

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector2.up);
    }
}
