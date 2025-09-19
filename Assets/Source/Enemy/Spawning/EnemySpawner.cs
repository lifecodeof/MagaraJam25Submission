using System.Collections.Generic;
using R3;
using UnityEngine;

// TODO: Make spawn rate scale with time survived and player level

class EnemySpawner : MonoBehaviour
{
    public SerializableReactiveProperty<float> SpawnInterval = new(5f);
    public SerializableReactiveProperty<int> SpawnCount = new(1);

    [SerializeField] private Enemy enemyPrefab;

    private Camera mainCamera;
    private Arena arena;

    private void Start()
    {
        mainCamera = Camera.main;
        arena = Helpers.FindRequired<Arena>();

        SpawnEnemies(SpawnCount.Value);

        // Spawn enemies
        // Of course, this could be done with update function, but this handles every property change.
        SpawnInterval
            // For each interval change, create a new interval observable
            .Select(interval => Observable.Interval(System.TimeSpan.FromSeconds(interval)))
            .Switch() // Flatten the nested observables (so we can handle subscriptions and disposal for both at the same time)
            .Subscribe(_ => SpawnEnemies(SpawnCount.Value)) // Call SpawnEnemies on each interval tick
            .AddTo(this); // Dispose on destroy
    }

    private IEnumerable<Vector2> GetRandomPointsOnCameraBorder(int count)
    {
        var cameraHeight = mainCamera.orthographicSize * 2f;
        var cameraWidth = cameraHeight * mainCamera.aspect;
        var cameraPos = mainCamera.transform.position;

        var halfWidth = cameraWidth / 2f;
        var halfHeight = cameraHeight / 2f;

        for (int i = 0; i < count; i++)
        {
            int edge = Random.Range(0, 4);
            var spawnPoint = edge switch
            {
                // Top
                0 => new Vector2(Random.Range(-halfWidth, halfWidth), halfHeight),
                // Right
                1 => new Vector2(halfWidth, Random.Range(-halfHeight, halfHeight)),
                // Bottom
                2 => new Vector2(Random.Range(-halfWidth, halfWidth), -halfHeight),
                // Left
                _ => new Vector2(-halfWidth, Random.Range(-halfHeight, halfHeight)),
            };

            // Convert to world position
            yield return new Vector2(cameraPos.x + spawnPoint.x, cameraPos.y + spawnPoint.y);
        }
    }

    private Vector2 ClampToArenaBounds(Vector2 point)
    {
        var extents = arena.Extents;
        var center = arena.Center;

        var clampedX = Mathf.Clamp(point.x, center.x - extents.x, center.x + extents.x);
        var clampedY = Mathf.Clamp(point.y, center.y - extents.y, center.y + extents.y);

        return new Vector2(clampedX, clampedY);
    }

    private void SpawnEnemies(int count = 1)
    {
        foreach (var spawnPoint in GetRandomPointsOnCameraBorder(count))
        {
            var clamped = ClampToArenaBounds(spawnPoint);
            var enemy = Instantiate(enemyPrefab, clamped, Quaternion.identity);
            arena.RegisterEnemy(enemy);
        }
    }
}
