using System.Collections.Generic;

public interface SaveData
{
    /// <summary>
    /// 저장 데이터 ID 반환
    /// </summary>
    string GetSaveId();
    
    /// <summary>
    /// 저장 데이터를 Dictionary로 직렬화
    /// </summary>
    /// <returns>직렬화된 데이터</returns>
    Dictionary<string, object> Serialize();
    
    /// <summary>
    /// Dictionary에서 데이터 역직렬화
    /// </summary>
    /// <param name="data">직렬화된 데이터</param>
    void Deserialize(Dictionary<string, object> data);
    
    /// <summary>
    /// 저장 시간 반환
    /// </summary>
    System.DateTime GetSaveTime();
}