using UnityEngine;

public class BossSceneDoorController : MonoBehaviour
{
    [SerializeField] Collider2D leftDoor;
    [SerializeField] Collider2D rightDoor;
    
    [SerializeField] private BossHealth BH;

    void Start()
    {
        if (leftDoor != null) leftDoor.enabled = true;
        if (rightDoor != null) rightDoor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(BH.GetIsDeath() == true)
        {
            OpenDoor();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player")) return;
        CloseDoor();

    }
    private void OpenDoor()
    {
        if (leftDoor != null) leftDoor.enabled = false;
        if (rightDoor != null) rightDoor.enabled = false;
    }
    private void CloseDoor()
    {
        if (leftDoor != null) leftDoor.enabled = true;
        if (rightDoor != null) rightDoor.enabled = true;
    }
}
