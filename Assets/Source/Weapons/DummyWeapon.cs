using UnityEngine;

class DummyWeapon : Weapon
{
    public override string SpriteAddress => "Weapons/Dummy";
    public override float Cooldown => 0.5f;

    protected override void OnFire(Vector2 direction)
    {
        var go = new GameObject("DummyProjectile");
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        go.AddComponent<DummyProjectile>();
    }
}
