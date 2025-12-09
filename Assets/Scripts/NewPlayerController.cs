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
                transform.position += new Vector3(xInput, yInput, 0);
                lastMoveTime = Time.time;
            }
        }
        
        // 2. 상호작용 로직 (텔레포트 확인)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 반지름 0.2f 원 안에 걸리는 게 있는지 확인
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.2f);
            
            if (hit != null)
            {
                Hole holeScript = hit.GetComponent<Hole>();
                if (holeScript == null) holeScript = hit.GetComponentInParent<Hole>();

                if (holeScript != null)
                {
                    // [텔레포트] 연결된 구멍이 있다면 이동!
                    if (holeScript.connectedHole != null)
                    {
                        Debug.Log("토끼굴 발견! 이동합니다.");
                        transform.position = holeScript.connectedHole.transform.position;
                        lastMoveTime = Time.time; // 이동 직후 실수 방지 쿨타임
                        return; // 텔레포트 했으면 아래 코드는 실행 안 함
                    }

                    // [클로버 수확]
                    int type = holeScript.OnInteract(); 
                    if (type != -1)
                    {
                        Stage2Manager.instance.AddScore(type);
                        if (type == 0)
                        {
                            StopCoroutine("InvertRoutine");
                            StartCoroutine("InvertRoutine");
                        }
                    }
                }
            }
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