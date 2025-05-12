using UnityEngine;

public interface EnemyAI
{
    /// <summary>
    /// AI의 타겟 설정
    /// </summary>
    /// <param name="target">타겟 Transform</param>
    void SetTarget(Transform target);
    
    /// <summary>
    /// AI 로직 업데이트
    /// </summary>
    void UpdateAI();
    
    /// <summary>
    /// 현재 AI 상태 반환
    /// </summary>
    /// <returns>EnemyState 열거형 값</returns>
    EnemyState GetState();
    
    /// <summary>
    /// AI 활성화/비활성화 설정
    /// </summary>
    /// <param name="active">활성화 여부</param>
    void SetActive(bool active);
    
    /// <summary>
    /// 타겟까지의 거리 반환
    /// </summary>
    float GetDistanceToTarget();
}

public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Retreating,
    Stunned,
    Dead
}