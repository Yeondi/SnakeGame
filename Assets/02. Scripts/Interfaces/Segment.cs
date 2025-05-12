using UnityEngine;

public interface Segment
{
    /// <summary>
    /// 세그먼트 위치 업데이트
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    /// <param name="targetRotation">목표 회전</param>
    void UpdatePosition(Vector3 targetPosition, Quaternion targetRotation);
    
    /// <summary>
    /// 세그먼트 타입 반환
    /// </summary>
    /// <returns>SegmentType 열거형 값</returns>
    SegmentType GetSegmentType();
    
    /// <summary>
    /// 세그먼트 인덱스 반환 (머리로부터의 거리)
    /// </summary>
    /// <returns>인덱스 값</returns>
    int GetIndex();
    
    /// <summary>
    /// 세그먼트 특수 효과 활성화
    /// </summary>
    /// <param name="activator">효과를 활성화한 GameObject</param>
    void ActivateEffect(GameObject activator);
    
    /// <summary>
    /// 세그먼트 시각 효과 설정
    /// </summary>
    /// <param name="visualType">시각 효과 타입</param>
    void SetVisualEffect(SegmentVisualEffectType visualType);
}

public enum SegmentType
{
    Normal,
    Shield,
    Power,
    Speed,
    Magnet,
    Explosive
}

public enum SegmentVisualEffectType
{
    None,
    Highlight,
    Damaged,
    PoweredUp,
    Shielded
}