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
                        
                        // [소리 추가] 순간이동 소리
                        if (Stage3Manager.instance != null) Stage3Manager.instance.PlaySFX("Teleport");

                        Vector3 targetPos = holeScript.connectedHole.transform.position;
                        targetPos.x += teleportOffsetX;
                        targetPos.y += teleportOffsetY; 
                        transform.position = targetPos;
                        lastMoveTime = Time.time; 
                        return; 
                    }

                    // [클로버 수확 시도]
                    int type = holeScript.OnInteract(); 
                    
                    if (type != -1) // 무언가 획득!
                    {
                        if (Stage3Manager.instance != null)
                        {
                            // 점수 적용
                            Stage3Manager.instance.AddScore(type);

                            // [소리 추가] 타입별 소리 재생
                            if (type == 0)      Stage3Manager.instance.PlaySFX("Red");    // 빨강(감점)
                            else if (type == 1) Stage3Manager.instance.PlaySFX("Green");  // 초록
                            else if (type == 2) Stage3Manager.instance.PlaySFX("Pink");   // 분홍
                        }

                        // 텍스트 효과
                        if (type == 0) ShowFloatingText("-1", Color.red); 
                        else if (type == 1) ShowFloatingText("+1", Color.green); 
                        else if (type == 2) ShowFloatingText("+3", new Color(1f, 0.5f, 0.8f)); 

                        // 빨강 클로버 저주 효과
                        if (type == 0)
                        {
                            StopCoroutine("InvertRoutine");
                            StartCoroutine("InvertRoutine");
                        }
                    }
                    else // 빈 구멍 클릭
                    {
                        // [소리 추가] 빈 구멍 소리
                        if (Stage3Manager.instance != null)
                        {
                             Stage3Manager.instance.PlaySFX("Empty");
                             Stage3Manager.instance.AddScore(-1); // 감점
                        }
                        ShowFloatingText("-1", Color.gray); 
                    }
                }
            }
        }
    }

    // FallingItem 호환용 함수
    public int AddItemToStack(GameObject itemObj, bool isBad, int colorIndex)
    {
        return 1; 
    }

    void ShowFloatingText(string msg, Color color)
    {
        if (floatingTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0.8f, 0.5f, 0);
            GameObject obj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
            
            FloatingText ft = obj.GetComponent<FloatingText>();
            if (ft != null) ft.SetText(msg, color);
        }
    }

    IEnumerator InvertRoutine()
    {
        isInverted = true;
        if(fogEffect != null) fogEffect.SetActive(true);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(1, 0.5f, 0.5f); 

        yield return new WaitForSeconds(3.0f); 

        isInverted = false; 
        if(fogEffect != null) fogEffect.SetActive(false); 
        sr.color = Color.white; 
    }
}