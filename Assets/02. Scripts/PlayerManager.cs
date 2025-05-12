using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    // [SerializeField] private Transform spawnPoint;

    public PlayerController Player { get; private set; }

    public void SpawnPlayer()
    {
        if (Player != null)
        {
            Destroy(Player.gameObject);
        }

        var go = Instantiate(playerPrefab, new Vector3(0f,4.5f,0f), Quaternion.identity);
        Player = go.GetComponent<PlayerController>();
        Player.OnDeath += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        GameManager.Instance.EndGame(false);
    }
} 