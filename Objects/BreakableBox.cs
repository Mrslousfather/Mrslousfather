using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakableBox : NetworkBehaviour
{
    public int maxHealth ;// ���ӵ��������ֵ
    [SyncVar]
    public int currentHealth;      // ��ǰ����ֵ

    public Sprite spriteBelowHealth; // Ѫ��С��50ʱ��ͼƬ

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

        // �����������
        if (currentHealth <= maxHealth/2)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = spriteBelowHealth;
        }

        // �������ֵ�Ƿ�С�ڵ���0
        if (currentHealth <= 0)
        {
            // �ݻ�����
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
    }
}
