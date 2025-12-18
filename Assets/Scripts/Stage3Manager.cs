using UnityEngine;
using TMPro;            
using UnityEngine.UI;   
using UnityEngine.SceneManagement; 
using System.Collections; 

public class Stage3Manager : MonoBehaviour
{
    [Header("게임 기본 설정")]
    public float gameTime = 30.0f; 
    public int targetScore = 10;   

    [Header("다음 단계 설정")]
    public string nextSceneName = "level03_Intro 1"; 

    [Header("오디오 설정 (배경음악)")]
    public AudioSource bgmAudioSource; 
    public float fadeDuration = 2.0f;  

    [Header("효과음 설정 (추가됨)")]
    public AudioSource sfxAudioSource; // 효과음 전용 스피커
    public AudioClip greenCloverSound; // 초록 클로버 (+1)
    public AudioClip pinkCloverSound;  // 분홍 클로버 (+3)
    public AudioClip redCloverSound;   // 빨강 클로버 (-1)
    public AudioClip emptyHoleSound;   // 빈 구멍 (-1)
    public AudioClip teleportSound;    // 순간이동

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

    public static Stage3Manager instance;

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
            else StartCoroutine(GameClearRoutine());
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnRandom();
            spawnTimer = 0f;
        }

        UpdateUI();
    }

    // [핵심] 효과음 재생 함수
    public void PlaySFX(string type)
    {
        if (sfxAudioSource == null) return;

        AudioClip clipToPlay = null;

        switch (type)
        {
            case "Green": clipToPlay = greenCloverSound; break;
            case "Pink": clipToPlay = pinkCloverSound; break;
            case "Red": clipToPlay = redCloverSound; break;
            case "Empty": clipToPlay = emptyHoleSound; break;
            case "Teleport": clipToPlay = teleportSound; break;
        }

        if (clipToPlay != null)
        {
            sfxAudioSource.PlayOneShot(clipToPlay);
        }
    }

    public void AddScore(int type)
    {
        if (!isGameActive) return;

        int points = 0;
        // 점수 계산 로직
        if (type == 0) points = -1;       // 빨강
        else if (type == 1) points = 1;   // 초록
        else if (type == 2) points = 3;   // 분홍 (NewPlayerController 표기 기준 +3)
        else if (type == -1) points = -1; // 빈 구멍

        currentScore += points;
        // 점수가 0 밑으로 떨어지지 않게 하려면 아래 Clamp 최소값을 0으로, 
        // 음수도 허용하려면 제거하세요. 여기선 0~targetScore로 제한했습니다.
        currentScore = Mathf.Clamp(currentScore, 0, targetScore);

        UpdateUI(); 

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
            // 점수에 따라 게이지 이미지 변경 (비율로 계산)
            float ratio = (float)currentScore / targetScore;
            int spriteIndex = Mathf.FloorToInt(ratio * (gaugeSprites.Length - 1));
            spriteIndex = Mathf.Clamp(spriteIndex, 0, gaugeSprites.Length - 1);
            
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

    IEnumerator GameClearRoutine()
    {
        isGameActive = false; 
        if (clearText != null) clearText.SetActive(true);
        
        if (bgmAudioSource != null)
        {
            float startVolume = bgmAudioSource.volume;
            float timer = 0;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeDuration);
                yield return null;
            }
            bgmAudioSource.volume = 0; 
        }
        else
        {
            yield return new WaitForSeconds(fadeDuration);
        }

        SceneManager.LoadScene(nextSceneName);
    }

    void GameOverAndRestart()
    {
        isGameActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}