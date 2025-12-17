using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필요

public class LPlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f; 
    [Tooltip("대각선 이동 시 속도 보정 여부")]
    public bool normalizeDiagonal = true;

    [Header("블러 효과")]
    public float blurDuration = 5f;

    [Header("오디오 설정")]
    public AudioSource audioSource; 
    public AudioClip redSound;   
    public AudioClip whiteSound; 
    public AudioClip greenSound; 

    [Header("클로버/파워업")]
    public float sizeBoostMultiplier = 1.5f;
    // (사용하지 않는 변수들은 깔끔하게 정리하거나 두셔도 무방합니다)

    // 내부 변수
    private int clover = 0;
    public int CloverCount => clover;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // [수정] 오디오 소스 연결 로직 정리
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // 찾아봤는데도 없으면 경고
        if (audioSource == null)
        {
            Debug.LogWarning("LPlayerController: AudioSource가 없습니다. 소리가 나지 않습니다.");
        }   

        if (spriteRenderer == null)
        {
            Debug.LogWarning("LPlayerController: SpriteRenderer가 없습니다. 캐릭터 뒤집기가 안 됩니다.");
        }
    }

    void Update()
    {
        // --- WASD 이동 ---
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) y = 1f;
        if (Input.GetKey(KeyCode.S)) y = -1f;

        Vector2 dir = new Vector2(x, y);
        if (dir.sqrMagnitude > 0f)
        {
            if (normalizeDiagonal && dir.sqrMagnitude > 1f) dir = dir.normalized;
            transform.Translate((Vector3)dir * moveSpeed * Time.deltaTime, Space.World);

            if (spriteRenderer != null)
            {
                if (x < 0f) spriteRenderer.flipX = false; 
                else if (x > 0f) spriteRenderer.flipX = true; 
            }
        }
    }

    // 소리 재생 헬퍼 함수
    void PlayerSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ▼ 충돌 감지 및 아이템 처리
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // BlurController 찾기
        var blur = UnityEngine.Object.FindFirstObjectByType<BlurController>();

        // 1. 빨강 클로버 (블러 효과 4개 동시 발동)
        if (other.CompareTag("RedClover"))
        {
            PlayerSound(redSound);
            if (blur != null)
            {
                // [주의] BlurController 리스트에 이미지가 최소 4개(0~3) 있어야 합니다.
                blur.ApplyBlur(0, blurDuration);
                blur.ApplyBlur(1, blurDuration);
                blur.ApplyBlur(2, blurDuration);
                blur.ApplyBlur(3, blurDuration);
            }
            else
            {
                Debug.LogWarning("LPlayerController: 씬에 BlurController가 없습니다.");
            }
            Destroy(other.gameObject);
            return;
        }

        // 2. 화이트 클로버 (해독제)
        else if (other.CompareTag("WhiteClover"))
        {
            PlayerSound(whiteSound);
            if (blur != null) 
            {
                blur.StopBlur(); 
            }
            Destroy(other.gameObject);
            return;
        }

        // 3. 초록 클로버 (다음 스테이지)
        else if (other.CompareTag("GreenClover"))
        {
            PlayerSound(greenSound);
            clover++;
            Debug.Log("클로버 획득! 현재 점수: " + clover);

            // [중요] 게임 클리어 시 타이머 멈추기
            var timer = UnityEngine.Object.FindFirstObjectByType<GameTimer>();
            if (timer != null) 
            {
                timer.StopTimer(); 
            }

            // 다음 레벨로 이동
            var game = UnityEngine.Object.FindFirstObjectByType<LGameController>();
            if (game != null)
            {
                game.GoToNextLevel();
            }
            else
            {
                // 컨트롤러 없으면 비상용으로 직접 로드
                Debug.LogWarning("LGameController가 없어 Level2로 강제 이동합니다.");
                SceneManager.LoadScene("Level2");
            }
            Destroy(other.gameObject);
            return;
        }
    }
}