using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour
{
    public float moveCooldown = 0.15f; 
    
    [Header("이동 거리 설정")]
    public float moveStepX = 1.0f; 
    public float moveStepY = 1.0f; 
    
    [Header("텔레포트 위치 보정")]
    public float teleportOffsetX = 0f; // 가로 보정 (0이면 구멍 중앙)
    public float teleportOffsetY = 0.5f; // 세로 보정 (0.5면 반 칸 위)

    [Header("연결 요소")]
    public Transform gridParent; 
    public GameObject fogEffect; 

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
                
                transform.position += new Vector3(xInput * moveStepX, yInput * moveStepY, 0);
                
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
                        
                        // 목표 위치 가져오기
                        Vector3 targetPos = holeScript.connectedHole.transform.position;
                        
                        // [X, Y 보정값 적용]
                        targetPos.x += teleportOffsetX;
                        targetPos.y += teleportOffsetY; 
                        
                        transform.position = targetPos;
                        lastMoveTime = Time.time; 
                        return; 
                    }

                    // [클로버 수확]
                    int type = holeScript.OnInteract(); 
                    
                    if (type != -1)
                    {
                        if (Stage2Manager.instance != null)
                            Stage2Manager.instance.AddScore(type);

                        if (type == 0)
                        {
                            StopCoroutine("InvertRoutine");
                            StartCoroutine("InvertRoutine");
                        }
                    }
                    else
                    {
                        Debug.Log("빈 구멍입니다.");
                    }
                }
            }
            else
            {
                Debug.Log("너무 멉니다.");
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