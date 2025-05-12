using System.Collections;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public TMP_Text text;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(int damage, Vector3 worldPosition)
    {
        // 텍스트 설정
        text.text = damage.ToString();
        
        // 월드 좌표를 스크린 좌표로 변환
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition + Vector3.up);
        
        // RectTransform 위치 설정 (오버레이 캔버스용)
        rectTransform.position = screenPosition;
        
        // 애니메이션 시작
        StartCoroutine(FadeOutAndMove());
    }

    private IEnumerator FadeOutAndMove()
    {
        float duration = 2f;
        float elapsed = 0f;
        
        // 시작 위치 기록
        Vector2 startPosition = rectTransform.position;
        // 종료 위치 계산 (위로 100픽셀 이동)
        Vector2 endPosition = startPosition + new Vector2(0, 100);
        
        // 색상 설정
        Color startColor = text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 부드러운 이지 아웃 애니메이션
            float easedT = 1 - Mathf.Pow(1 - t, 3);
            
            // 위치 및 색상 보간
            rectTransform.position = Vector2.Lerp(startPosition, endPosition, easedT);
            text.color = Color.Lerp(startColor, endColor, t);
            
            yield return null;
        }
        
        // 오브젝트 풀로 반환
        if (GameManager.Instance != null && GameManager.Instance.PoolManager != null)
        {
            gameObject.SetActive(false);
            GameManager.Instance.PoolManager.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}