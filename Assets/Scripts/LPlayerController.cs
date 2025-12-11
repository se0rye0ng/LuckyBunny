using UnityEngine;

public class LPlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f; // 이동 속도
    [Tooltip("대각선 이동 시 속도 보정 여부 (true면 정규화하여 동일 속도 유지)")]
    public bool normalizeDiagonal = true;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("LPlayerController: SpriteRenderer가 없습니다. 캐릭터 뒤집기가 작동하지 않습니다.");
        }
    }

    void Update()
    {
        // WASD 입력 처리: W=위, A=왼쪽, S=아래, D=오른쪽
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) y = 1f;
        if (Input.GetKey(KeyCode.S)) y = -1f;

        Vector2 dir = new Vector2(x, y);
        if (dir.sqrMagnitude > 0f)
        {
            if (normalizeDiagonal && dir.sqrMagnitude > 1f)
            {
                dir = dir.normalized;
            }

            // Transform 기반 이동 (무중력/직접 이동 방식)
            transform.Translate((Vector3)dir * moveSpeed * Time.deltaTime, Space.World);

            // 좌우 방향에 따라 스프라이트 뒤집기
            if (spriteRenderer != null)
            {
                if (x < 0f) spriteRenderer.flipX = false; // 왼쪽
                else if (x > 0f) spriteRenderer.flipX = true; // 오른쪽
            }
        }
    }
}
