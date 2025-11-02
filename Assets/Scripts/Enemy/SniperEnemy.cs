using System.Collections;
using UnityEngine;

public class SniperEnemy : EnemyAI
{
    public Transform Enemytip; // 총구 위치
    public GameObject EnemyBullet; // 현재 총알 오브젝트
    private LineRenderer laserLine; // 레이저 라인 렌더러
    GameObject spawnedBullet;
    protected override void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // 플레이어의 Transform을 찾음

        laserLine = GetComponent<LineRenderer>(); // 레이저 라인 렌더러 컴포넌트 가져오기

        HP = GetComponent<Health>();
        HP.maxHealth = 5.0f; //최대 체력 설정

        detectionRange = 30.0f;
        attackRange = 30.0f;
        moveSpeed = 0.0f;
        isPlayerDetected = false;

        damage = 1; // 공격력 설정
        attackCycle = 5.0f; // 공격 쿨타임 설정
        isAttacking = false; // 공격 중인지 여부 초기화
        knockbackRange = 7.0f; // 넉백 거리 설정

        Enemytip = transform.Find("EnemySniperGun/tip"); // 총구 위치 설정
    }
    protected override IEnumerator EnemyAttack()
    {
        if (isAttacking) yield break;
        isAttacking = true;
        EnemyAnimator.SetBool("isAttacking", true);

        // 총알 방향 계산
        Vector3 ToPlayerPos = player.position;
        Vector2 ToPlayerdirection = (ToPlayerPos - Enemytip.position).normalized; // 플레이어 방향 벡터 계산

        // 마우스 방향과 총구(tip) 위치를 이용해 방향 벡터 계산
        float Playerangle = Mathf.Atan2(ToPlayerdirection.y, ToPlayerdirection.x) * Mathf.Rad2Deg;
        Quaternion Playerrotation = Quaternion.Euler(0, 0, Playerangle);

        yield return ShowLaser(Enemytip.position, ToPlayerdirection, 0.5f);
 
        HideLaser();

        spawnedBullet = Instantiate(EnemyBullet, Enemytip.position, Playerrotation);

        // 적군 총 발사
        spawnedBullet.GetComponent<EnemyBullet>().EnemyShoot(ToPlayerdirection);
        // 넉백 값 전달하기
        spawnedBullet.GetComponent<EnemyBullet>().GetValue(knockbackRange, damage);


        Debug.Log($"{attackCycle}초 후 공격");
        yield return new WaitForSeconds(attackCycle); // 공격 쿨타임 대기
        isAttacking = false; // 공격 종료

        // yield return base.EnemyAttack();  // 부모의 EnemyAttack 호출을 안함
    }
    IEnumerator ShowLaser(Vector3 start, Vector2 dir, float duration)
    {
        float t = 0f;
        if (laserLine == null) yield break;

        laserLine.enabled = true;
        laserLine.material.color = Color.red;
        laserLine.positionCount = 2;

        while (t < duration)
        {
            t += Time.deltaTime;
            laserLine.SetPosition(0, start);
            laserLine.SetPosition(1, start + (Vector3)dir * attackRange);
            yield return null;
        }
    }
    void HideLaser()
    {
        if (laserLine == null) return;
        laserLine.enabled = false;
    }
}
