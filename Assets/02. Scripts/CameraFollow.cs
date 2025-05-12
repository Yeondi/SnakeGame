using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    // 카메라 셋업
    [SerializeField] private Vector3 offset = new Vector3(0f, 30f, -30f);
    [SerializeField] private Vector3 rotation = new Vector3(45f, 0f, 0f);
    
    [SerializeField] private float smoothSpeed = 0.125f;
    
    private void Start()
    {
        StartCoroutine(WaitForPlayerAssignment());
        
        transform.eulerAngles = rotation;
    }
    
    private void LateUpdate()
    {
        if (target == null) return;
        
        Vector3 desiredPosition = new Vector3(target.position.x, offset.y, target.position.z + offset.z);
        
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        transform.eulerAngles = rotation;
    }
    
    private IEnumerator WaitForPlayerAssignment()
    {
        // 플레이어가 할당될 때까지 대기
        while (GameManager.Instance == null || 
               GameManager.Instance.PlayerManager == null || 
               GameManager.Instance.PlayerManager.Player == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
    
        // 플레이어가 할당되었을 때 타겟 설정
        target = GameManager.Instance.PlayerManager.Player.transform;
    }

}