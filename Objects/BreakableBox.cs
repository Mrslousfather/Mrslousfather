using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BreakableBox : NetworkBehaviour
{
    public int maxHealth ;// 箱子的最大生命值
    [SyncVar]
    public int currentHealth;      // 当前生命值

    public Sprite spriteBelowHealth; // 血量小于50时的图片

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

        // 更新箱子外观
        if (currentHealth <= maxHealth/2)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = spriteBelowHealth;
        }

        // 检查生命值是否小于等于0
        if (currentHealth <= 0)
        {
            // 摧毁物体
            Destroy(gameObject);
        }
    }
    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
    }
}
