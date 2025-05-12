using UnityEngine;

public interface Weapon
{
    /// <summary>
    /// 무기 발사 메서드
    /// </summary>
    /// <returns>발사 성공 여부</returns>
    bool Fire();
    
    /// <summary>
    /// 무기 재장전 메서드
    /// </summary>
    /// <returns>재장전 성공 여부</returns>
    bool Reload();
    
    /// <summary>
    /// 무기 발사 가능 여부 확인
    /// </summary>
    /// <returns>발사 가능 여부</returns>
    bool CanFire();
    
    /// <summary>
    /// 현재 탄약 상태 조회
    /// </summary>
    /// <returns>(현재 탄약, 최대 탄약) 튜플</returns>
    (int current, int max) GetAmmoStatus();
    
    /// <summary>
    /// 무기 데미지 반환
    /// </summary>
    float GetDamage();
    
    /// <summary>
    /// 무기 타입 반환
    /// </summary>
    WeaponType GetWeaponType();
}