using UnityEngine;

class AirWeapon : Weapon
{
    public override float Cooldown => 0.5f;

    protected override void OnFire(Quaternion direction)
    {
        throw new System.NotImplementedException();
    }
}
