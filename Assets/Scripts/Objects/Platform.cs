using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Collider2D colliders;
    public GameObject player;

    private bool isTriggered;

    void Awake()
    {
        colliders = GetComponent<Collider2D>();
        isTriggered = false;
    }
    void Update()
    {
        //
        //
        //
        if (player.GetComponent<Rigidbody2D>().linearVelocityY > 0)
        {
            colliders.enabled = false;
        }
        else
        {
            colliders.enabled = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            DisablePlatformsTemporarily(0.9f);
        }

        if (isTriggered)
        {
            colliders.enabled = false;
        }
        else
        {
            colliders.enabled = true;
        }
    }
    public void DisablePlatformsTemporarily(float duration)
    {
        StartCoroutine(DisableRoutine(duration));
    }
    private IEnumerator DisableRoutine(float duration)
    {
        isTriggered = true;
        yield return new WaitForSeconds(duration);
        isTriggered = false;
    }
}
