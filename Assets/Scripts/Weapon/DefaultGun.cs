using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DefaultGun : baseGun
{
    public override void InitSetting()
    {
        base.InitSetting(); // 부모 클래스의 InitSetting 호출
        gundata.fireRate = 0.25f; // 발사 간격 설정
        gundata.nextFireTime = 0f; // 초기화
        gundata.AmmoreloadTime = 0.5f; // 재장전 시간 설정
    }
    public override void Fire(GameObject player, Transform tip)
    {
        base.Fire(player, tip);
    }
    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
    {
        return base.DelayedShoot(player, tip);
    }
    public override IEnumerator ReloadAmmo()
    {
        return base.ReloadAmmo();
        
    }
}
