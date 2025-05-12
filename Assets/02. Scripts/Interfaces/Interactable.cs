using UnityEngine;

public interface Interactable
{
    /// <summary>
    /// 상호작용 실행 메서드
    /// </summary>
    /// <param name="interactor">상호작용을 시도하는 GameObject</param>
    /// <returns>상호작용 성공 여부</returns>
    bool Interact(GameObject interactor);
    
    /// <summary>
    /// 상호작용 가능 여부 확인
    /// </summary>
    /// <param name="interactor">상호작용을 시도하는 GameObject</param>
    /// <returns>상호작용 가능 여부</returns>
    bool CanInteract(GameObject interactor);
    
    /// <summary>
    /// 상호작용 가능 범위 반환
    /// </summary>
    float GetInteractionRange();
    
    /// <summary>
    /// 상호작용 아이콘 또는 텍스트 반환
    /// </summary>
    string GetInteractionPrompt();
}