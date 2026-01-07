using NUnit.Framework;
using UnityEngine;
using System;

public class BossHealth : Health
{
    public int Phase {get; private set;} = 1; // 보스 페이즈 추적용 속성(Property)

    public event Action OnPhase2Request; // 페이즈 2 진입 요청 이벤트
    public event Action<int> OnPhaseChanged; // 페이즈 변경 이벤트
    // [추가설명] 페이즈가 바뀌었다는 사실을 BossAI가 알아야 하므로 이벤트를 사용

    private bool isInvicible; // 무적 상태 플래그 // 페이즈 변화 시 잠시 무적 상태로 만들기 위해 필요

    protected override void Awake(){
        base.Awake();
        Phase = 1;
        isInvicible = false;
    }

    public override void ApplyDamage(float damage)
    {
        if(isInvicible) return;

        base.ApplyDamage(damage);

        // 보스 체력이 100 이하로 떨어지면 페이즈 변경
        if (Phase == 1 && currentEnemyHealth <= 300.0f)
        {
            isInvicible = true; // 무적 상태로 변경 // 무적상태 해제는 MiddleBoss1AI.cs에서 페이즈 전환 애니메이션 끝난 후에 처리
            OnPhase2Request?.Invoke();
            Debug.Log($"{gameObject.name} has entered phase 2!");
        }

        // 추후 기절하는 로직 추가 해야함
    }

    // AI가 페이즈를 확정할 때 호출하는 메서드
    public void SetPhase(int phase)
    {
        Phase = phase;
        OnPhaseChanged?.Invoke(Phase);

    }
    public void SetInvicible(bool value)
    {
        isInvicible = value;
    }
    
    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}