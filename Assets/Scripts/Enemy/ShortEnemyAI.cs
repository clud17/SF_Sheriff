using System.Collections;
using UnityEngine;

public class ShortEnemyAI : EnemyAI
{
    private PlayerMove playerMove;
    RevolverHealthSystem revolverHealthSystem;
    Animator animator;
    GameObject knifeObject;
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음
        playerMove = player.GetComponent<PlayerMove>(); // 플레이어 이동 스크립트 가져오기(넉백을 위해 필요)
        revolverHealthSystem = player.GetComponent<RevolverHealthSystem>(); // 플레이어의 리볼버 체력 시스템 가져오기
        knifeObject = transform.Find("knife").gameObject;

        HP = GetComponent<Health>();
        HP.maxHealth = 25f; //최대 체력 설정

        detectionRange = 15f;
        attackRange = 3.0f;
        moveSpeed = 4f;
        isPlayerDetected = false;

        damage = 1; // 공격력 설정
        attackCycle = 1.0f; // 공격 쿨타임 설정
        isAttacking = false; // 공격 중인지 여부 초기화
        knockbackRange = 4.0f; // 넉백 거리 설정

        //animator = GetComponent<Animator>();
    }
    protected override IEnumerator EnemyAttack()
    {
        if (isAttacking) return null; // 이미 공격 중이면 중복 공격 방지       

        if (revolverHealthSystem != null)   // 데미지 적용
        {
            knifeObject.GetComponent<knife>().GetValue(knockbackRange, damage);

        }
        return base.EnemyAttack();
    }

}