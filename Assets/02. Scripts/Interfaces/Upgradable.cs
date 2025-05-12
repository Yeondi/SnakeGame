public interface Upgradable
{
    /// <summary>
    /// 업그레이드 실행
    /// </summary>
    /// <returns>업그레이드 성공 여부</returns>
    bool Upgrade();
    
    /// <summary>
    /// 현재 레벨 반환
    /// </summary>
    /// <returns>현재 레벨</returns>
    int GetLevel();
    
    /// <summary>
    /// 최대 레벨 반환
    /// </summary>
    /// <returns>최대 레벨</returns>
    int GetMaxLevel();
    
    /// <summary>
    /// 다음 업그레이드 비용 반환
    /// </summary>
    /// <returns>비용</returns>
    int GetUpgradeCost();
    
    /// <summary>
    /// 업그레이드 가능 여부 확인
    /// </summary>
    /// <returns>업그레이드 가능 여부</returns>
    bool CanUpgrade();
    
    /// <summary>
    /// 현재 업그레이드 단계에서의 스탯 반환
    /// </summary>
    /// <returns>업그레이드 스탯 정보</returns>
    UpgradeStats GetCurrentStats();
}

public struct UpgradeStats
{
    public float damageMultiplier;
    public float speedMultiplier;
    public float rangeMultiplier;
    public float durabilityMultiplier;
    public float cooldownReduction;
    public string specialEffect;
}