// PlayerController.cs
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : BaseCharacter
{
    #region Events
    public event Action OnDeath;
    #endregion

    #region Movement Settings
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private DynamicJoystick joystick;
    [SerializeField] private GameObject joystickPrefab;
    private Rigidbody rb;
    private const float RotationSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f; 
    #endregion

    #region Combat Settings
    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // 대충 허리 ~ 팔 위치, 팔이 있다고 가정.
    [SerializeField] private float fireInterval = 0.5f;
    private Transform currentTarget;
    private const float TargetingInterval = 0.2f;
    private const float TargetingRange = 20f;
    #endregion

    #region Weapons
    [Header("Weapons")]
    [SerializeField] private TMP_Text weaponTypeText;
    private WeaponType currentWeaponType = WeaponType.Pistol;
    private Dictionary<WeaponType, WeaponData> weaponDataMap = new Dictionary<WeaponType, WeaponData>();
    private Coroutine fireCoroutine;
    private Coroutine targetingCoroutine;
    #endregion
    
    #region Squadmembers
    [Header("Squadmembers")]
    [SerializeField] private GameObject squadMemberPrefab;
    [SerializeField] private int maxSquadMembers = 5;
    private List<GameObject> squadMembers = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    private int positionHistoryMaxCount = 50;
    private float memberFollowDistance = 1.5f;

    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHP = maxHP;
        damage = 10;
        
        InitializeWeaponData();
    }
    
    private void Start()
    {
        targetingCoroutine = StartCoroutine(UpdateTargeting()); // 적찾기
        fireCoroutine = StartCoroutine(AutoFire()); // 일단 발사
        
        Equip(currentWeaponType);
    }
    
    private void Update() // 이동 및 타겟 바라보기만 동기 / 이외는 비동기식
    {
        HandleMovement();
        FaceTarget();
        
        UpdatePositionHistory();
    }
    
    #endregion

    #region Movement
    private void HandleMovement()
    {
        EnsureJoystickExists();
        Vector3 moveDir = CalculateMoveDirection();
        rb.velocity = moveDir * moveSpeed;
    }
    
    private void EnsureJoystickExists()
    {
        if (joystick != null) return;
        
        GameObject canvas = GameObject.Find("UI_Canvas");
        GameObject go = Instantiate(joystickPrefab, transform.position, Quaternion.identity, canvas.transform);
        ConfigureJoystickRectTransform(go.GetComponent<RectTransform>());
        joystick = go.GetComponent<DynamicJoystick>();
    }
    
    private void ConfigureJoystickRectTransform(RectTransform rt)
    {
        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
    
    private Vector3 CalculateMoveDirection()
    {
        Vector3 moveDir = Vector3.zero;
        
#if UNITY_EDITOR
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
    
        if (horizontal != 0 || vertical != 0)
        {
            moveDir = new Vector3(horizontal, 0f, vertical).normalized;
        }
        else
        {
            moveDir = new Vector3(joystick.Direction.x, 0f, joystick.Direction.y).normalized;
        }
#else
        moveDir = new Vector3(joystick.Direction.x, 0f, joystick.Direction.y).normalized;
#endif

        return moveDir;
    }
    
    // 타겟을 향해 회전하는 메서드
    private void FaceTarget()
    {
        Vector3 desiredLookDirection;
    
        if (currentTarget != null)
        {
            // 타겟이 있으면 타겟 방향을 바라봄
            desiredLookDirection = currentTarget.position - transform.position;
        }
        else
        {
            // 타겟이 없으면 이동 방향을 바라봄
            Vector3 moveDir = CalculateMoveDirection();
        
            if (moveDir.magnitude < 0.1f)  // 움직이지 않으면 현재 방향 유지
                return;
            
            desiredLookDirection = moveDir;
        }
    
        // y축 회전만 적용
        desiredLookDirection.y = 0f;
    
        if (desiredLookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredLookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    // 위치 기록 업데이트 함수
    private void UpdatePositionHistory()
    {
        // 현재 위치 기록에 추가
        positionHistory.Insert(0, transform.position);
    
        // 최대 기록 수를 초과하면 오래된 기록 제거
        if (positionHistory.Count > positionHistoryMaxCount)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
    }
    
    public List<Vector3> GetPositionHistory()
    {
        return positionHistory;
    }
    #endregion

    #region Combat
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
    
    private void StartWeaponSystems()
    {
        fireCoroutine = StartCoroutine(AutoFire());
        targetingCoroutine = StartCoroutine(UpdateTargeting());
    }
    
    private IEnumerator UpdateTargeting()
    {
        while (true)
        {
            FindNearestEnemy();
            yield return new WaitForSeconds(TargetingInterval);
        }
    }
    
    private void FindNearestEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, TargetingRange);
        float closestDistance = float.MaxValue;
        Transform closestEnemy = null;
    
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.transform;
                }
            }
        }
    
        currentTarget = closestEnemy;
    }
    #endregion

    #region Weapon Systems
    private IEnumerator AutoFire()
    {
        while (true)
        {
            WeaponData weaponData = weaponDataMap[currentWeaponType];
            yield return new WaitForSeconds(weaponData.fireInterval);
            
            if (bulletPrefab != null && firePoint != null)
            {
                FireWeapon();
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
            // 타겟이 없을 때는 입력 방향 사용|
            Vector3 inputDir = CalculateMoveDirection();
            if (inputDir.magnitude < 0.1f)
            {
                return transform.forward;
            }
        
            return inputDir;
        }
        else
        {
            // 타겟 있을때
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
    
        // 타겟이 없을 경우 플레이어의 실제 forward 방향 사용
        Vector3 shootDirection = (currentTarget == null) ? transform.forward : direction;
    
        // 중요: 총알의 회전을 플레이어의 forward 방향과 정확히 일치시킴
        bullet.transform.rotation = Quaternion.LookRotation(shootDirection) * Quaternion.Euler(90, 0, 0);
    
        // 디버깅
        Debug.DrawRay(firePoint.position, shootDirection * 10f, Color.red, 1f);
    
        int minDamage = (int)(weaponDamage * 0.5f);
        int maxDamage = weaponDamage;
    
        int randomDamage = Random.Range(minDamage, maxDamage);
        bullet.GetComponent<Bullet>().Init(gameObject, randomDamage);
    }
    
    private void InitializeWeaponData()
    {
        weaponDataMap[WeaponType.Pistol] = new WeaponData
        {
            fireInterval = 0.5f,
            damage = 10,
            bulletCount = 1,
            spreadAngle = 0f
        };
        
        weaponDataMap[WeaponType.Shotgun] = new WeaponData
        {
            fireInterval = 1.2f,
            damage = 6,
            bulletCount = 5,
            spreadAngle = 25f
        };
    }
    #endregion

    #region Input Handling & Cheat Section
    public void OnSwitchToPistol(InputAction.CallbackContext context)
    {
        if (context.performed)
            Equip(WeaponType.Pistol);
    }
    
    public void OnSwitchToShotgun(InputAction.CallbackContext context)
    {
        if (context.performed)
            Equip(WeaponType.Shotgun);
    }

    public void OnAddSquadMember(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (squadMembers.Count >= maxSquadMembers)
            {
                Debug.Log("최대 분대 멤버 수에 도달했습니다.");
                return;
            }

            Vector3 spawnPosition = transform.position - transform.forward * 1.5f;
            GameObject newMember = Instantiate(squadMemberPrefab, spawnPosition, Quaternion.identity);
        
            SquadMember squadMember = newMember.GetComponent<SquadMember>();
            if (squadMember != null)
            {
                // 무기 타입 설정 (첫 번째는 피스톨, 두 번째는 샷건)
                WeaponType weaponType = (squadMembers.Count % 2 == 0) ? 
                    WeaponType.Pistol : WeaponType.Shotgun;
                squadMember.Initialize(weaponType, squadMembers.Count + 1);
            }
        
            squadMembers.Add(newMember);
        }
    }
    #endregion
    
    public void Equip(WeaponType weapon)
    {
        if (weaponTypeText == null)
            weaponTypeText = GameObject.Find("CurrentWeapon").GetComponent<TMP_Text>();
        
        weaponTypeText.text = "CurrentWeapon : " + weapon.ToString();
        currentWeaponType = weapon;
        
        RestartFireCoroutine();
    }
    
    private void RestartFireCoroutine()
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }
        fireCoroutine = StartCoroutine(AutoFire());
    }
}