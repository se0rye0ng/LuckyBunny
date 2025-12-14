using UnityEngine;

public class LPlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 2f; // 이동 속도
    [Tooltip("대각선 이동 시 속도 보정 여부 (true면 정규화하여 동일 속도 유지)")]
    public bool normalizeDiagonal = true;
    [Header("블러 효과")]
    public float blurWeight = 1f;
    public float blurDuration = 5f;
    [Header("클로버/파워업")]

    [Header("오디오 설정 (여기에 파일 넣으세요)")]
    public AudioSource audioSource; // 플레이어에 붙인 스피커
    public AudioClip redSound;   // 빨강 클로버 소리
    public AudioClip whiteSound; // 하양 클로버 소리
    public AudioClip greenSound; // 초록 클로버 소리

    public float sizeBoostMultiplier = 1.5f;
    public float sizeBoostDuration = 5f;
    public float speedBoostMultiplier = 1.5f;
    public float speedBoostDuration = 5f;

    // internal state
    private int clover = 0;
    public int CloverCount => clover;
    private float baseMoveSpeed;
    private Vector3 originalScale;
    private Coroutine speedBoostCoroutine;
    private Coroutine sizeBoostCoroutine;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        {
            Debug.LogWarning("LPlayerController: AudioSource가 설정되지 않았습니다. 소리가 재생되지 않을 수 있습니다.");
        }   
        if (spriteRenderer == null)
        {
            Debug.LogWarning("LPlayerController: SpriteRenderer가 없습니다. 캐릭터 뒤집기가 작동하지 않습니다.");
        }
    }

    void Update()
    {
        // WASD 입력 처리: W=위, A=왼쪽, S=아래, D=오른쪽
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A)) x = -1f;
        if (Input.GetKey(KeyCode.D)) x = 1f;
        if (Input.GetKey(KeyCode.W)) y = 1f;
        if (Input.GetKey(KeyCode.S)) y = -1f;

        Vector2 dir = new Vector2(x, y);
        if (dir.sqrMagnitude > 0f)
        {
            if (normalizeDiagonal && dir.sqrMagnitude > 1f)
            {
                dir = dir.normalized;
            }

            // Transform 기반 이동 (무중력/직접 이동 방식)
            transform.Translate((Vector3)dir * moveSpeed * Time.deltaTime, Space.World);

            // 좌우 방향에 따라 스프라이트 뒤집기
            if (spriteRenderer != null)
            {
                if (x < 0f) spriteRenderer.flipX = false; // 왼쪽
                else if (x > 0f) spriteRenderer.flipX = true; // 오른쪽
            }
        }
    }

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

        // [중요 수정] BlurController를 맨 처음에 찾습니다. 
        // 그래야 RedClover, WhiteClover 모든 곳에서 'blur' 변수를 쓸 수 있습니다.
        var blur = UnityEngine.Object.FindFirstObjectByType<BlurController>();

        // 1. 빨강 클로버 (블러 효과 발동)
        if (other.CompareTag("RedClover"))
        {
            PlayerSound(redSound);
            if (blur != null)
            {
                // 인스펙터 리스트에 넣어둔 0, 1, 2번 이미지를 동시에 켭니다.
                // (리스트에 이미지가 최소 3개 있어야 에러가 안 납니다!)
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

        // 2. 화이트 클로버 (해독제 - 모든 효과 끄기)
        else if (other.CompareTag("WhiteClover"))
        {
            PlayerSound(whiteSound);
            if (blur != null) 
            {
                blur.StopBlur(); // 모든 블러 끄기
            }
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("GreenClover"))
        {
            PlayerSound(greenSound);
            // GreenClover 획득은 점수(클로버 카운트) 증가로 취급
            clover++;
            Debug.Log("클로버 획득! 현재 점수: " + clover);

            var game = UnityEngine.Object.FindFirstObjectByType<LGameController>();
            if (game != null)
            {
                game.GoToNextLevel();
            }
            else
            {
                Debug.LogWarning("LPlayerController: LGameController가 씬에 없습니다. 직접 Level2로 로드합니다.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Level2");
            }
            Destroy(other.gameObject);
            return;
        }
    }
}
