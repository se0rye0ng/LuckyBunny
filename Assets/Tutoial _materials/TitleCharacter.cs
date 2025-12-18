using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleCharacter : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 150f;          // 이동 속도 (인스펙터에서 토끼마다 다르게 설정하세요)
    public float changeTargetTime = 2.5f;    // 방향 전환 주기
    public RectTransform canvasRect;         // 토끼가 움직일 영역 (Canvas 연결)

    [Header("방향별 이미지")]
    public Sprite frontSprite;  // 앞
    public Sprite backSprite;   // 뒤
    public Sprite leftSprite;   // 왼쪽
    public Sprite rightSprite;  // 오른쪽

    [Header("설정")]
    public bool isTutorialStarter = false;  // 체크하면 클릭 시 튜토리얼 시작

    private Vector2 targetPosition;
    private RectTransform rectTransform;
    private Image characterImage;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        characterImage = GetComponent<Image>();
        
        // [중요] 모든 토끼가 동시에 방향을 바꾸지 않도록 랜덤 지연 시간을 줍니다.
        // 이를 통해 네 마리 토끼가 서로 다른 타이밍에 목적지를 바꿉니다.
        float randomDelay = Random.Range(0f, changeTargetTime);
        InvokeRepeating("SetRandomTarget", randomDelay, changeTargetTime);
        
        SetRandomTarget(); // 첫 번째 목표는 즉시 설정
    }

    void Update()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        
        // 목표 지점으로 부드럽게 이동
        rectTransform.anchoredPosition = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.deltaTime);

        // 아주 조금이라도 움직이고 있다면 방향 이미지를 업데이트
        if (Vector2.Distance(currentPos, targetPosition) > 0.1f)
        {
            UpdateCharacterDirection(targetPosition - currentPos);
        }
    }

    void UpdateCharacterDirection(Vector2 direction)
    {
        // 좌우 이동량이 상하 이동량보다 클 때 (좌우 이미지)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            characterImage.sprite = (direction.x > 0) ? rightSprite : leftSprite;
        }
        // 상하 이동량이 더 클 때 (앞뒤 이미지)
        else
        {
            characterImage.sprite = (direction.y > 0) ? backSprite : frontSprite;
        }
    }

    void SetRandomTarget()
    {
        if (canvasRect == null) return;

        // 캔버스 크기의 90% 영역 내에서 랜덤한 좌표를 생성합니다.
        float rangeX = (canvasRect.rect.width / 2) * 0.9f;
        float rangeY = (canvasRect.rect.height / 2) * 0.9f;

        float x = Random.Range(-rangeX, rangeX);
        float y = Random.Range(-rangeY, rangeY);
        
        targetPosition = new Vector2(x, y);
    }

    // 캐릭터(버튼) 클릭 시 실행되는 함수
    public void OnCharacterClick()
    {
        if (isTutorialStarter)
        {
            Debug.Log("진짜 토끼 클릭! 튜토리얼을 시작합니다.");
            SceneManager.LoadScene("TutorialScene"); // 씬 이름이 정확해야 합니다.
        }
        else
        {
            Debug.Log("이 토끼는 가짜입니다!");
        }
    }
}