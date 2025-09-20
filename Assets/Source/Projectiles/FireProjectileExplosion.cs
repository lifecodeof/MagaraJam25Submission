using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

class FireProjectileExplosion : MonoBehaviour
{
    public int DamageAmount = 10;

    [SerializeField]
    private float MaxRadius = 10f;

    [SerializeField]
    private float ExpansionSpeed = 20f;

    [SerializeField, ReadOnly]
    private float currentRadius = 0f;

    [SerializeField, ReadOnly]
    private List<Enemy> affectedEnemies = new();

    void Update()
    {
        currentRadius += ExpansionSpeed * Time.deltaTime;
        transform.localScale = Vector3.one * currentRadius;

        if (currentRadius >= MaxRadius)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            if (!affectedEnemies.Contains(enemy))
            {
                affectedEnemies.Add(enemy);
                enemy.TakeDamage(new Damage(DamageAmount));
            }
        }
    }
}
