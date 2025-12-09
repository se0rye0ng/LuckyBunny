using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 설정")]
    public float gameTime = 60.0f; // 제한 시간
    public int targetScore = 20;   // 목표 점수 (분모로 표시됨)

    [Header("연결할 UI들")]
    public TextMeshProUGUI timeText;  // 시간 표시용 텍스트
    public TextMeshProUGUI scoreText; // 점수 표시용 텍스트 (여기에 1/20 표시)
    public GameObject clearText;      // 클리어 시 뜰 텍스트

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
        
        // 게임 시작하자마자 0 / 20 이라고 뜨게 갱신
        UpdateUI(); 
    }

    void Update()
    {
        if (!isGameActive) return;

        // 1. 시간 줄이기
        currentTimer -= Time.deltaTime;
        
        // 2. 시간 초과 체크
        if (currentTimer <= 0)
        {
            currentTimer = 0;
            if (currentScore < targetScore)
            {
                GameOverAndRestart();
            }
            else
            {
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

        // 매 프레임 UI 갱신 (시간이 계속 흐르므로)
        UpdateUI();
    }

    public void AddScore(int type)
    {
        if (!isGameActive) return;

        int points = 0;

        if (type == 0) points = -1; // 빨강
        else if (type == 1) points = 1; // 초록
        else if (type == 2) points = 3; // 분홍
        else if (type == -1) points = -1; // 빈 구멍

        currentScore += points;

        // 점수가 바뀌었으니 UI 갱신
        UpdateUI();

        if (currentScore >= targetScore)
        {
            GameClear();
        }
    }

    // [핵심] 화면 표시 내용 설정하는 함수
    void UpdateUI()
    {
        // 1. 시간 표시 (소수점 없이 정수로 깔끔하게)
        if (timeText != null)
            timeText.text = $"Time: {currentTimer:F0}";

        // 2. 점수 표시 (요청하신 1 / 20 형식)
        if (scoreText != null)
        {
            // 예: "3 / 20" 처럼 표시됨
            scoreText.text = $"{currentScore} / {targetScore}"; 
        }
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
        Debug.Log("스테이지 클리어!");
    }

    void GameOverAndRestart()
    {
        isGameActive = false;
        Debug.Log("재시작");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}