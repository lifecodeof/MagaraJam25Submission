using UnityEngine;

class DummyEnemy : Enemy
{
    protected override void OnTakeDamage(Damage damage)
    {
        throw new System.NotImplementedException();
    }
}
