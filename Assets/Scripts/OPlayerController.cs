using System.Collections;
using UnityEngine;

public class OPlayerController : MonoBehaviour
{
    
    // 이동 속도 (Inspector에서 조절 가능)
    [Header("이동 설정")]
    public float moveSpeed = 5.0f;
    
    [Header("점프 설정")]  // 새로 추가!
    public float jumpForce = 7.0f;  // 점프 힘

    private Rigidbody2D rb;
    private int clover = 0;
    // 외부에서 점수(클로버 수)를 읽을 수 있게 프로퍼티 추가
    public int CloverCount => clover;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition; // 리스폰용 시작위치
    private float baseMoveSpeed;
    private Coroutine speedBoostCoroutine;
    // 사이즈 부스트 필드
    public float sizeBoostMultiplier = 1.5f; // 1.5배 (50% 증가)
    public float sizeBoostDuration = 5f; // 초 단위, 0이면 영구
    private Vector3 originalScale;
    private Coroutine sizeBoostCoroutine;
    // 이동 제어
    private bool movementEnabled = true;
    private Coroutine movementBlockCoroutine;

    void Update()
    {
        // 입력 감지
        float moveX = 0f;

        if (!movementEnabled)
        {
            // 이동이 차단된 경우 수평 입력이나 flip 처리를 하지 않음
            // 단, 물리(중력/점프)는 계속 작동하도록 허용함
            if (rb != null)
            {
                // 차단 중에는 수평 속도가 0이 되도록 함
                var velLock = rb.linearVelocity;
                velLock.x = 0f;
                rb.linearVelocity = velLock;
            }
            return;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;  // 왼쪽
            spriteRenderer.flipX = false;
            //Debug.Log("왼쪽으로 이동 중!");
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;   // 오른쪽
            spriteRenderer.flipX = true;
            //Debug.Log("오른쪽으로 이동 중!");
        }

        // 물리 기반 이동
        if (rb != null)
        {
            var vel = rb.linearVelocity;
            vel.x = moveX * moveSpeed;
            rb.linearVelocity = vel;
        }
    
        // 점프 입력 처리 (새로 추가!)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rb != null)
            {
                var vel = rb.linearVelocity;
                vel.y = jumpForce;
                rb.linearVelocity = vel;
                Debug.Log("점프!");
            }
        }


        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            //Debug.Log("아래쪽으로 이동 중!");
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            //Debug.Log("위쪽으로 이동 중!");
        }

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Clover"))
        {
            clover++;  // 점수 증가
            Debug.Log("클로버 획득! 현재 점수: " + clover);
            Destroy(other.gameObject);  // 제거
        }

        if (other.CompareTag("Yellowclover"))
        {
            // 노란 클로버: 이동속도 일시 증가 (5초)
            ApplySpeedBoost(2.0f, 5f); // 1.5배, 5초
            Debug.Log("노란 클로버 획득! 임시 스피드 적용: " + moveSpeed);
            Destroy(other.gameObject);  // 제거
        }

        if (other.CompareTag("Purpleclover"))
        {
            // 보라 클로버: 캐릭터 사이즈 일시 증가
            ApplySizeBoost(sizeBoostMultiplier, sizeBoostDuration);
            Debug.Log("보라 클로버 획득! 임시 사이즈 적용: " + sizeBoostMultiplier);
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 장애물 충돌 감지 - 새로 추가!
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("⚠️ 장애물 충돌! 시작 지점으로 돌아갑니다.");
            
            // 시작 위치로 순간이동
            transform.position = startPosition;
            
            // 속도 초기화 (안 하면 계속 날아감)
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
        Debug.Log("시작 위치 저장: " + startPosition);
        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("OPlayerController: Rigidbody2D가 없습니다. 물리 기반 이동/점프가 작동하지 않습니다.");
        }
        baseMoveSpeed = moveSpeed;
        originalScale = transform.localScale;
    }

    void ApplySpeedBoost(float multiplier, float duration)
    {
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
            speedBoostCoroutine = null;
            // 복원 전에 baseMoveSpeed는 그대로 유지되므로 중첩일 때도 정상 동작
        }
        speedBoostCoroutine = StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        moveSpeed = baseMoveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseMoveSpeed;
        speedBoostCoroutine = null;
    }

    void ApplySizeBoost(float multiplier, float duration)
    {
        if (sizeBoostCoroutine != null)
        {
            StopCoroutine(sizeBoostCoroutine);
            sizeBoostCoroutine = null;
        }
        if (duration <= 0f)
        {
            // 영구(복원 없음)
            transform.localScale = originalScale * multiplier;
        }
        else
        {
            sizeBoostCoroutine = StartCoroutine(SizeBoostCoroutine(multiplier, duration));
        }
    }

    IEnumerator SizeBoostCoroutine(float multiplier, float duration)
    {
        transform.localScale = originalScale * multiplier;
        yield return new WaitForSeconds(duration);
        transform.localScale = originalScale;
        sizeBoostCoroutine = null;
    }

    // 플레이어가 통과하지 못하도록 뒤로 밀고 잠시 이동을 비활성화
    public void PreventPassage(Vector2 pushDirection, float pushDistance = 0.5f, float disableDuration = 0.5f)
    {
        if (movementBlockCoroutine != null)
        {
            StopCoroutine(movementBlockCoroutine);
            movementBlockCoroutine = null;
        }
        movementBlockCoroutine = StartCoroutine(PreventPassageCoroutine(pushDirection, pushDistance, disableDuration));
    }

    IEnumerator PreventPassageCoroutine(Vector2 pushDirection, float pushDistance, float disableDuration)
    {
        movementEnabled = false;
        // 플레이어를 골에서 멀어지게 밀어냄
        transform.position += (Vector3)(pushDirection.normalized * pushDistance);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        yield return new WaitForSeconds(disableDuration);
        movementEnabled = true;
        movementBlockCoroutine = null;
    }
}
