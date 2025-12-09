using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; // 씬 재시작을 위해 필수

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 설정")]
    public float gameTime = 60.0f; // 제한 시간
    public int targetScore = 20;   // 목표 점수

    [Header("연결할 UI들")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public GameObject clearText;

    [Header("구멍 시스템")]
    public Hole[] holes;
    public float spawnInterval = 1.0f;

    private float currentTimer;
    private int currentScore = 0;
    private float spawnTimer = 0f;
    private bool isGameActive = true;

    public static Stage2Manager instance;

    void Awake()
    {
        instance = this; 
    }

    void Start()
    {
        holes = FindObjectsByType<Hole>(FindObjectsSortMode.None);
        currentTimer = gameTime;
        UpdateUI();
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. 시간 줄이기
        currentTimer -= Time.deltaTime;
        
        // 2. 시간 초과 체크 (실패 조건)
        if (currentTimer <= 0)
        {
            currentTimer = 0;
            // 시간이 끝났는데 점수가 모자라면 실패 -> 재시작
            if (currentScore < targetScore)
            {
                GameOverAndRestart();
            }
            else
            {
                // (혹시 모르니) 시간이 끝났는데 점수는 넘었다면 클리어 처리
                GameClear();
            }
        }

        // 3. 구멍 생성
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnRandom();
            spawnTimer = 0f;
        }

        UpdateUI();
    }

    public void AddScore(int type)
    {
        if (!isGameActive) return;

        int points = 0;

        // [점수 로직 수정]
        if (type == 0) // 빨강
        {
            points = -1;
            Debug.Log("빨간 클로버! -1점! 으악!");
        }
        else if (type == 1) // 초록
        {
            points = 1;
            Debug.Log("초록 클로버! +1점");
        }
        else if (type == 2) // 분홍
        {
            points = 3;
            Debug.Log("분홍 클로버! +3점 대박!");
        }
        
        currentScore += points;

        // [요청] 콘솔창에 현재 점수 띄우기
        Debug.Log($"현재 총 점수: {currentScore} / {targetScore}");

        UpdateUI();

        // 목표 점수 도달 시 즉시 클리어
        if (currentScore >= targetScore)
        {
            GameClear();
        }
    }

    void UpdateUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {currentTimer:F1}";

        if (scoreText != null)
            scoreText.text = $"Score: {currentScore} / {targetScore}";
    }

    void TrySpawnRandom()
    {
        if (holes.Length == 0) return;
        int randomIndex = Random.Range(0, holes.Length);
        Hole selectedHole = holes[randomIndex];

        if (!selectedHole.isActive)
        {
            selectedHole.SpawnClover();
        }
    }

    void GameClear()
    {
        isGameActive = false;
        if (clearText != null) clearText.SetActive(true);
        Time.timeScale = 0; 
        Debug.Log("축하합니다! 스테이지 클리어!");
    }

    void GameOverAndRestart()
    {
        isGameActive = false;
        Debug.Log("시간 종료! 목표 점수 미달로 재시작합니다.");
        
        // 현재 씬을 다시 로드 (재시작)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}