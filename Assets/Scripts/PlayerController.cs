using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 7f;
    public float itemHeight = 1f;
    public GameObject clearText;
    public float friction = 2.5f;
    public float xLimit = 8.0f;

    private float currentVelocity = 0f;

    [SerializeField]
    private int stackCount = 0;
    private const int maxStack = 7;

    void Update()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        
        float targetVelocity = xInput * maxSpeed;
        currentVelocity = Mathf.Lerp(currentVelocity, targetVelocity, friction * Time.deltaTime);

        transform.Translate(Vector3.right * currentVelocity * Time.deltaTime);

        Vector3 currentPos = transform.position;
        currentPos.x = Mathf.Clamp(currentPos.x, -xLimit, xLimit);
        transform.position = currentPos;
    }

    public int AddItemToStack(GameObject item, bool isBadItem, int itemColorIndex)
    {
        if (isBadItem || (itemColorIndex != stackCount))
        {
            if (stackCount > 0)
            {
                DropTopItem();
            }
            
            Destroy(item);
            return 1;
        }

        if (stackCount >= maxStack) return 0;

        float diffX = item.transform.position.x - transform.position.x;
        bool isPerfect = Mathf.Abs(diffX) < 0.2f;

        item.transform.SetParent(transform);
        
        float targetX = isPerfect ? 0 : diffX;
        
        item.transform.localPosition = new Vector3(targetX, (stackCount + 1) * itemHeight, 0);
        item.transform.localRotation = Quaternion.identity;

        stackCount++;

        if(stackCount == maxStack)
        {
            if(clearText != null) clearText.SetActive(true);
            Time.timeScale = 0;
        }

        return isPerfect ? 2 : 1;
    }

    void DropTopItem()
    {
        if (transform.childCount == 0) return;

        Transform topItem = transform.GetChild(transform.childCount - 1);
        
        EnablePhysics(topItem.gameObject);
        
        stackCount--;
    }

    void EnablePhysics(GameObject obj)
    {
        obj.transform.SetParent(null);
        
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        BoxCollider2D col = obj.GetComponent<BoxCollider2D>();

        if (rb != null)
        {
            rb.simulated = true;
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(Random.Range(-1f, 1f), 2f), ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-30f, 30f));
        }
        if (col != null) col.enabled = true;
    }
}