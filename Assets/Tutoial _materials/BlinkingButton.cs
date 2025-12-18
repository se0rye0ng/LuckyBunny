using UnityEngine;
using TMPro; // 텍스트 제어를 위해 반드시 필요

public class BlinkingButton : MonoBehaviour
{
    public float blinkSpeed = 2.0f; // 깜빡이는 속도 (숫자가 클수록 빠름)
    private TextMeshProUGUI buttonText;

    void Start()
    {
        // 버튼의 자식 오브젝트에 있는 텍스트 컴포넌트를 찾습니다
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (buttonText != null)
        {
            // Mathf.Sin을 사용해 0~1 사이를 부드럽게 반복하는 투명도 값을 만듭니다
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) + 1.0f) / 2.0f;

            // 텍스트의 현재 색상을 가져와 투명도(a)만 변경합니다
            Color textColor = buttonText.color;
            textColor.a = alpha;
            buttonText.color = textColor;
        }
    }
}