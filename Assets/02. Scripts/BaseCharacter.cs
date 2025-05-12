using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BaseCharacter : MonoBehaviour, ICharacter
{
    [Header("Health")]
    public int maxHP = 100;
    protected int currentHP;
    public int damage{get; protected set;}
    public bool IsDead { get; protected set; }

    private Transform damageCanvas;
    
    protected virtual void Awake()
    {
        currentHP = maxHP;
        IsDead = false;
        damageCanvas = GameObject.Find("UI_Canvas")?.transform;
    }

    public virtual void TakeDamage(int amount)
    {
        if (IsDead) return;
        
        currentHP -= amount;
        
        DisplayDamageEffect(amount);
        
        if (currentHP <= 0) Die();
    }

    protected virtual void DisplayDamageEffect(int damageAmount)
    {
        // 오버레이 캔버스를 사용하므로 회전은 필요 없음
        GameObject damageText = GameManager.Instance.PoolManager.SpawnFromPool("DamageText", Vector3.zero, Quaternion.identity);
        if (damageText == null) return;
    
        // 캔버스의 자식으로 설정
        if (damageCanvas != null)
        {
            damageText.transform.SetParent(damageCanvas);
        }
    
        // 스케일 리셋 (캔버스 스케일에 영향받지 않도록)
        damageText.transform.localScale = Vector3.one;
    
        // 텍스트 초기화
        DamageText damageTextComponent = damageText.GetComponent<DamageText>();
        if (damageTextComponent != null)
        {
            damageTextComponent.Init(damageAmount, transform.position);
        
            // 텍스트 색상 설정 (선택적)
            if (damageText.TryGetComponent<TMP_Text>(out var textComponent))
            {
                textComponent.color = Color.red;
            }
        }
    }

    public virtual void Die()
    {
        IsDead = true;
        Destroy(gameObject);
    }
}