using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DefaultGun : baseGun
{
    public override void InitSetting()
    {
        base.InitSetting(); // �θ� Ŭ������ InitSetting ȣ��
        gundata.fireRate = 0.25f; // �߻� ���� ����
        gundata.nextFireTime = 0f; // �ʱ�ȭ
        gundata.AmmoreloadTime = 0.5f; // ������ �ð� ����
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
