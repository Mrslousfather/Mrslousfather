using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : MonoBehaviour
{
    public int healAmount = 50;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // ȷ����ײ�Ķ��������
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.Heal(healAmount);  // �ָ����Ѫ��
                Debug.Log(healAmount);
                Destroy(gameObject);  // ���ٱ�������
            }
        }
    }
}
