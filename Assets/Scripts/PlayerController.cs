using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("빙판길 이동 설정")]
    public float maxSpeed = 5.0f;      
    public float acceleration = 20.0f; // 가속도 (빠릿하게)
    public float deceleration = 10.0f; // 감속도 (덜 미끄러지게)

    [Header("맵 제한 좌표")]
    public float minX = -8f, maxX = 8f;
    public float minY = -4.5f, maxY = 4.5f;
    
    [Header("텔레포트 위치 보정")]
    public float teleportOffsetX = 0f; 
    public float teleportOffsetY = 0.5f; 

    private Vector2 currentVelocity; 

    void Start() { }

    void Update()
    {
        // 1. 이동 로직
        float xInput = Input.GetAxisRaw("Horizontal"); 
        float yInput = Input.GetAxisRaw("Vertical");   

        Vector2 inputDir = new Vector2(xInput, yInput).normalized;

        if (inputDir.magnitude > 0)
            currentVelocity = Vector2.MoveTowards(currentVelocity, inputDir * maxSpeed, acceleration * Time.deltaTime);
        else
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);

        transform.Translate(currentVelocity * Time.deltaTime);

        // 맵 제한
        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
        transform.position = clampedPos;

        // 2. 구멍 상호작용
        if (Input.GetKeyDown(KeyCode.Space)) Interact();
    }

    void Interact()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.7f);
        if (hit != null)
        {
            Hole holeScript = hit.GetComponent<Hole>();
            if (holeScript == null) holeScript = hit.GetComponentInParent<Hole>();
            if (holeScript != null)
            {
                if (holeScript.connectedHole != null)
                {
                    transform.position = holeScript.connectedHole.transform.position + new Vector3(teleportOffsetX, teleportOffsetY, 0);
                    currentVelocity = Vector2.zero;
                    return; 
                }
                int type = holeScript.OnInteract(); 
                if (type != -1)
                {
                    if (Stage2Manager.instance != null) Stage2Manager.instance.ProcessInteraction(type);
                    // else if (Stage3Manager.instance != null) Stage3Manager.instance.AddScore(type);
                }
            }
        }
    }

    // [핵심] FallingItem 처리 함수 (폭탄 로직 추가됨)
    public int AddItemToStack(GameObject itemObj, bool isBad, int colorIndex)
    {
        if (Stage2Manager.instance != null)
        {
            // 1. [추가됨] 나쁜 아이템(폭탄)일 경우
            if (isBad)
            {
                Debug.Log("폭탄 맞음! 점수 깎임");
                // 99번을 보내면 매니저가 알아서 1단계 깎습니다.
                Stage2Manager.instance.ProcessInteraction(99); 
                return 1; // 1을 리턴하면 폭탄이 사라집니다 (Destroy)
            }

            // 2. 정답 클로버인지 확인
            bool isCorrect = Stage2Manager.instance.CheckIsCorrectStep(colorIndex);

            if (isCorrect)
            {
                // 정답 -> 점수 오름 -> 클로버 사라짐
                Stage2Manager.instance.ProcessInteraction(colorIndex);
                Debug.Log("정답 클로버 획득!");
                return 1; 
            }
            else
            {
                // 오답 -> 튕겨나감
                Debug.Log("틀린 클로버! 튕겨냄");
                return 0; 
            }
        }
        return 0; // 매니저 없으면 튕겨냄
    }
}