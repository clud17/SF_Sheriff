using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public baseGun currentGun; // ���� ������ ��
    public GameObject player; // �÷��̾� ������Ʈ
    public Transform tip; // �Ѿ� �߻� ��ġ

    public BulletBase[] myBullets = new BulletBase[7];
    public GameObject[] bulletPrefabs;
    public GameObject ChargeBulletObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �⺻ źȯ�� ������ �����ϴ� �迭. 0: �⺻, 1: charge(�ٲ���� ��Ŭ������ ����ǰ�), 2: ���� ���. ���߿� �޽��� �� ��, �ٲܼ� �־����
        int[] startBullet = { 1, 3, 1, 3, 3, 1 }; // ABCDEF.
        for (int i = 6; i >= 1; i--) // �⺻ źȯ 6��.
        {
            //�Ѿ� �������� �̸� ����� �ΰ�, Ŭ������ ��ť�� �ִ´�.
            GameObject bulletObj = Instantiate(bulletPrefabs[startBullet[i - 1]]);
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            myBullets[7 - i] = bullet;
            //myBullets[i] = bullet;
        }
        // �� �ڵ带 ������ ���� myBullets �迭�� { ,F,E,D,C,B,A}�� �ȴ�
        ChargeBulletObj = Instantiate(bulletPrefabs[0]); // bulletPrefabs[0] 

        currentGun.InitSetting(); // �� �ʱ�ȭ ����
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGun.gundata.isReloading) return; // ������ ���̸� �ƹ��͵� ���� ����

        if (Input.GetMouseButtonDown(0) && Time.time >= currentGun.gundata.nextFireTime)
        { //��Ŭ�� �ڵ� ����
            if (currentGun.gundata.currentAmmo == 0) Debug.Log("need to reload"); //����źȯ ť�� ���������
            else // ź���� ������
            {
                currentGun.Fire(player, tip); // �߻�
                Debug.Log($"{currentGun.gundata.currentAmmo}");
                currentGun.gundata.nextFireTime = Time.time + currentGun.gundata.fireRate;  // �߻� �ֱ⸦ �����ϱ� ����

            }
        }
        if (Input.GetMouseButtonDown(1))
        { // ��Ŭ�� �ڵ� ����
            if (currentGun.gundata.currentAmmo == 0) Debug.Log("need to reload");
            else
            {
                StartCoroutine(currentGun.DelayedShoot(player, tip));
                Debug.Log($"{currentGun.gundata.currentAmmo}");

                StartCoroutine(currentGun.ReloadAmmo());  // ��Ŭ���� ���� �Ѿ� ������� ������
            }
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            // ª�� ������ ��� �� �Ѿ� ������
            if (!currentGun.gundata.isReloading)
            {
                StartCoroutine(currentGun.ReloadAmmo());
            }       
        }
    }
}
