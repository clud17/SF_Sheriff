using System;
using UnityEngine;
using System.Collections;
using UnityEngine.U2D.Animation;
using UnityEngine.XR;

public class PlayerMove : MonoBehaviour
{
    //캐릭터의 좌우,점프,대시 등의 전반적인 움직임을 구현하는 코드
    float moveX;
    private float moveSpeed; // 이속
    private float jumpSpeed; // 점프높이
    private float maxJumpTime; // 최대점프시간
    private GameObject lastTouchedObject;
    //isJumping 확인용 필드
    [SerializeField] private LayerMask groundLayer;

    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private bool isJumping = false;
    private float jumpTimeCounter = 0f;

    /*─────대시기능 구현 필드────────*/
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashTime = 0.2f; // 대시하는 시간
    [SerializeField] private float dashCooldown = 0.5f; // 대시 쿨타임
    private bool isDashing = false;
    private float dashTimer = 0f; // 대시 시간 체크용
    private float lastDashTime = -999f; // Time.time과 함께 쿨다운 체크용
    /*────────────────────────────*/

    public bool isKnockback = false; // 넉백 상태 플래그
    private bool canMove = true; // 움직임 가능 여부 플래그 ex) 기절당했을 때
    private bool isExternallyControlled = false; // 외부에서 움직임을 통제하는지 확인하는 플래그

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb.linearDamping = 0f;  // 감속 없음
        rb.gravityScale = 3.0f;
        dashSpeed = 30f;

        moveSpeed = 6.4f; // 이속
        jumpSpeed = 17.5f; // 점프높이
        maxJumpTime = 0.1f; // 최대점프시간
    }

    // Update is called once per frame
    void Update()
    {
        // 보스에게 기절 당했을 때, 움직임을 제한하기 위해 사용
        if (!canMove || isExternallyControlled) return;

        if (!isKnockback) // 넉백중이 아닐때만 이동및 대시 점프 구현
        {
            HandleMovement();
            HandleJump();
            HandlePlatform();
            //...
        }
        HandleDashTimer(); // 대시타이머는 언제든지 흘러감
    }
    //좌우이동 및 대시 기능
    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX < 0) // 좌측 이동
        {
            transform.localScale = new Vector3(-0.55f, 0.55f, 0.55f);
            anim.SetBool("isMoving", true);
        }
        else if (moveX > 0) // 우측 이동
        {
            transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown && IsGrounded())
        {
            isDashing = true;
            //Debug.Log("isDashing이 true입니다.");
            anim.SetBool("isDashing", true);
            dashTimer = dashTime;
            lastDashTime = Time.time;
            rb.gravityScale = 0f; // 공중에서도 평평하게 대시하기 위해
            rb.linearVelocity = new Vector2(moveX * dashSpeed, 0f);
        }
        if (!isDashing) // 대시 중 이동 방지
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);
        }
        moveX = 0f;
    }
    private void HandleDashTimer() //
    {
        if (!isDashing){return;}
        else // 대시 중이면
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                //Debug.Log("isDashing이 false입니다.");
                anim.SetBool("isDashing", false);
                rb.gravityScale = 3f;
            }
        }
    }
    //점프 기능
    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) // 스페이스에서 키를 떼면
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
            isJumping = false;
        }
    }

    /*---------플레이어가 S키 누르면 플랫폼 내려가는 거 구현하는 코드입니다.-------*/
    private void HandlePlatform()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector2 feetPoint = new Vector2(col.bounds.center.x, col.bounds.min.y);
            RaycastHit2D hit = Physics2D.Raycast(feetPoint, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
            if (hit.collider != null && hit.collider.CompareTag("Platform"))
            {
                StartCoroutine(DisableCollisionTemporarily(hit.collider));
            }
        }
    }
    private System.Collections.IEnumerator DisableCollisionTemporarily(Collider2D platformCol)
    {
        Physics2D.IgnoreCollision(col, platformCol, true);
        yield return new WaitForSeconds(0.9f); // 0.9초 동안 충돌 무시
        Physics2D.IgnoreCollision(col, platformCol, false);

    }
    /*----------------*/
    private bool IsGrounded()
    {
        // 발끝 위치 계산
        float offsetY = -(col.bounds.extents.y + 0.1f);
        Vector2 origin = (Vector2)transform.position;

        // Ground 레이어만 감지하도록 LayerMask 설정
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        // 발끝에서 아래로 레이캐스트 쏘기
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, col.bounds.extents.y + 0.1f, groundLayer);

        // 디버그용 레이 표시
        Debug.DrawRay(origin, Vector2.down * (col.bounds.extents.y + 0.1f), Color.red);

        return hit.collider != null;
    }



    // 땅에 닿으면 넉백 상태 해제
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥만 인식
        {
            isKnockback = false;
        }
        if (collision.gameObject.CompareTag("Spike")) // 바닥만 인식
        {
            transform.position = lastTouchedObject.transform.position;
        }
    }
    // isTrigger 켜놓은 오브젝트 감지용
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Return"))
        {
            lastTouchedObject = other.gameObject;
        }
    }

    // 외부에서 넉백 시작할 때 호출
    public void ApplyKnockback(Vector2 force)
    {
        isKnockback = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);       // 넉백 구현
        Debug.Log("넉백 적용 됨");
    }
    
    // 외부에서 플레이어의 움직임을 제한할 때 쓰는 메소드
    public void SetExternalControl(bool on)
    {
        isExternallyControlled = on;

        if (on)
        {
            canMove = false;

            // 상태 정리(대시/이동 애니메이션 포함)
            isDashing = false;
            anim.SetBool("isDashing", false);
            anim.SetBool("isMoving", false);
            dashTimer = 0f;

            // 물리 정리
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        else
        {
            canMove = true;
        }
    }
}
