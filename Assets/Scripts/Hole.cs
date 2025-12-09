using UnityEngine;
using System.Collections;

public class Hole : MonoBehaviour
{
    public SpriteRenderer itemRenderer; 
    
    [Header("연결된 토끼굴 (여기에 짝꿍 구멍을 넣으세요!)")]
    public Hole connectedHole; // 이게 있어야 Inspector에 뜹니다!

    [Header("클로버 이미지들")]
    public Sprite redClover;
    public Sprite greenClover;
    public Sprite pinkClover;

    public bool isActive = false; 
    public int currentType = -1;  
    private Coroutine activeRoutine; 

    public void SpawnClover()
    {
        // 연결된 구멍(토끼굴)이라면 클로버 생성 금지
        if (connectedHole != null) return; 

        if (isActive) return; 

        float rand = Random.value;
        if (rand < 0.1f) 
        {
            currentType = 2; 
            itemRenderer.sprite = pinkClover;
        }
        else if (rand < 0.55f)
        {
            currentType = 0; 
            itemRenderer.sprite = redClover;
        }
        else
        {
            currentType = 1; 
            itemRenderer.sprite = greenClover;
        }

        isActive = true;
        itemRenderer.color = Color.white; 
        activeRoutine = StartCoroutine(LifeCycleRoutine());
    }

    // ... (LifeCycleRoutine, ResetHole, OnInteract 등 나머지 코드는 기존과 동일)
    // 아래 내용이 지워졌다면 복사해서 채워주세요.
    
    IEnumerator LifeCycleRoutine()
    {
        float totalLifeTime = 3.0f; 
        float changeTime = Random.Range(1.0f, 2.0f); 
        bool willChange = (Random.value < 0.5f); 
        if (currentType == 2) willChange = false;
        float timer = 0f;
        while (timer < changeTime) { timer += Time.deltaTime; yield return null; }
        if (willChange) {
            for (int i = 0; i < 5; i++) {
                itemRenderer.color = new Color(1, 1, 1, 0.2f); yield return new WaitForSeconds(0.05f);
                itemRenderer.color = Color.white; yield return new WaitForSeconds(0.05f);
            }
            timer += 0.5f; 
            if (currentType == 0) { currentType = 1; itemRenderer.sprite = greenClover; }
            else if (currentType == 1) { currentType = 0; itemRenderer.sprite = redClover; }
        }
        while (timer < totalLifeTime) { timer += Time.deltaTime; yield return null; }
        ResetHole();
    }
    public void ResetHole() {
        isActive = false; itemRenderer.sprite = null; currentType = -1;
        if (activeRoutine != null) StopCoroutine(activeRoutine);
    }
    public int OnInteract() {
        if (!isActive) return -1; 
        int gatheredType = currentType; ResetHole(); return gatheredType; 
    }
}