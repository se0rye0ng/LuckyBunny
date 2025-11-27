using UnityEngine;

public class FallingItem : MonoBehaviour
{
    // [중요] 중복 실행 방지용 변수 (성공/실패 통합)
    private bool isFinished = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 이미 처리가 끝난 아이템이면 무조건 무시
        if (isFinished) return;

        // 1. 플레이어가 잡았을 때
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 플레이어가 쌓기 성공했다고 하면
                if (player.AddItemToStack(this.gameObject))
                {
                    isFinished = true; // [중요] 처리 완료 플래그 세우기

                    // 물리 끄기
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null) { 
                        rb.linearVelocity = Vector2.zero; 
                        rb.isKinematic = true; 
                        rb.simulated = false; 
                    }
                    BoxCollider2D col = GetComponent<BoxCollider2D>();
                    if (col != null) col.enabled = false;

                    // 스포너에게 "성공" 보고 (딱 한 번만 실행됨)
                    FindObjectOfType<Spawner>().RequestNextItem(true);
                }
            }
        }
        // 2. 바닥에 닿았을 때 (놓침)
        else if (other.CompareTag("Finish"))
        {
            isFinished = true; // [중요] 처리 완료 플래그 세우기

            // 스포너에게 "실패" 보고
            FindObjectOfType<Spawner>().RequestNextItem(false);

            Destroy(gameObject);
        }
    }
}