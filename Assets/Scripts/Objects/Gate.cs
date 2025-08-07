using UnityEngine;
using System.Collections;
public class Gate : MonoBehaviour
{
    public SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void MoveGate(Vector2 dir){ this.MoveGate(dir, 0.5f); }
    public void MoveGate(Vector2 dir, float time)
    {
        StartCoroutine(MoveOverTime(dir,time));
    }
    IEnumerator MoveOverTime(Vector2 dir, float time)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, sr.bounds.size.y * ((dir == Vector2.up) ? 1f : -1f) , 0);

        float elapsed = 0f;

        while (elapsed < time)
        {
            float t = elapsed / time;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos; 
    }

}
