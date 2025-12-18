using UnityEngine;
using TMPro;            
using UnityEngine.UI;   
using UnityEngine.SceneManagement; 
using System.Collections; 

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 기본 설정")]
    public float gameTime = 60.0f; 
    public string nextSceneName = "level03_Intro 1"; 

    [Header("오디오 설정 (추가됨)")]
    public AudioSource sfxAudioSource; // 효과음 전용 오디오 소스
    public AudioClip correctSound;     // GreenCloverBGM.wav 넣으세요
    public AudioClip badSound;         // RedCloverBGM.wav 넣으세요

    [Header("UI 연결")]
    public TextMeshProUGUI timeText;  
    public Image gaugeImage;       
    public GameObject clearText;      

    [Header("게이지 바 설정")]
    public Sprite[] gaugeSprites;  
    
    [Header("구멍 시스템")]
    public Hole[] holes;           
    public float spawnInterval = 1.0f; 

    // 내부 변수
    private float currentTimer;
    private int currentStep = 0; 
    private int targetStep = 7;  
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
            GameOverAndRestart(); 
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            TrySpawnRandom();
            spawnTimer = 0f;
        }

        UpdateUI();
    }

    // [추가됨] 효과음 재생 함수
    public void PlaySFX(bool isBad)
    {
        if (sfxAudioSource == null) return;

        if (isBad)
        {
            // 나쁜 아이템 소리 (RedCloverBGM)
            if (badSound != null) sfxAudioSource.PlayOneShot(badSound);
        }
        else
        {
            // 정답 소리 (GreenCloverBGM)
            if (correctSound != null) sfxAudioSource.PlayOneShot(correctSound);
        }
    }

    public bool CheckIsCorrectStep(int colorIndex)
    {
        if (colorIndex == 99) return false; 
        return colorIndex == currentStep;
    }

    public void ProcessInteraction(int type)
    {
        if (!isGameActive) return;

        // 폭탄 로직
        if (type == 99)
        {
            if (currentStep > 0) currentStep--;
            UpdateUI();
            return;
        }

        // 정답 로직
        if (type == currentStep)
        {
            currentStep++; 
            UpdateUI();

            if (currentStep >= targetStep)
            {
                StartCoroutine(GameClearRoutine());
            }
        }
    }

    void UpdateUI()
    {
        if (timeText != null) timeText.text = $"Time: {currentTimer:F0}";

        if (gaugeImage != null && gaugeSprites != null && gaugeSprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(currentStep, 0, gaugeSprites.Length - 1);
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
        Time.timeScale = 1.0f; 
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(nextSceneName);
    }

    void GameOverAndRestart()
    {
        isGameActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}