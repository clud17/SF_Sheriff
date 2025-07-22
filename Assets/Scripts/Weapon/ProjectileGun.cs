using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;
public class ProjectileGun : BaseGun
{
    //public BulletBase
    public override void InitSetting()
    {
        base.InitSetting(); // �θ� Ŭ������ InitSetting ȣ��
        gundata.fireRate = 0.25f; // �߻� ���� ����
        gundata.nextFireTime = 0f; // �ʱ�ȭ
        gundata.AmmoreloadTime = 1.5f; // ������ �ð� ����

    }
    public override void Fire(GameObject player, Transform tip)
    {
        // �Ѿ� �߻� �޼ҵ�
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - tip.position).normalized;
        // ȸ�� ���� ���
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        //Debug.Log(currentAmmo + "�̹Ƿ� " + myBullets[currentAmmo].bulletName + "�� �߻�˴ϴ�.");
        Debug.Log(gundata.currentAmmo);
        
        GameObject now = WC.myBulletObj[gundata.currentAmmo--];
        GameObject spawnedBullet = Instantiate(now, tip.position, rotation);
        Debug.Log(spawnedBullet);
        spawnedBullet.GetComponent<BulletBase>().Projectile();
    }
    public override IEnumerator DelayedShoot(GameObject player, Transform tip)
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

        GameObject now = WC.myBulletObj[0];
        now.GetComponent<BulletBase>().InitFromData();
        //now.GetComponent<BulletBase>().moveDirection
        GameObject spawnedBullet = Instantiate(now, tip.position, rotation);
        spawnedBullet.GetComponent<BulletBase>().Projectile();

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
    public override IEnumerator ReloadAmmo()
    {
        return base.ReloadAmmo();

    }
}