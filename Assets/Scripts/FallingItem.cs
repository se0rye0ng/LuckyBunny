using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public int colorIndex = -1; 
    private bool isFinished = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetColorInfo(int index)
    {
        colorIndex = index;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isFinished) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                bool isBad = gameObject.CompareTag("Enemy");
                
                // 플레이어에게 처리 결과 물어보기
                int result = player.AddItemToStack(this.gameObject, isBad, colorIndex);

                if (result == 1) // [성공] 먹어야 하는 색
                {
                    isFinished = true;
                    Destroy(gameObject); // 냠냠 (삭제)
                }
                else // [실패] 순서 틀림 or 나쁜 아이템
                {
                    // 튕겨나가는 효과 실행!
                    BounceOff();
                }
            }
        }
        else if (other.CompareTag("Finish")) // 바닥에 닿음
        {
            Destroy(gameObject);
        }
    }

    // [핵심] 튕겨나가는 함수
    void BounceOff()
    {
        isFinished = true; // 더 이상 충돌 처리 안 함

        // 1. 플레이어와 또 부딪히지 않게 콜라이더 끄기
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. 물리력 적용 (튕겨나가기)
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // 물리 적용 켜기
            rb.gravityScale = 3.0f; // 빨리 떨어지게 중력 강화
            
            // 랜덤한 방향(위쪽 + 좌/우)으로 힘 가하기
            float randomX = Random.Range(-1f, 1f); 
            Vector2 bounceForce = new Vector2(randomX, 1f).normalized * 10.0f; // 10.0f는 튕기는 세기
            
            // Unity 6 최신 문법 (구버전이면 rb.velocity = bounceForce; 사용)
            rb.linearVelocity = bounceForce; 
            
            // 회전 효과 추가 (빙글빙글)
            rb.angularVelocity = Random.Range(-360f, 360f);
        }

        // 3초 뒤에 삭제 (메모리 정리)
        Destroy(gameObject, 3.0f);
    }
}