using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� ����ϱ� ���� �ʿ�

public class RevolverHealthSystem : MonoBehaviour
{
    [Header("ü�� ����")]
    public int maxHealth = 60; // �ִ� ü�� (�Ѿ� ���� ���� * 10���� ����, ��: 60)
    private int currentHealth; // ���� �÷��̾��� ü��

    [Header("�Ѿ� ����")]
    public int maxBullets = 6; // �������� �ִ� �Ѿ� ���� ���� (�ѱ� ����)
    private int currentUsableBullets; // ���� ���� �� �� �ִ� (������ ����) �Ѿ� ���� ����
    private bool[] bulletBlockedStatus; // �� �Ѿ� ������ �������� ���θ� ������ �迭 (true = ����)

    [Header("UI ����")]
    // �ν����Ϳ��� ������� �Ѿ� ���� UI Image�� �Ҵ��ؾ� �մϴ� (��: bulletSlot1, bulletSlot2...)
    public Image[] bulletSlots;
    public Sprite filledBulletSprite;   // �Ѿ��� ä���� ��������Ʈ (���� ��)
    public Sprite blockedBulletSprite;  // �ѱ��� ������ ���� ��������Ʈ (������ ��)

    // �ʱ�ȭ
    void Start()
    {
        currentHealth = maxHealth; // ü���� �ִ�� ����
        currentUsableBullets = maxBullets; // ���� �� ��� �ѱ��� ��� ���� (������ ����)

        // �� �Ѿ� ������ ���� ���¸� �ʱ�ȭ (��� '������ ����' ���·� ����)
        bulletBlockedStatus = new bool[maxBullets];
        for (int i = 0; i < maxBullets; i++)
        {
            bulletBlockedStatus[i] = false; // �⺻������ ������ ���� ����
        }

        UpdateUI(); // ���� �� UI�� �ʱ� ����(��� ä���� �ѱ�)�� ������Ʈ
    }

    // �������� �޴� �Լ�
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // ���� ü�¿��� ������ ����

        // ü���� 0 �̸����� �������� �ʵ��� ����
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // --- �ٽ� ����: ü�¿� ���� �Ѿ� ���� ���� (������ ��������Ʈ�� ����) ---
        // �Ѿ� 1���� �ʿ��� ü��(������) ���� ��� (��: 60 / 6 = 10 �������� �Ѿ� 1��)
        int healthPerBullet = maxHealth / maxBullets;

        // ���� ü������ ���� �� �� �ִ� �ִ� �Ѿ� ���� ���� ���
        int newUsableBulletCount = Mathf.CeilToInt((float)currentHealth / healthPerBullet);

        // ���� �� �� �ִ� �Ѿ� ������ �پ����ٸ� (��, �������� �޾� �ѱ��� ������ �Ѵٸ�)
        if (newUsableBulletCount < currentUsableBullets)
        {
            // �پ�� ������ŭ �Ѿ� ������ �ڿ������� ���� (bulletBlockedStatus�� true�� ����)
            for (int i = currentUsableBullets - 1; i >= newUsableBulletCount; i--)
            {
                bulletBlockedStatus[i] = true; // �ش� ������ ���� ���·� ����
            }
            currentUsableBullets = newUsableBulletCount; // �� �� �ִ� �Ѿ� ���� ������Ʈ

            Debug.Log($"�������� �޾� �ѱ� ������ {currentUsableBullets}�� ���ҽ��ϴ�. (�� {maxBullets - currentUsableBullets}�� ����)");

            // �ѱ� ������ ��� ������ �� ���� ���� ó��
            if (currentUsableBullets <= 0)
            {
                Debug.Log("���� ����! ��� �ѱ� ������ �������ϴ�.");
                // ���⿡ ���� ���� ó�� ���� �߰� (��: ���� �����, Ư�� UI Ȱ��ȭ ��)
            }
        }

        UpdateUI(); // UI ������Ʈ �Լ� ȣ�� (���� �ѱ��� ������ ��������Ʈ�� ����)
    }

    // ü���� ȸ���ϴ� �Լ�
    public void Heal(int amount)
    {
        currentHealth += amount; // ���� ü�¿� ȸ���� �߰�

        // ü���� �ִ� ü���� ���� �ʵ��� ����
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // --- �ٽ� ����: ü�� ȸ���� ���� ���� �ѱ� �ٽ� Ȱ��ȭ ---
        // �Ѿ� 1���� �ʿ��� ü��(������) ���� ���
        int healthPerBullet = maxHealth / maxBullets;

        // ���� ü������ ���� �� �� �ִ� �ִ� �Ѿ� ���� ���� ���
        int newUsableBulletCount = Mathf.CeilToInt((float)currentHealth / healthPerBullet);

        // ���� �� �� �ִ� �Ѿ� ������ �þ�ٸ� (��, ü���� ȸ���Ͽ� �ѱ��� Ȱ��ȭ�ؾ� �Ѵٸ�)
        if (newUsableBulletCount > currentUsableBullets)
        {
            // �þ ������ŭ �Ѿ� ������ �տ������� Ȱ��ȭ (bulletBlockedStatus�� false�� ����)
            // ��, ������ �ѱ��� �ٽ� ä���� ���·� �ǵ���
            for (int i = currentUsableBullets; i < newUsableBulletCount; i++) // ����: currentUsableBullets���� ����
            {
                if (i < maxBullets) // �迭 ������ ����� �ʵ��� Ȯ��
                {
                    bulletBlockedStatus[i] = false; // �ش� ������ ������ ���� ���·� ����
                }
            }
            currentUsableBullets = newUsableBulletCount; // �� �� �ִ� �Ѿ� ���� ������Ʈ

            Debug.Log($"ü�� ȸ������ �ѱ� ������ {currentUsableBullets}�� ���ҽ��ϴ�.");
        }

        UpdateUI(); // UI ������Ʈ �Լ� ȣ�� (���� �ѱ��� �ٽ� ä���� ��������Ʈ�� ����)
    }


    // ���� ��� �Լ� (����� ������/ȸ�� �׽�Ʈ�� ����)
    public void Shoot()
    {
        Debug.Log("���� ��� ����� ���� �������� �ʾҽ��ϴ�. ������/ȸ�� �׽�Ʈ�� �����մϴ�.");
        // ���߿� ���� ��� ������ ���⿡ �߰��˴ϴ�.
    }

    // UI�� ������Ʈ�ϴ� �Լ� (�ѱ� ���� ��������Ʈ ����)
    void UpdateUI()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            if (bulletBlockedStatus[i]) // �ش� �ѱ� ������ �����ٸ�
            {
                bulletSlots[i].sprite = blockedBulletSprite; // ���� ��������Ʈ (������ ���׶��) ǥ��
            }
            else // ������ ������ �ʾҴٸ� (��� ������ �ѱ����)
            {
                bulletSlots[i].sprite = filledBulletSprite; // ä���� ��������Ʈ ǥ��
            }
        }
    }

    // �ܺο��� ���� ���� �ѱ� ������ �������� �Լ� (�ʿ�� ���)
    public int GetCurrentUsableBullets()
    {
        return currentUsableBullets;
    }
}