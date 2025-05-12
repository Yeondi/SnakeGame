using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadMember : BaseCharacter
{
    [Header("Squadron Settings")]
    private WeaponType currentWeaponType;
    private int squadIndex;
    private Transform leader; // 플레이어

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float catchupMultiplier = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    private Rigidbody rb;
    
    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float targetingInterval = 0.2f;
    [SerializeField] private float targetingRange = 18f;
    private Transform currentTarget;
    private Coroutine fireCoroutine;
    private Coroutine targetingCoroutine;
    
    [Header("Weapon Data")]
    private Dictionary<WeaponType, WeaponData> weaponDataMap = new Dictionary<WeaponType, WeaponData>();
    
    // 이동 경로 추적을 위한 변수들
    private int targetHistoryIndex;
    private float minDistanceToTarget = 0.3f; // 목표 지점에 너무 가까이 다가가면 이동 중지
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InitializeWeaponData();
    }
    
    private void Start()
    {
        targetingCoroutine = StartCoroutine(UpdateTargeting());
        fireCoroutine = StartCoroutine(AutoFire());
    }
    
    private void FixedUpdate()
    {
        FollowTarget();
        FaceTarget();
    }
    
    public void Initialize(WeaponType weaponType, int squadMemberIndex)
    {
        currentWeaponType = weaponType;
        squadIndex = squadMemberIndex;
        
        // 플레이어가 무조건 리더
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            leader = player.transform;
        }
        
        currentHP = maxHP;
        damage = 8;
        
        // 위치 타겟 인덱스 설정
        targetHistoryIndex = squadIndex * 15; // 적절한 간격으로 조정
    }
    
    private void FollowTarget()
    {
        if (leader == null) return;
    
        PlayerController playerController = leader.GetComponent<PlayerController>();
        if (playerController == null) return;
    
        List<Vector3> positionHistory = playerController.GetPositionHistory();
        if (positionHistory.Count <= targetHistoryIndex) return;
    
        Vector3 targetPosition = positionHistory[targetHistoryIndex];
    
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
    
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
    
        if (distanceToTarget < minDistanceToTarget)
        {
            rb.velocity = Vector3.zero;
            return;
        }
    
        float speedMultiplier = 1.0f;
        if (distanceToTarget > 3.0f)
        {
            speedMultiplier = catchupMultiplier;
        }
    
        rb.velocity = directionToTarget * moveSpeed * speedMultiplier;
    
        if (directionToTarget != Vector3.zero && currentTarget == null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                Time.deltaTime * rotationSpeed);
        }
    }
    
    private void FaceTarget()
    {
        if (currentTarget != null)
        {
            Vector3 targetDirection = currentTarget.position - transform.position;
            targetDirection.y = 0f;
            
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                                                     Time.deltaTime * rotationSpeed);
            }
        }
    }
    
    private IEnumerator UpdateTargeting()
    {
        while (true)
        {
            FindNearestEnemy();
            yield return new WaitForSeconds(targetingInterval);
        }
    }
    
    private void FindNearestEnemy()
    {
        // 타겟 초기화
        Transform previousTarget = currentTarget;
        currentTarget = null;
    
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, targetingRange);
        float closestDistance = float.MaxValue;
    
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = hitCollider.transform;
                }
            }
        }
    
        // 디버깅
        if (currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.position, Color.red, targetingInterval);
        }
        else if (previousTarget != null)
        {
            // 타겟이 사라졌을 때 로그
            Debug.Log($"{squadIndex}번째 스쿼드멤버 타겟 사라짐");
        }
    }
    
    private IEnumerator AutoFire()
    {
        while (true)
        {
            if (currentTarget != null && weaponDataMap.ContainsKey(currentWeaponType))
            {
                WeaponData weaponData = weaponDataMap[currentWeaponType];
            
                // 타겟이 있을 때만 발사
                FireWeapon();
                yield return new WaitForSeconds(weaponData.fireInterval);
            }
            else
            {
                // 타겟이 없으면 기다림
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    private void FireWeapon()
    {
        WeaponData weaponData = weaponDataMap[currentWeaponType];
        Vector3 targetDirection = CalculateTargetDirection();
        
        switch (currentWeaponType)
        {
            case WeaponType.Pistol:
                FireBullet(targetDirection, weaponData.damage);
                break;
                
            case WeaponType.Shotgun:
                FireShotgun(targetDirection, weaponData);
                break;
        }
    }
    
    private Vector3 CalculateTargetDirection()
    {
        if (currentTarget == null)
        {
            return transform.forward;
        }
        else
        {
            return (currentTarget.position - firePoint.position).normalized;
        }
    }
    
    private void FireShotgun(Vector3 targetDirection, WeaponData weaponData)
    {
        for (int i = 0; i < weaponData.bulletCount; i++)
        {
            Vector3 spreadDirection = ApplySpread(targetDirection, weaponData.spreadAngle);
            FireBullet(spreadDirection, weaponData.damage);
        }
    }
    
    private Vector3 ApplySpread(Vector3 direction, float maxSpreadAngle)
    {
        float spreadX = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        float spreadY = Random.Range(-maxSpreadAngle, maxSpreadAngle);
        
        Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0);
        return spreadRotation * direction;
    }
    
    private void FireBullet(Vector3 direction, int weaponDamage)
    {
        GameObject bullet = GameManager.Instance.PoolManager.SpawnFromPool("Bullet", firePoint.position, Quaternion.identity);
        if (bullet == null)
        {
            Debug.LogWarning("탄창 비었음!");
            return;
        }
    
        // 회전 설정
        Quaternion lookRot = Quaternion.LookRotation(direction);
        Quaternion bulletRotation = lookRot * Quaternion.Euler(90, 0, 0);
        bullet.transform.rotation = bulletRotation;
    
        // 디버깅을 위해 총알의 up 벡터 방향을 표시
        Debug.DrawRay(bullet.transform.position, bullet.transform.up * 5f, Color.yellow, 1f);
    
        int minDamage = (int)(weaponDamage * 0.5f);
        int maxDamage = weaponDamage;
    
        int randomDamage = Random.Range(minDamage, maxDamage);
        bullet.GetComponent<Bullet>().Init(gameObject, randomDamage);
    }
    
    private void InitializeWeaponData()
    {
        weaponDataMap[WeaponType.Pistol] = new WeaponData
        {
            fireInterval = 0.6f,
            damage = 8,
            bulletCount = 1,
            spreadAngle = 0f
        };
        
        weaponDataMap[WeaponType.Shotgun] = new WeaponData
        {
            fireInterval = 1.3f,
            damage = 5,
            bulletCount = 5,
            spreadAngle = 30f
        };
    }
}