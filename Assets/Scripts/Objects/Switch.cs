using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject[] DownObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TurnSwitch()
    {
        foreach (GameObject DownObject in DownObjects)
        {
            SpriteRenderer sr = DownObject.GetComponent<SpriteRenderer>();
            Vector3 newPosition = DownObject.transform.position;
            newPosition.y -= sr.bounds.size.y;
            DownObject.transform.position = newPosition;
        }

    }
    
}
