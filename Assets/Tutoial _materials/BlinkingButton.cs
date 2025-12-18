using UnityEngine;
using UnityEngine.UI; // 버튼 기능을 사용하기 위해 추가!
using TMPro; 

public class BlinkingButton : MonoBehaviour
{
    [Header("설정")]
    public float blinkSpeed = 2.0f; 
    public AudioClip clickSound; // 🔊 여기에 효과음 파일을 넣을 변수

    private TextMeshProUGUI buttonText;
    private AudioSource audioSource; // 소리를 재생할 스피커 역할
    private Button btn;

    void Start()
    {
        // 1. 텍스트 컴포넌트 찾기
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // 2. 오디오 소스(스피커) 컴포넌트 설정
        // 이 오브젝트에 AudioSource가 없으면 코드가 알아서 추가해줍니다.
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // 시작하자마자 소리나는 것 방지

        // 3. 버튼 컴포넌트 찾고 클릭 이벤트 연결하기
        btn = GetComponent<Button>();
        if (btn != null)
        {
            // 버튼이 클릭되면 PlaySound 함수를 실행하라고 명령
            btn.onClick.AddListener(PlaySound);
        }
    }

    void Update()
    {
        // (기존 깜빡임 코드 유지)
        if (buttonText != null)
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) / 2.0f;
            Color textColor = buttonText.color;
            textColor.a = alpha;
            buttonText.color = textColor;
        }
    }

    // 소리를 재생하는 함수
    void PlaySound()
    {
        // 효과음 파일과 오디오 소스가 모두 있을 때만 재생
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound); // 효과음은 PlayOneShot이 적합합니다
        }
    }
}