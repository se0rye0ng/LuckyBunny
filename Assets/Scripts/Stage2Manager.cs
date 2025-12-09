using UnityEngine;
using TMPro; // 텍스트 매쉬 프로 사용
using UnityEngine.SceneManagement; // 나중에 씬 이동 등을 위해

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 설정")]
    public float gameTime = 60.0f; // 제한 시간 60초
    public int targetScore = 20;   // 목표 점수

    [Header("연결할 UI들")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;
    public GameObject clearText;

    [Header("구멍 시스템")]
    public Hole[] holes;
    public float spawnInterval = 1.0f;

    // 내부 변수
    private float currentTimer;
    private int currentScore = 0;
    private float spawnTimer = 0f;
    private bool isGameActive = true;

    // 싱글톤 패턴 (어디서든 매니저를 부르기 쉽게 만듦)
    public static Stage2Manager instance;

    void Awake()
    {
        instance = this; // "내가 관리자다" 선언
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
        
        // 시간 초과 체크
        if (currentTimer <= 0)
        {
            currentTimer = 0;
            GameOver();
        }

        // 2. 구멍 생성 타이머
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnRandom();
            spawnTimer = 0f;
        }

        // 3. UI 갱신
        UpdateUI();
    }

    // 플레이어가 클로버를 먹었을 때 호출하는 함수
    public void AddScore(int type)
    {
        if (!isGameActive) return;

        // 분홍(2)은 3점, 나머지(0,1)는 1점
        int points = (type == 2) ? 3 : 1; 
        
        currentScore += points;
        UpdateUI();

        // 목표 달성 체크
        if (currentScore >= targetScore)
        {
            GameClear();
        }
    }

    void UpdateUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {currentTimer:F1}"; // 소수점 한자리까지

        if (scoreText != null)
            scoreText.text = $"Clovers: {currentScore} / {targetScore}";
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
        Time.timeScale = 0; // 시간 정지
        Debug.Log("스테이지 클리어!");
    }

    void GameOver()
    {
        isGameActive = false;
        Debug.Log("타임 오버! 실패!");
        // 여기에 실패 UI를 띄우거나 재시작 로직 추가 가능
        Time.timeScale = 0;
    }
}