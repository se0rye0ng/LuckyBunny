using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("빙판길 이동 설정")]
    public float maxSpeed = 5.0f;      
    public float acceleration = 20.0f; 
    public float deceleration = 10.0f; 

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
        float xInput = Input.GetAxisRaw("Horizontal"); 
        float yInput = Input.GetAxisRaw("Vertical");   

        Vector2 inputDir = new Vector2(xInput, yInput).normalized;

        if (inputDir.magnitude > 0)
            currentVelocity = Vector2.MoveTowards(currentVelocity, inputDir * maxSpeed, acceleration * Time.deltaTime);
        else
            currentVelocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);

        transform.Translate(currentVelocity * Time.deltaTime);

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
        transform.position = clampedPos;

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
                }
            }
        }
    }

    // [핵심] 소리 재생 코드 추가됨
    public int AddItemToStack(GameObject itemObj, bool isBad, int colorIndex)
    {
        if (Stage2Manager.instance != null)
        {
            // 1. 나쁜 아이템(폭탄)일 경우 -> 빨간 소리
            if (isBad)
            {
                Debug.Log("폭탄 맞음! 점수 깎임");
                Stage2Manager.instance.PlaySFX(true); // true = 나쁜 소리 재생해라!
                Stage2Manager.instance.ProcessInteraction(99); 
                return 1; 
            }

            // 2. 정답 클로버인지 확인
            bool isCorrect = Stage2Manager.instance.CheckIsCorrectStep(colorIndex);

            if (isCorrect)
            {
                // 정답 -> 초록 소리
                Debug.Log("정답 클로버 획득!");
                Stage2Manager.instance.PlaySFX(false); // false = 좋은 소리 재생해라!
                Stage2Manager.instance.ProcessInteraction(colorIndex);
                return 1; 
            }
            else
            {
                // 오답 -> 튕겨나감 (소리 없음 혹은 원하면 튕기는 소리 추가 가능)
                Debug.Log("틀린 클로버! 튕겨냄");
                return 0; 
            }
        }
        return 0; 
    }
}