using UnityEngine;

class EarthWeapon : Weapon
{
    public override float Cooldown => 1f;

    protected override void OnFire(Quaternion direction)
    {
        throw new System.NotImplementedException();
    }
}
