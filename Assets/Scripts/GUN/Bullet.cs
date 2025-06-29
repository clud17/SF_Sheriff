using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 300f;
    public float lifetime = 2f;
    private Vector2 moveDirection;

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // 총알이 2초 후 자동 파괴
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World); //space.World을 사용하여 월드 좌표계에서 이동(아니면 방향이 안맞음.)
    }
}
