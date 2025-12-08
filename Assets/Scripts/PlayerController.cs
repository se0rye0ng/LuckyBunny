using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;            
    public float itemHeight = 1f;       
    public GameObject clearText;

    [SerializeField]
    private int stackCount = 0;         
    private const int maxStack = 7;     

    void Update()
    {
        float xInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * xInput * speed * Time.deltaTime);
    }

    public bool AddItemToStack(GameObject item)
    {
        if (stackCount >= maxStack) return false; 

        item.transform.SetParent(transform);
        item.transform.localPosition = new Vector3(0, (stackCount + 1) * itemHeight, 0);
        item.transform.localRotation = Quaternion.identity;

        stackCount++;
        
        if(stackCount == maxStack)
        {
            Debug.Log("게임 클리어!");
            if(clearText != null) clearText.SetActive(true);
            Time.timeScale = 0; 
        }

        return true;
    }
}