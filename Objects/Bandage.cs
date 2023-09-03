using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : MonoBehaviour
{
    public int healAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // 确认碰撞的对象是玩家
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Heal(healAmount);  // 恢复玩家血量
                Debug.Log(healAmount);
                Destroy(gameObject);  // 销毁绷带对象
            }
        }
    }
}
