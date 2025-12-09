using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour
{
    public float moveCooldown = 0.15f; 
    
    [Header("이동 거리 및 맵 제한")]
    public float moveStepX = 1.0f; 
    public float moveStepY = 1.0f; 
    // [추가] 맵 밖으로 못 나가게 막는 좌표 (Inspector에서 맵 크기에 맞춰 조절하세요)
    public float minX = -8f, maxX = 8f;
    public float minY = -4.5f, maxY = 4.5f;
    
    [Header("텔레포트 위치 보정")]
    public float teleportOffsetX = 0f; 
    public float teleportOffsetY = 0.5f; 

    [Header("연결 요소")]
    public Transform gridParent; 
    public GameObject fogEffect; 
    // [추가] 반짝이는 점수 글자 프리팹 연결
    public GameObject floatingTextPrefab; 

    private float lastMoveTime;
    private Animator anim;
    private int facingDir = 1; 
    private bool isInverted = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetInteger("Direction", 1);
    }

    void Update()
    {
        // 1. 이동 로직
        if (Time.time >= lastMoveTime + moveCooldown)
        {
            float xInput = Input.GetAxisRaw("Horizontal"); 
            float yInput = Input.GetAxisRaw("Vertical");   

            if (isInverted)
            {
                xInput = -xInput;
                yInput = -yInput;
            }

            if (xInput != 0) yInput = 0;

            if (xInput != 0 || yInput != 0)
            {
                if (yInput > 0) facingDir = 0;      
                else if (yInput < 0) facingDir = 1; 
                else if (xInput < 0) facingDir = 2; 
                else if (xInput > 0) facingDir = 3; 
                
                anim.SetInteger("Direction", facingDir);
                
                // [수정] 이동 목표 위치 계산
                Vector3 targetPos = transform.position + new Vector3(xInput * moveStepX, yInput * moveStepY, 0);

                // [추가] 맵 밖으로 나가지 못하게 가두기 (Clamp)
                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

                // 실제 위치 적용
                transform.position = targetPos;
                
                lastMoveTime = Time.time;
            }
        }
        
        // 2. 상호작용 로직
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.7f);
            
            if (hit != null)
            {
                Hole holeScript = hit.GetComponent<Hole>();
                if (holeScript == null) holeScript = hit.GetComponentInParent<Hole>();

                if (holeScript != null)
                {
                    // [텔레포트]
                    if (holeScript.connectedHole != null)
                    {
                        Debug.Log("토끼굴 이동!");
                        Vector3 targetPos = holeScript.connectedHole.transform.position;
                        targetPos.x += teleportOffsetX;
                        targetPos.y += teleportOffsetY; 
                        transform.position = targetPos;
                        lastMoveTime = Time.time; 
                        return; 
                    }

                    // [클로버 수확 시도]
                    int type = holeScript.OnInteract(); 
                    
                    if (type != -1) // 클로버 획득 성공!
                    {
                        if (Stage2Manager.instance != null)
                            Stage2Manager.instance.AddScore(type);

                        // [추가] 점수 효과 띄우기 (색상 지정)
                        if (type == 0) ShowFloatingText("-1", Color.red); // 빨강
                        else if (type == 1) ShowFloatingText("+1", Color.green); // 초록
                        else if (type == 2) ShowFloatingText("+3", new Color(1f, 0.5f, 0.8f)); // 분홍

                        if (type == 0)
                        {
                            StopCoroutine("InvertRoutine");
                            StartCoroutine("InvertRoutine");
                        }
                    }
                    else // [추가] 빈 구멍 클릭! (감점)
                    {
                        Debug.Log("빈 구멍입니다. 감점!");
                        if (Stage2Manager.instance != null)
                            Stage2Manager.instance.AddScore(-1); // -1점 처리
                        
                        ShowFloatingText("-1", Color.gray); // 회색 글씨
                    }
                }
            }
            else
            {
                Debug.Log("너무 멉니다.");
            }
        }
    }

    // [추가] 텍스트 생성 함수 (캐릭터 우측 상단에 표시)
    void ShowFloatingText(string msg, Color color)
    {
        if (floatingTextPrefab != null)
        {
            // 캐릭터 위치 + 오른쪽(0.8) + 위(0.5)
            Vector3 spawnPos = transform.position + new Vector3(0.8f, 0.5f, 0);
            
            GameObject obj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            
            // 프리팹에 있는 FloatingText 스크립트 가져와서 설정
            FloatingText ft = obj.GetComponent<FloatingText>();
            if (ft != null)
            {
                ft.SetText(msg, color);
            }
        }
    }

    IEnumerator InvertRoutine()
    {
        Debug.Log("저주 시작!");
        isInverted = true;
        if(fogEffect != null) fogEffect.SetActive(true);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 0.5f, 0.5f); 

        yield return new WaitForSeconds(3.0f); 

        isInverted = false; 
        if(fogEffect != null) fogEffect.SetActive(false); 
        sr.color = Color.white; 
        Debug.Log("저주 해제.");
    }
}