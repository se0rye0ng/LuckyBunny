using UnityEngine;
using TMPro;            
using UnityEngine.UI;   
using UnityEngine.SceneManagement; 
using System.Collections; // [필수] 코루틴 사용을 위해 추가

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 기본 설정")]
    public float gameTime = 30.0f; 
    public int targetScore = 10;   

    [Header("다음 단계 설정")]
    // 여기에 "level03_Intro 1" 또는 "EndingScene"을 적으면 거기로 이동합니다.
    public string nextSceneName = "level03_Intro 1"; 

    [Header("오디오 설정")]
    public AudioSource bgmAudioSource; // 배경음악이 나오는 오디오 소스 연결
    public float fadeDuration = 2.0f;  // 음악이 꺼지는 데 걸리는 시간 (2초)

    [Header("UI 연결")]
    public TextMeshProUGUI timeText;  
    public TextMeshProUGUI scoreText; 
    public Image gaugeImage;       
    public Sprite[] gaugeSprites;  
    public GameObject clearText;      

    [Header("구멍 및 생성 설정")]
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

        currentTimer -= Time.deltaTime;
        
        if (currentTimer <= 0)
        {
            currentTimer = 0;
            if (currentScore < targetScore) GameOverAndRestart();
            else StartCoroutine(GameClearRoutine()); // 시간 끝났는데 점수 넘으면 클리어
        }

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
        if (type == 0) points = -1;       
        else if (type == 1) points = 1;   
        else if (type == 2) points = 2;   
        else if (type == -1) points = -1; 

        currentScore += points;
        currentScore = Mathf.Clamp(currentScore, 0, targetScore);

        UpdateUI(); 

        // 목표 점수 도달 시 클리어 루틴 시작!
        if (currentScore >= targetScore)
        {
            StartCoroutine(GameClearRoutine());
        }
    }

    void UpdateUI()
    {
        if (timeText != null) timeText.text = $"Time: {currentTimer:F0}";
        if (scoreText != null) scoreText.text = $"{currentScore} / {targetScore}";

        if (gaugeImage != null && gaugeSprites != null && gaugeSprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(currentScore, 0, gaugeSprites.Length - 1);
            gaugeImage.sprite = gaugeSprites[spriteIndex];
        }
    }

    void TrySpawnRandom()
    {
        if (holes.Length == 0) return;
        int randomIndex = Random.Range(0, holes.Length);
        Hole selectedHole = holes[randomIndex];
        if (!selectedHole.isActive) selectedHole.SpawnClover();
    }

    // [핵심] 페이드아웃 및 씬 이동 처리
    IEnumerator GameClearRoutine()
    {
        isGameActive = false; // 게임 멈춤 (타이머, 구멍 생성 정지)
        
        if (clearText != null) clearText.SetActive(true);
        Debug.Log("스테이지 클리어! 페이드아웃 시작...");

        // BGM 페이드아웃 로직
        if (bgmAudioSource != null)
        {
            float startVolume = bgmAudioSource.volume;
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                // 시간이 지날수록 볼륨을 0으로 줄임
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
                yield return null;
            }
            bgmAudioSource.volume = 0; // 확실하게 0으로
        }
        else
        {
            // 오디오 소스가 없으면 그냥 설정한 시간만큼 대기
            yield return new WaitForSeconds(fadeDuration);
        }

        // 다음 씬으로 이동
        Debug.Log($"씬 이동: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    void GameOverAndRestart()
    {
        isGameActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}