using UnityEngine;

public interface Lootable
{
    /// <summary>
    /// 루트 아이템 생성
    /// </summary>
    /// <param name="position">생성 위치</param>
    /// <param name="rotation">생성 회전</param>
    /// <returns>생성된 루트 아이템</returns>
    GameObject SpawnLoot(Vector3 position, Quaternion rotation);
    
    /// <summary>
    /// 루트 확률 반환
    /// </summary>
    /// <returns>0-1 사이의 확률</returns>
    float GetDropChance();
    
    /// <summary>
    /// 루트 타입 반환
    /// </summary>
    /// <returns>LootType 열거형 값</returns>
    LootType GetLootType();
    
    /// <summary>
    /// 루트 값 또는 강도 반환
    /// </summary>
    /// <returns>루트 값</returns>
    float GetLootValue();
}

public enum LootType
{
    Coin,
    Health,
    Ammo,
    Weapon,
    PowerUp,
    Segment
}