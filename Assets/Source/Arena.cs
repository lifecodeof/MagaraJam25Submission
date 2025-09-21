using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

class Arena : MonoBehaviour
{
    [field: SerializeField]
    public Vector2 Extents { get; private set; }

    [field: SerializeField]
    public Vector2 Center { get; private set; }

    [field: SerializeField]
    private List<Enemy> enemies;
    public IReadOnlyList<Enemy> Enemies => enemies;
    private Dictionary<Enemy, IDisposable> enemySubscriptions = new();

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Center, Extents * 2f);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            enemySubscriptions[enemy] = enemy.Health.IsDead
                .WhereTrue()
                .Subscribe(_ => UnregisterEnemy(enemy));
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            if (enemySubscriptions.TryGetValue(enemy, out var subscription))
            {
                subscription.Dispose();
                enemySubscriptions.Remove(enemy);
            }
        }
    }
}
