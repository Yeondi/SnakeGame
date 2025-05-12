using UnityEngine;

public interface Damageable
{
    /// <summary>
    /// 데미지를 받을 때 호출되는 메서드
    /// </summary>
    /// <param name="damage">받은 데미지 양</param>
    /// <param name="instigator">데미지를 입힌 GameObject</param>
    /// <returns>실제로 적용된 데미지</returns>
    float TakeDamage(float damage, GameObject instigator);
    
    /// <summary>
    /// 사망 처리를 위한 메서드
    /// </summary>
    void Die();
    
    /// <summary>
    /// 현재 체력을 반환
    /// </summary>
    float GetCurrentHealth();
    
    /// <summary>
    /// 최대 체력을 반환
    /// </summary>
    float GetMaxHealth();
}