using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 씬 전환 기능을 위해 반드시 필요합니다!

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string[] sentences;
    private int index;
    public float typingSpeed = 0.05f;

    [Header("소원 관련 설정")]
    public GameObject wishInputPanel;      // 소원 입력창 패널
    public TMP_InputField wishInputField;  // 소원을 입력하는 필드
    public static string savedWish;        // 엔딩 씬에서도 사용하기 위해 static으로 선언

    [Header("시작 연출 이미지")]
    public GameObject startImage;          // 첫 대사 때 띄울 이미지

    private bool isWaitingForInput = false;

    void Start()
    {
        // 1. 소원 입력창은 처음에 꺼둡니다.
        if (wishInputPanel != null) wishInputPanel.SetActive(false);
        
        // 2. 튜토리얼 시작 시 이미지를 보여줍니다.
        if (startImage != null) startImage.SetActive(true); 

        dialogueText.text = "";
        StartCoroutine(Type());
    }

    void Update()
    {
        // 소원을 입력하는 중에는 대화가 넘어가지 않게 방지합니다.
        if (isWaitingForInput) return;

        // 클릭이나 스페이스바를 누르면 다음 대사로 진행합니다.
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueText.text == sentences[index])
            {
                NextSentence();
            }
        }
    }

    IEnumerator Type()
    {
        dialogueText.text = "";
        foreach (char letter in sentences[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {
        // Element 1 대사가 끝나면 소원 입력창을 띄웁니다.
        if (index == 1 && !isWaitingForInput)
        {
            ShowWishInput();
            return;
        }

        // 아직 보여줄 대사가 남아있다면 진행합니다.
        if (index < sentences.Length - 1)
        {
            index++;
            StartCoroutine(Type());
        }
        else
        {
            Debug.Log("모든 튜토리얼이 끝났습니다. 로딩 씬으로 이동합니다.");
            SceneManager.LoadScene("level01_Intro"); 
        }
    }

    void ShowWishInput()
    {
        isWaitingForInput = true;
        if (wishInputPanel != null) wishInputPanel.SetActive(true);
    }

    // 소원 입력 완료 버튼(Button)에 이 함수를 연결하세요.
    public void OnWishInputComplete()
    {
        if (wishInputField != null)
        {
            savedWish = wishInputField.text; // 입력받은 소원을 static 변수에 저장
            Debug.Log("저장된 소원: " + savedWish);
        }

        if (wishInputPanel != null) wishInputPanel.SetActive(false);
        isWaitingForInput = false;
        
        index++;
        StartCoroutine(Type());
    }
}