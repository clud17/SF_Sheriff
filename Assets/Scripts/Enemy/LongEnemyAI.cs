using System.Collections;
using UnityEngine;

public class LongEnemyAI : EnemyAI
{
    public Transform Enemytip; // 총구 위치
    public GameObject EnemyBullet; // 현재 총알 오브젝트
    GameObject spawnedBullet;
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음

        HP = GetComponent<Health>();
        HP.maxHealth = 15.0f; //최대 체력 설정
        HP.currentEnemyHealth = HP.maxHealth; // 현재 체력 초기화

        detectionRange = 15.0f;
        attackRange = 15.0f;
        moveSpeed = 3.0f;
        isPlayerDetected = false;

        damage = 1; // 공격력 설정
        attackCycle = 2.0f; // 공격 쿨타임 설정
        isAttacking = false; // 공격 중인지 여부 초기화
        knockbackRange = 2.0f; // 넉백 거리 설정

        //Enemytip = transform.Find("EnemyGun/tip"); // 총구 위치 설정
    }
    protected override IEnumerator EnemyAttack()
    {
        // 총알 방향 계산
        Vector3 ToPlayerPos = player.position;
        Vector2 ToPlayerdirection = (ToPlayerPos - Enemytip.position).normalized; // 플레이어 방향 벡터 계산

        // 마우스 방향과 총구(tip) 위치를 이용해 방향 벡터 계산
        float Playerangle = Mathf.Atan2(ToPlayerdirection.y, ToPlayerdirection.x) * Mathf.Rad2Deg;
        Quaternion Playerrotation = Quaternion.Euler(0, 0, Playerangle);

        spawnedBullet = Instantiate(EnemyBullet, Enemytip.position, Playerrotation);

        // 적군 총 발사
        spawnedBullet.GetComponent<EnemyBullet>().EnemyShoot(ToPlayerdirection);
        // 넉백 값 전달하기
        spawnedBullet.GetComponent<EnemyBullet>().GetValue(knockbackRange, damage);
        
        yield return base.EnemyAttack();
    }
}