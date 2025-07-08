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

    public virtual void Fire(GameObject player, Transform tip)
    { // �Ѿ� �߻� �޼ҵ�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        // ȸ�� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(currentAmmo + "�̹Ƿ� " + myBullets[currentAmmo].bulletName + "�� �߻�˴ϴ�.");
        Debug.Log(gundata.currentAmmo);
        BulletBase now = WC.myBullets[gundata.currentAmmo--];
        Debug.Log(now);
        now.gameObject.SetActive(true);                         // ������Ʈ �ٽ� Ȱ��ȭ
        now.transform.position = tip.position;            // �߻� ��ġ
        now.SetDirection(direction);                            // ���� ����
        now.transform.rotation = rotation;                      // ȸ�� ����
        now.Fire();
    }
    public virtual IEnumerator DelayedShoot(GameObject player, Transform tip)     
    {
        if (gundata.isCharging || gundata.isReloading) yield break; // �̹� ���� ���̸� ����
        gundata.isCharging = true;
        yield return new WaitForSeconds(0.3f); // 0.3�� ���  //������ �����̽ð�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;

        // ȸ�� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        //�Ѿ� �߻�
        BulletBase now = WC.myBullets[gundata.currentAmmo];
        Debug.Log(now);
        now.gameObject.SetActive(true);                         // ������Ʈ �ٽ� Ȱ��ȭ
        now.transform.position = tip.position;            // �߻� ��ġ
        now.SetDirection(direction);                            // ���� ����
        now.transform.rotation = rotation;                      // ȸ�� ����
        now.Fire();

        gundata.currentAmmo = 0; // �������� ��������Ƿ� ���� ź���� 0���� ����

        //�÷��̾� �˹�
        Vector2 knockbackDir = new Vector2(-direction.x, -direction.y).normalized;     // �˹� ���� ���� (x���� ����/������, y���� �������� ����)

        // ���� ĳ������ rigidbody2D�� ������
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 knockbackForce = knockbackDir * 8f;                         // �˹� ���� ����(����+��)

            player.GetComponent<PlayerMove>().ApplyKnockback(knockbackForce);   // �÷��̾� �̵� ��ũ��Ʈ�� �˹� ���� �Լ� ȣ��
        }
        gundata.isCharging = false; // �ٽ� �߻� ��������
    }
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
