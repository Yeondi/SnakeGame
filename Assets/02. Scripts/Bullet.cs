using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject Owner { get; private set; }
    public int Damage { get; private set; }

    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;
    
    private PlayerController playerController;
    private EnemyController enemyController;

    public void Init(GameObject owner, int damage)
    {
        Owner = owner;
        Damage = damage;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (rb != null)
        {
            // 캡슐 모델은 X축으로 90도 -> up으로 발싸!
            Vector3 forwardDirection = transform.up;
            rb.velocity = forwardDirection * speed;
        
            // 디버깅
            Debug.DrawRay(transform.position, forwardDirection * 5f, Color.green, 3f);
        }
        // Destroy(gameObject, lifeTime);
        StartCoroutine(RemoveAfterFewSeconds());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Owner) return; // 자해 방지

        if (Owner.CompareTag("Enemy") && other.CompareTag("Player"))
        {
            if (playerController == null) 
                playerController = other.GetComponent<PlayerController>();
        
            if (playerController != null)
                playerController.TakeDamage(Damage);
        }
        else if (Owner.CompareTag("Player") && other.CompareTag("Enemy"))
        {
            if (enemyController == null)
                enemyController = other.GetComponent<EnemyController>();
            
            if (enemyController != null)
                enemyController.TakeDamage(Damage);
        }

        //Destroy(gameObject); 
        GameManager.Instance.PoolManager.ReturnToPool(gameObject);
    }
    
    private IEnumerator RemoveAfterFewSeconds()
    {
        yield return new WaitForSeconds(lifeTime);
        GameManager.Instance.PoolManager.ReturnToPool(gameObject);
    }
}