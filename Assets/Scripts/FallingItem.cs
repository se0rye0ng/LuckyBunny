using UnityEngine;

public class FallingItem : MonoBehaviour
{
    public int colorIndex = -1; 
    private bool isFinished = false;

    public void SetColorInfo(int index)
    {
        colorIndex = index;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isFinished) return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                bool isBad = gameObject.CompareTag("Enemy");
                int result = player.AddItemToStack(this.gameObject, isBad, colorIndex);

                if (result == -1) 
                {
                    isFinished = true;
                    return; 
                }
                else if (result > 0) 
                {
                    isFinished = true;
                    if (!isBad)
                    {
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();
                        if (rb != null) { rb.linearVelocity = Vector2.zero; rb.bodyType = RigidbodyType2D.Kinematic; rb.simulated = false; }
                        BoxCollider2D col = GetComponent<BoxCollider2D>();
                        if (col != null) col.enabled = false;
                    }
                }
            }
        }
        else if (other.CompareTag("Finish"))
        {
            isFinished = true;
            Destroy(gameObject);
        }
    }
}