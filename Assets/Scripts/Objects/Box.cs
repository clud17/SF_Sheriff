using UnityEngine;

public class Box : MonoBehaviour
{
    private bool isPlayerNearby = false;
    public Sprite closedBoxSprite;
    public Sprite openedBoxSprite;
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = closedBoxSprite;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Box가 열렸습니다.");
            gameObject.GetComponent<SpriteRenderer>().sprite = openedBoxSprite;
        }
    }
}
