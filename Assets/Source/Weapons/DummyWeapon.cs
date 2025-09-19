using NaughtyAttributes;
using UnityEngine;

class DummyWeapon : Weapon
{
    [SerializeField, Required]
    private GameObject projectilePrefab;

    public override float Cooldown => 0.5f;

    protected override void OnFire(Vector2 direction)
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.FromToRotation(Vector2.right, direction));
    }
}
