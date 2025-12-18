using UnityEngine;
using TMPro;            
using UnityEngine.UI;   
using UnityEngine.SceneManagement; 
using System.Collections; 
using System.Collections.Generic; 

public class Stage2Manager : MonoBehaviour
{
    [Header("게임 기본 설정")]
    public float gameTime = 60.0f; 
    public string nextSceneName = "level03_Intro 1"; 

    [Header("UI 연결")]
    public TextMeshProUGUI timeText;  
    public Image gaugeImage;       
    public GameObject clearText;      

    [Header("게이지 바 설정")]
    public Sprite[] gaugeSprites;  // 이미지 순서대로 (0:빨강 완료, 1:주황 완료...)
    public List<Vector3> barScales; // 크기 조절용 리스트

    [Header("구멍 시스템 (사용 안 해도 유지)")]
    public Hole[] holes;           
    public float spawnInterval = 1.0f; 

    // 내부 변수
    private float currentTimer;
    private int currentStep = 0; // 현재 단계 (0:빨강 기다리는 중)
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
        currentTimer = gameTime;
        
        // 크기 설정 리스트 초기화 (에러 방지)
        if (barScales == null || barScales.Count == 0)
        {
            barScales = new List<Vector3>();
            for(int i=0; i < gaugeSprites.Length; i++) 
                barScales.Add(Vector3.one);
        }

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
        UpdateUI();
    }

    // [추가] 순서가 맞는지 확인만 하는 함수 (FallingItem이 물어봄)
    public bool CheckIsCorrectStep(int colorIndex)
    {
        // 99번(폭탄)은 무조건 오답 처리 (튕겨내기 원하면 false, 먹어서 터지게 하려면 별도 처리)
        if (colorIndex == 99) return false; 

        return colorIndex == currentStep;
    }

    // 실제 점수/단계 적용 함수
    public void ProcessInteraction(int type)
    {
        if (!isGameActive) return;

        // 폭탄 로직 (필요시 사용)
        if (type == 99)
        {
            if (currentStep > 0) currentStep--;
            UpdateUI();
            return;
        }

        // 순서 맞으면 단계 상승
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

            // 1. 이미지 교체
            gaugeImage.sprite = gaugeSprites[spriteIndex];

            // 2. [수정됨] 리스트에 값이 있을 때만 크기 적용 (0,0,0이면 무시)
            // 만약 Inspector에서 Bar Scales 값을 모두 0으로 두면, 화면에서 설정한 크기가 유지됩니다.
            if (spriteIndex < barScales.Count)
            {
                // 리스트에 적힌 값이 (0,0,0)이나 (1,1,1)이 아닐 때만 적용하고 싶다면 조건을 걸 수도 있지만,
                // 지금은 그냥 리스트의 X값이 1보다 클 때만 강제로 적용하도록 해보겠습니다.
                if (barScales[spriteIndex].x > 1.0f || barScales[spriteIndex].y > 1.0f)
                {
                     gaugeImage.rectTransform.localScale = barScales[spriteIndex];
                }
            }
            
            // 또는 아예 크기 조절 코드를 지워버리면(= 주석 처리하면) 화면에서 늘린 그대로 나옵니다!
            // gaugeImage.rectTransform.localScale = barScales[spriteIndex];  <-- 이 줄을 지우면 해결
        }
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