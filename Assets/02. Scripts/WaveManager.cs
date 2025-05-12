using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private float waveInterval = 10f; // 웨이브 간 텀
    [SerializeField] private float spawnBurstInterval = 0.1f; // 한 웨이브 안에서 간격

    public int CurrentWave { get; private set; } = 0;
    public bool IsSpawning { get; private set; } = false;

    public void StartFirstWave()
    {
        Debug.Log("웨이브 시작!");
        CurrentWave = 1;
        StartCoroutine(WaveLoop());
    }

    private IEnumerator WaveLoop()
    {
        while (true)
        {
            Debug.Log($"▶ 웨이브 {CurrentWave} 시작");
            IsSpawning = true;

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnBurstInterval); // 다다다닥 느낌
            }

            IsSpawning = false;

            Debug.Log($"⏸ 웨이브 {CurrentWave} 종료, 다음 웨이브까지 {waveInterval}초");
            yield return new WaitForSeconds(waveInterval);
            CurrentWave++;
        }
    }

    private void SpawnEnemy()
    {
        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector3 spawnPos = GetRandomSpawnPositionAroundPlayer();
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private Vector3 GetRandomSpawnPositionAroundPlayer()
    {
        Transform player = GameManager.Instance.PlayerManager.Player.transform;

        Vector2 dir2D = Random.insideUnitCircle.normalized;
        float distance = Random.Range(10f, 30f);
        Vector3 offset = new Vector3(dir2D.x, 0f, dir2D.y) * distance;

        Vector3 spawnPos = player.position + offset;

        spawnPos.y = player.position.y; // 땅 박히는 거 방지

        return spawnPos;
    }
}