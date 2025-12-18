using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour
{
    public float moveCooldown = 0.15f; 
    
    [Header("이동 거리 및 맵 제한")]
    public float moveStepX = 1.0f; 
    public float moveStepY = 1.0f; 
    public float minX = -8f, maxX = 8f;
    public float minY = -4.5f, maxY = 4.5f;
    
    [Header("텔레포트 위치 보정")]
    public float teleportOffsetX = 0f; 
    public float teleportOffsetY = 0.5f; 

    [Header("연결 요소")]
    public Transform gridParent; 
    public GameObject fogEffect; 
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
                
                Vector3 targetPos = transform.position + new Vector3(xInput * moveStepX, yInput * moveStepY, 0);

                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

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
                        // [핵심 수정 1] Stage3Manager에게 점수 추가 명령! (이게 빠져있었습니다)
                        if (Stage3Manager.instance != null)
                        {
                            Stage3Manager.instance.AddScore(type);
                        }
                        // 혹시 Stage2Manager를 쓸 경우를 대비
                        else if (Stage2Manager.instance != null)
                        {
                            Stage2Manager.instance.ProcessInteraction(type);
                        }

                        // 점수 효과 띄우기
                        if (type == 0) ShowFloatingText("-1", Color.red); 
                        else if (type == 1) ShowFloatingText("+1", Color.green); 
                        else if (type == 2) ShowFloatingText("+3", new Color(1f, 0.5f, 0.8f)); 

                        if (type == 0)
                        {
                            StopCoroutine("InvertRoutine");
                            StartCoroutine("InvertRoutine");
                        }
                    }
                    else // 빈 구멍 클릭
                    {
                        Debug.Log("빈 구멍입니다. 감점!");
                        
                        // [핵심 수정 2] 빈 구멍일 때도 점수 깎으라고 명령!
                        if (Stage3Manager.instance != null)
                        {
                            Stage3Manager.instance.AddScore(-1); 
                        }

                        ShowFloatingText("-1", Color.gray); 
                    }
                }
            }
            else
            {
                Debug.Log("너무 멉니다.");
            }
        }
    }

    // --------------------------------------------------------------------------
    // [핵심 수정 3] FallingItem 오류 해결을 위한 함수 추가
    // --------------------------------------------------------------------------
    public int AddItemToStack(GameObject itemObj, bool isBad, int colorIndex)
    {
        // FallingItem 스크립트가 에러 나지 않도록 "잘 받았다(1)" 신호를 줍니다.
        return 1; 
    }

    void ShowFloatingText(string msg, Color color)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0.8f, 0.5f, 0);
            GameObject obj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            
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