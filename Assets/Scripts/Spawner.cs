using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public float xRange = 2.3f; 
    public float yHeight = 6f;

    public Color[] rainbowColors = new Color[] {
        Color.red,
        new Color(1f, 0.5f, 0f), // 주황
        Color.yellow,
        Color.green,
        Color.cyan,
        Color.blue,
        new Color(0.5f, 0f, 1f)  // 보라
    };

    private int colorIndex = 0; 

    void Start()
    {
        // 게임 시작 시 첫 번째 아이템 생성
        SpawnItem();
    }

    // [중요] 이제 InvokeRepeating을 안 씁니다. 한 번만 부릅니다.
    void SpawnItem()
    {
        float randomX = Random.Range(-xRange, xRange);
        Vector3 spawnPos = new Vector3(randomX, yHeight, 0);

        GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();
        if (sr != null && colorIndex < rainbowColors.Length)
        {
            sr.color = rainbowColors[colorIndex];
        }
    }

    // [새로운 기능] 아이템이 사라질 때 호출하는 함수
    public void RequestNextItem(bool isSuccess)
    {
        if (isSuccess)
        {
            // 성공했으면 다음 색깔 준비
            if (colorIndex < rainbowColors.Length - 1)
            {
                colorIndex++;
            }
        }
        // 실패했으면 colorIndex 그대로 유지 (같은 색 나옴)

        // 바로 나오면 너무 빠르니까 0.5초 뒤에 생성
        Invoke("SpawnItem", 0.5f);
    }
}