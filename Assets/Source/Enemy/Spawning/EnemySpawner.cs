using System.Collections.Generic;
using UnityEngine;

class EnemySpawner : MonoBehaviour
{
    public int HardModeTreshold = 500;
    public float SpawnInterval = 3f;
    public int SpawnCount = 2;
    public int HpPer10Score = 10;

    public List<Enemy> EnemiesToSpawn = new();

    [field: SerializeField]
    public float LastSpawnTime { get; private set; } = 0f;

    private int score => psm?.Score.Value ?? 0;

    private Camera mainCamera;
    private PlayerStateManager psm;

    void Start()
    {
        mainCamera = Camera.main;
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

    private void SpawnEnemies(int count = 1)
    {
        foreach (var spawnPoint in GetRandomPointsOnCameraBorder(count))
        {
            var index = score > HardModeTreshold
                ? (Random.value > 0.2f ? 1 : 0)
                : 0;
            var enemyPrefab = EnemiesToSpawn[index];

            var enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            var health = enemy.GetComponent<Health>();
            health.Max.Value = health.Current.Value = Mathf.CeilToInt((1 + score) / 10f) * HpPer10Score;
        }
    }
}
