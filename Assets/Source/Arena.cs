using System.Collections.Generic;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Center, Extents * 2f);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    /// <returns>nullable</returns>
    public Enemy FindClosestEnemy(Vector2 position)
    {
        Enemy closest = null;
        float closestDistSqr = float.MaxValue;

        foreach (var enemy in enemies)
        {
            var distSqr = (enemy.transform.position - (Vector3)position).sqrMagnitude;
            if (distSqr < closestDistSqr)
            {
                closest = enemy;
                closestDistSqr = distSqr;
            }
        }

        return closest;
    }
}
