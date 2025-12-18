using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingDialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string[] sentences;
    private int index;
    public float typingSpeed = 0.05f;

    [Header("대사별 이미지 설정")]
    public GameObject image0; // Element 0에서 보일 이미지
    public GameObject image1; // Element 1에서 보일 이미지

    void Start()
    {
        // 시작 시 모든 이미지를 끄고 첫 번째만 켭니다.
        if (image0 != null) image0.SetActive(true);
        if (image1 != null) image1.SetActive(false);

        if (sentences.Length >= 3)
        {
            sentences[2] = "네가 간직했던 소원인 [" + DialogueSystem.savedWish + "], \n내가 꼭 이뤄줄게!";
        }
        
        dialogueText.text = "";
        StartCoroutine(Type());
    }

    // ... Update, Type 함수는 이전과 동일 ...

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            
            // 대사 인덱스에 따라 이미지 교체 처리
            if (index == 1) // Element 1이 시작될 때
            {
                if (image0 != null) image0.SetActive(false); // 0번 이미지 끄기
                if (image1 != null) image1.SetActive(true);  // 1번 이미지 켜기
            }
            
            StartCoroutine(Type());
        }
    }

    // (기존 Update와 Type 코루틴은 유지해주세요)
    void Update()
    {
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
}