using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [Header("Goal Settings")]
    [Tooltip("Number of clovers required to proceed to Level2")]
    public int requiredClovers = 10;

    // 다중 트리거 방지
    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        // 콜라이더 또는 부모에서 OPlayerController를 찾아 처리 (태그에 의존하는 것보다 안정적임)
        var playerController = other.GetComponentInParent<OPlayerController>();
        if (playerController == null)
        {
            // 플레이어가 아님 — 디버깅용 로그(많이 찍힐 수 있음)
            Debug.Log($"Goal triggered by non-player object: {other.gameObject.name}");
            return;
        }

        int current = playerController.CloverCount;
        if (current >= requiredClovers)
        {
            triggered = true;
            Debug.Log($"Goal reached: {current}/{requiredClovers} clovers — loading Level2...");
            SceneManager.LoadScene("Level2");
        }
        else
        {
            Debug.Log($"Player reached goal but has only {current}/{requiredClovers} clovers. Cannot proceed.");
            // 플레이어가 통과하지 못하도록: 뒤로 밀고 잠시 이동을 비활성화
            var playerRb = playerController.GetComponent<Rigidbody2D>();
            // compute push direction from goal to player
            // 골에서 플레이어 쪽으로의 밀어낼 방향 계산
            Vector2 pushDir = (other.transform.position - transform.position);
            playerController.PreventPassage(pushDir, 0.5f, 0.5f);
            // 선택 사항: 소리 재생이나 UI 표시 등
        }
    }
}
