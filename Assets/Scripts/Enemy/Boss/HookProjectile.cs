using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HookProjectile : MonoBehaviour
{
    private float speed = 25f;           // 초당 25타일
    private float maxDistance = 25f;     // 너무 멀리 가면 강제 종료(안전장치)
    private float maxLifeTime = 2.0f;    // 2초 지나면 강제 종료(안전장치)

    // 감속(플레이어 x좌표 도달 후 0.2초간 0으로 감소)
    private const float decelDuration = 0.2f;

    // Runtime
    private Transform player;
    private Vector2 fireDir;
    private Vector2 startPos2D;
    private Vector2 targetPos2D;

    private Action onHit;
    private Action<Vector3> onMiss;

    private bool finished;
    private bool reachedPlayerX;
    private bool decelerating;
    private float decelTimer;
    private float lifeTimer;

    /// <summary>
    /// 보스가 발사 직전 호출.
    /// dirSign: +1 오른쪽, -1 왼쪽
    /// </summary>
    public void Init(Transform target, Vector2 fireDir, Action onHit, Action<Vector3> onMiss)
    {
        player = target;

        this.fireDir = fireDir.sqrMagnitude < 0.0001f ? Vector2.right : fireDir.normalized;
        this.onHit = onHit;
        this.onMiss = onMiss;

        startPos2D = transform.position;
        targetPos2D = target.position;

        finished = false;
        reachedPlayerX = false;
        decelerating = false;
        decelTimer = 0f;
        lifeTimer = 0f;
    }

    private void Awake()
    {
        // Trigger 세팅 안전장치
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (finished) return;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= maxLifeTime)
        {
            FinishMiss(transform.position);
            return;
        }

        // 최대 이동거리 제한
        if (Vector3.Distance(startPos2D, transform.position) > maxDistance)
        {
            FinishMiss(transform.position);
            return;
        }

        // 플레이어 x좌표 도달 체크 → 감속 시작
        if (!reachedPlayerX)
        {
            float travelled = Vector2.Dot((Vector2)transform.position - startPos2D, fireDir);
            float targetProj = Vector2.Dot(targetPos2D - startPos2D, fireDir);
            
            if(travelled >= targetProj)
            {
                reachedPlayerX = true;
                decelerating = true;
                decelTimer = 0f;
            }
        }

        // 이동/감속 처리
        float currentSpeed = speed;

        if (decelerating)
        {
            decelTimer += Time.deltaTime;
            float t = Mathf.Clamp01(decelTimer / decelDuration);
            currentSpeed = Mathf.Lerp(speed, 0f, t);

            // 감속 완료 → 이 위치를 미스(정지 지점)로 확정
            if (t >= 1f)
            {
                FinishMiss(transform.position);
                return;
            }
        }

        transform.position += (Vector3)(fireDir * currentSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (finished) return;

        if (other.CompareTag("Player"))
        {
            FinishHit();
            return;
        }

        // 바닥/벽(환경) 태그가 Ground면 미스 처리
        if (other.CompareTag("Ground"))
        {
            FinishMiss(transform.position);
            return;
        }
    }

    private void FinishHit()
    {
        if (finished) return;
        finished = true;

        onHit?.Invoke();
        Destroy(gameObject);
    }

    private void FinishMiss(Vector3 pos)
    {
        if (finished) return;
        finished = true;

        onMiss?.Invoke(pos);
        Destroy(gameObject);
    }
}
