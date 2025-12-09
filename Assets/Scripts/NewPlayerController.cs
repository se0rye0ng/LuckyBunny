using UnityEngine;
using System.Collections;

public class NewPlayerController : MonoBehaviour
{
    public float moveCooldown = 0.15f; 
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
        // --- 1. 이동 로직 ---
        if (Time.time >= lastMoveTime + moveCooldown)
        {
            float xInput = Input.GetAxisRaw("Horizontal"); 
            float yInput = Input.GetAxisRaw("Vertical");   

            // 저주 걸렸으면 반대로
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
                transform.position += new Vector3(xInput, yInput, 0);
                lastMoveTime = Time.time;
            }
        }
        
        // --- 2. 상호작용 로직 ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // [수정] 인식 범위 0.7f로 대폭 확대 (이제 대충 서도 잡힘)
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
                        transform.position = holeScript.connectedHole.transform.position;
                        lastMoveTime = Time.time; 
                        return; 
                    }

                    // [클로버 수확]
                    int type = holeScript.OnInteract(); 
                    
                    if (type != -1)
                    {
                        // 매니저에게 점수 계산 요청
                        Stage2Manager.instance.AddScore(type);

                        // 빨간색(0번)이면 저주(조작반전)도 같이 검
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
                Debug.Log("너무 멉니다. 조금 더 가까이 가보세요.");
            }
        }
    }

    IEnumerator InvertRoutine()
    {
        Debug.Log("저주 시작! (조작 반전)");
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