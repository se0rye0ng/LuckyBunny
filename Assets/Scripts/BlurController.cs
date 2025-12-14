using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlurController : MonoBehaviour
{
    [Header("Overlay Objects List")]
    public List<GameObject> blurOverlays = new List<GameObject>();

    [Header("Settings")]
    public float fadeInDuration = 0f; 
    public float fadeOutDuration = 1.0f; 
    
    [Range(0f, 1f)]
    public float maxAlpha = 0.95f; // 투명도 조절 (여기서 숫자를 바꾸면 됩니다)

    // 여러 개의 코루틴을 관리하기 위한 딕셔너리
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    
    public void ApplyBlur(int index, float duration = 5f)
    {
        if (blurOverlays == null || index < 0 || index >= blurOverlays.Count) return;

        GameObject targetOverlay = blurOverlays[index];
        if (targetOverlay == null) return;

        // 이미 실행 중인 게 있으면 끄고 새로 시작 (갱신)
        if (activeCoroutines.ContainsKey(targetOverlay))
        {
            if (activeCoroutines[targetOverlay] != null)
                StopCoroutine(activeCoroutines[targetOverlay]);
            
            activeCoroutines.Remove(targetOverlay);
        }

        // 새 코루틴 시작 및 등록
        Coroutine newCoroutine = StartCoroutine(OverlayCoroutine(targetOverlay, fadeInDuration, fadeOutDuration, duration));
        activeCoroutines.Add(targetOverlay, newCoroutine);
    }

    // [수정됨] 화이트 클로버용 "모두 멈춰!" (Dictionary 버전)
    public void StopBlur()
    {
        // 1. 딕셔너리에 기록된 모든 코루틴 정지
        foreach (var entry in activeCoroutines)
        {
            // 실행 중인 코루틴 멈춤
            if (entry.Value != null) StopCoroutine(entry.Value);
            
            // 오버레이 이미지 끄기
            if (entry.Key != null)
            {
                SetAlpha(entry.Key, 0f);
                entry.Key.SetActive(false);
            }
        }

        // 2. 명단 초기화
        activeCoroutines.Clear();
        Debug.Log("BlurController: 모든 블러 효과가 해제되었습니다.");
    }

    IEnumerator OverlayCoroutine(GameObject target, float inTime, float outTime, float holdTime)
    {
        target.SetActive(true);

        // --- Fade In ---
        if (inTime <= 0f) SetAlpha(target, maxAlpha);
        else
        {
            float startAlpha = GetAlpha(target);
            float t = 0f;
            while (t < inTime)
            {
                t += Time.deltaTime;
                // maxAlpha(0.95)까지만 변함
                SetAlpha(target, Mathf.Lerp(startAlpha, maxAlpha, Mathf.Clamp01(t / inTime)));
                yield return null;
            }
            SetAlpha(target, maxAlpha);
        }

        // --- Hold ---
        yield return new WaitForSeconds(holdTime);

        // --- Fade Out ---
        float fadeT = 0f;
        while (fadeT < outTime)
        {
            fadeT += Time.deltaTime;
            // [중요 수정] 1f가 아니라 maxAlpha(0.95)에서부터 줄어들어야 깜빡이지 않음!
            SetAlpha(target, Mathf.Lerp(maxAlpha, 0f, Mathf.Clamp01(fadeT / outTime)));
            yield return null;
        }

        SetAlpha(target, 0f);
        target.SetActive(false);

        // 일이 다 끝나면 명단에서 삭제
        if (activeCoroutines.ContainsKey(target))
            activeCoroutines.Remove(target);
    }

    // --- 헬퍼 함수 ---
    void SetAlpha(GameObject go, float alpha)
    {
        if (go == null) return;
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) { var c = sr.color; c.a = alpha; sr.color = c; }
        var img = go.GetComponent<Image>();
        if (img != null) { var c = img.color; c.a = alpha; img.color = c; }
        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = alpha;
    }
    
    float GetAlpha(GameObject go)
    {
        if (go == null) return 0f;
        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.color.a;
        var img = go.GetComponent<Image>();
        if (img != null) return img.color.a;
        var cg = go.GetComponent<CanvasGroup>();
        if (cg != null) return cg.alpha;
        return 0f;
    }
}