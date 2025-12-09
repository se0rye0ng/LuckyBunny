using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject badItemPrefab;

    public float xRange = 2.3f;
    public float yHeight = 6f;
    public float spawnInterval = 0.5f;

    public Color[] rainbowColors = new Color[] {
        Color.red, new Color(1f, 0.5f, 0f), Color.yellow,
        Color.green, Color.cyan, Color.blue, new Color(0.5f, 0f, 1f)
    };

    void Start()
    {
        InvokeRepeating("SpawnRandomItem", 0f, spawnInterval);
    }

    void SpawnRandomItem()
    {
        float randomX = Random.Range(-xRange, xRange);
        Vector3 spawnPos = new Vector3(randomX, yHeight, 0);

        if (Random.value < 0.2f)
        {
            Instantiate(badItemPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            GameObject newItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            
            int randomIndex = Random.Range(0, rainbowColors.Length);

            SpriteRenderer sr = newItem.GetComponent<SpriteRenderer>();
            FallingItem itemScript = newItem.GetComponent<FallingItem>();

            if (sr != null)
            {
                sr.color = rainbowColors[randomIndex];
            }

            if (itemScript != null)
            {
                itemScript.SetColorInfo(randomIndex);
            }
        }
    }
}