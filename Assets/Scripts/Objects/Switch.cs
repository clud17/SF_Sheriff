using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject[] MoveObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void TurnSwitch()
    {
        foreach (GameObject MoveObject in MoveObjects)
        {
            MoveObject.GetComponent<Gate>().MoveGate(Vector2.down);
        }

    }
    
}
