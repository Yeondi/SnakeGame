public interface GameEventListener
{
    /// <summary>
    /// 웨이브 시작 이벤트
    /// </summary>
    /// <param name="waveNumber">웨이브 번호</param>
    void OnWaveStart(int waveNumber);
    
    /// <summary>
    /// 웨이브 종료 이벤트
    /// </summary>
    /// <param name="waveNumber">웨이브 번호</param>
    /// <param name="success">성공 여부</param>
    void OnWaveEnd(int waveNumber, bool success);
    
    /// <summary>
    /// 게임 오버 이벤트
    /// </summary>
    /// <param name="victory">승리 여부</param>
    void OnGameOver(bool victory);
    
    /// <summary>
    /// 점수 변경 이벤트
    /// </summary>
    /// <param name="newScore">새 점수</param>
    void OnScoreChanged(int newScore);
    
    /// <summary>
    /// 플레이어 상태 변경 이벤트
    /// </summary>
    /// <param name="health">현재 체력</param>
    /// <param name="maxHealth">최대 체력</param>
    /// <param name="segments">세그먼트 수</param>
    void OnPlayerStatusChanged(float health, float maxHealth, int segments);
}