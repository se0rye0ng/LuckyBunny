using UnityEngine;

public class OPlayerController : MonoBehaviour
{
    // 이동 속도 (Inspector에서 조절 가능)
        [Header("이동 설정")]
    public float moveSpeed = 5.0f;
    
    [Header("점프 설정")]  // 새로 추가!
    public float jumpForce = 10.0f;  // 점프 힘

    private Rigidbody2D rb;

    void Update()
    {
        // 입력 감지
        float moveX = 0f;
        
        if (Input.GetKey(KeyCode.A))
        {
            moveX = -1f;  // 왼쪽
            Debug.Log("왼쪽으로 이동 중!");
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX = 1f;   // 오른쪽
            Debug.Log("오른쪽으로 이동 중!");
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
            Debug.Log("아래쪽으로 이동 중!");
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            Debug.Log("위쪽으로 이동 중!");
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("OPlayerController: Rigidbody2D가 없습니다. 물리 기반 이동/점프가 작동하지 않습니다.");
        }
    }
}
