public interface Poolable
{
    /// <summary>
    /// 풀에서 오브젝트가 스폰될 때 호출
    /// </summary>
    void OnSpawn();
    
    /// <summary>
    /// 풀로 오브젝트가 반환될 때 호출
    /// </summary>
    void OnDespawn();
    
    /// <summary>
    /// 현재 오브젝트의 활성화 상태 반환
    /// </summary>
    /// <returns>활성화 상태</returns>
    bool IsActive();
    
    /// <summary>
    /// 오브젝트를 식별하기 위한 ID 반환
    /// </summary>
    string GetPoolableId();
}