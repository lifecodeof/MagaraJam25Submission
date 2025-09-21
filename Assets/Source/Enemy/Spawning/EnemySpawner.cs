using System.Collections.Generic;
using NaughtyAttributes;
using R3;
using UnityEngine;

class EnemySpawner : MonoBehaviour
{
    public float SpawnIntervalBase = 5f;
    public float SpawnIntervalPerScore = -0.05f;
    [ShowNativeProperty]
    public float SpawnInterval => psm?.Score.Value is int score
        ? Mathf.Max(0.5f, SpawnIntervalBase + score * SpawnIntervalPerScore)
        : SpawnIntervalBase;

    public float SpawnCountBase = 1;
    public float SpawnCountPerScore = 0.1f;

    public List<Enemy> EnemiesToSpawn;

    [ShowNativeProperty]
    public int SpawnCount => psm?.Score.Value is int score
        ? Mathf.Max(1, Mathf.FloorToInt(SpawnCountBase + score * SpawnCountPerScore))
        : Mathf.FloorToInt(SpawnCountBase);

    [field: SerializeField]
    public float LastSpawnTime { get; private set; } = 0f;

    [SerializeField] private Enemy enemyPrefab;

    private Camera mainCamera;
    private Arena arena;
    private PlayerStateManager psm;

    void Start()
    {
        mainCamera = Camera.main;
        arena = Helpers.FindRequired<Arena>();
        psm = Helpers.FindRequired<PlayerStateManager>();
    }

    void Update()
    {
        if (Time.time - LastSpawnTime >= SpawnInterval)
        {
            SpawnEnemies(SpawnCount);
            LastSpawnTime = Time.time;
        }
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
