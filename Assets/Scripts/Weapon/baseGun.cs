using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Data
{
    public float fireRate;      // �߻� ����
    public float nextFireTime; // �Ѿ� �߻� ������
    public bool isCharging;
    public bool isReloading; // ������ ������ �ƴ���
    public float AmmoreloadTime; // �������ð�
    //HP ���� �ʵ�
    public int maxHP;// �ִ� ü��
    public int currentHP;    // ���� ü�� (�ǰ� �� ����)
    public int currentAmmo;  // ���� ���� ź���� ����

}


public abstract class baseGun : MonoBehaviour
{
    public Data gundata; // ���� ������ ����ü
    public WeaponController WC;
    public virtual void InitSetting()
    {
        gundata.isCharging = false;
        gundata.isReloading = false; // ������ ������ �ƴ���
        gundata.maxHP = 6; // �ִ� ü��
        gundata.currentHP = gundata.maxHP; // ���� ü���� �ִ� ü������ �ʱ�ȭ
        gundata.currentAmmo = gundata.currentHP; // ���� ź���� ���� ü������ �ʱ�ȭ
        

    }

    public abstract void Fire(GameObject player, Transform tip);
    public abstract IEnumerator DelayedShoot(GameObject player, Transform tip);
    public virtual IEnumerator ReloadAmmo()
    {
        gundata.isReloading = true;
        Debug.Log("������..");
        yield return new WaitForSeconds(gundata.AmmoreloadTime);
        Debug.Log("���� �Ϸ�");

        gundata.currentAmmo = gundata.currentHP; // ���� ü���� ���� źâ����

        gundata.isReloading = false;
    }
}
