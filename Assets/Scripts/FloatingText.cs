using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("기본 설정")]
    public float moveSpeed = 2.0f; // 위로 올라가는 속도
    public float destroyTime = 1.0f; // 사라지는 시간

    [Header("별 반짝임(Twinkle) 설정")]
    public float sparkleSpeed = 20.0f; // 반짝이는 속도 (높을수록 파바박거림)
    public float sparkleAmount = 0.3f; // 반짝일 때 커지는 정도 (높을수록 크기 변화가 큼)
    // [선택사항] 약간의 회전을 주면 더 별 같습니다.
    public bool enableRotation = false; 
    public float rotationSpeed = 90f;

    private TextMeshPro textMesh;
    private Color alphaColor;
    private Vector3 initialScale;
    private float randomOffset; // 모든 글자가 똑같이 반짝이면 어색해서 넣는 랜덤값

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        alphaColor = textMesh.color;
        initialScale = transform.localScale;
        // 시작할 때 랜덤한 오프셋을 주어 글자마다 반짝이는 타이밍을 다르게 함
        randomOffset = Random.Range(0f, 100f); 
    }

    public void SetText(string message, Color color)
    {
        textMesh.text = message;
        textMesh.color = color;
        alphaColor = color;
        Destroy(gameObject, destroyTime); 
    }

    void Update()
    {
        // 1. 위로 이동 & 투명해지기 (기존과 동일)
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        alphaColor.a = Mathf.Lerp(alphaColor.a, 0, Time.deltaTime * 3.0f);
        textMesh.color = alphaColor;

        // --- 2. [핵심 변경] 별 반짝임 효과 (Perlin Noise) ---
        
        // 시간 흐름에 따라 불규칙한 0.0 ~ 1.0 사이의 값을 얻어옵니다.
        float noise = Mathf.PerlinNoise((Time.time + randomOffset) * sparkleSpeed, 0f);

        // 노이즈 값을 이용해 크기를 결정합니다.
        // 기본 크기(1.0)에서 최대 (1.0 + sparkleAmount)까지 노이즈에 따라 왔다갔다 합니다.
        float currentScaleMultiplier = Mathf.Lerp(1.0f, 1.0f + sparkleAmount, noise);
        
        transform.localScale = initialScale * currentScaleMultiplier;

        // [선택사항] 회전 추가
        if (enableRotation)
        {
            // Z축(앞뒤 방향)을 기준으로 빙글빙글 돌립니다.
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
    }
}