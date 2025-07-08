using UnityEngine;
using UnityEngine.XR;

public class PlayerMove : MonoBehaviour
{
    //캐릭터의 좌우,점프,대시 등의 전반적인 움직임을 구현하는 코드

    [SerializeField] private float moveSpeed = 5f; // 이속
    [SerializeField] private float jumpSpeed = 5f; // 점프높이
    [SerializeField] private float maxJumpTime = 0.4f; // 최대점프시간

    //isJumping 확인용 필드
    [SerializeField] private LayerMask groundLayer; 
    
    private Collider2D col;
    private Rigidbody2D rb;
    private Animator anim;

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

    private bool isKnockback = false; // 넉백 상태 플래그

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rb.linearDamping = 0f;  // 감속 없음
        rb.gravityScale = 2f;
        dashSpeed = 30f;
        // sr = GetComponent<SpriteRenderer>();
        // anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isKnockback) return; //넉백중이면 이동X

        HandleMovement();
        HandleJump();
    }
    //좌우이동 및 대시 기능
    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > lastDashTime + dashCooldown)
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
            isJumping = false;
        }
    }
    
    private bool IsGrounded()
    {
        float offsetY = -(col.bounds.extents.y + 0.5f); // 오프셋 = 캐릭터 길이 절반 + 여유
        float checkRadius = 0.1f;
        Vector2 checkPosition = (Vector2)transform.position + new Vector2(0f, offsetY); // 착지판정기준 = 캐릭터 중심 + 오프셋 = 캐릭터 발끝 + 여유
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        // Collider2D hit = Physics2D.OverlapCircle(checkPosition, checkRadius, groundLayer);
        // Debug.Log(hit);
        return Physics2D.OverlapCircle(checkPosition, checkRadius, groundLayer);// 착지판정기준 좌표에서 radius만큼 주변에 GroundLayer가 있는가?
    }

    // 땅에 닿으면 넉백 상태 해제
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // 바닥만 인식
        {
            isKnockback = false;
        }
    }

    // 외부에서 넉백 시작할 때 호출
    public void ApplyKnockback(Vector2 force)
    {
        isKnockback = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);       // 넉백 구현
    }

}
