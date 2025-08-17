using UnityEngine;
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
    private SpriteRenderer sprend;

    private bool isJumping = false;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb.linearDamping = 0f;  // 감속 없음
        rb.gravityScale = 3.0f;
        dashSpeed = 30f;

        moveSpeed = 6.4f; // 이속
        jumpSpeed = 14f; // 점프높이
        maxJumpTime = 0.1f; // 최대점프시간
        sprend = GetComponent<SpriteRenderer>();
        // anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        moveX = 0f;
        if (!isKnockback)
        { //넉백중이면 이동X
            moveX = Input.GetAxisRaw("Horizontal");  // 좌우 이동 입력

            if (moveX < 0) // 좌측 이동
            {
                sprend.transform.localScale = new Vector3(-1f, 1f, 1f); // 좌측 이동시 스프라이트 반전
            }
            else if (moveX > 0) // 우측 이동
            {
                sprend.transform.localScale = new Vector3(1f, 1f, 1f); // 우측 이동시 스프라이트 원래대로
            }
        }
        HandleMovement();
        HandleJump();
    }
    //좌우이동 및 대시 기능
    private void HandleMovement()
    {
        // float moveX = Input.GetAxisRaw("Horizontal");
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

        if (!isDashing) // 대시 중이 아니라면 기본 이동
        {
            rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        }
        else // 대시 중이면
        {

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                //Debug.Log("isDashing이 false입니다.");
                anim.SetBool("isDashing", false);
                rb.gravityScale = 2f;
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
        else if (Input.GetKeyUp(KeyCode.Space)) // 스페이스에서 키를 떼면
        {
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }
            isJumping = false;
        }
    }

    private bool IsGrounded()
    {
        float offsetY = -(col.bounds.extents.y + 0.1f); // 발끝에서 살짝 아래
        Vector2 boxSize = new Vector2(col.bounds.size.x, 0.2f); // 캐릭터 너비의 90%, 높이는 얇게
        Vector2 checkPosition = (Vector2)transform.position + new Vector2(0f, offsetY);
        LayerMask groundLayer = LayerMask.GetMask("Ground");

        return Physics2D.OverlapBox(checkPosition, boxSize, 0f, groundLayer);
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
}
