using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class Rat : NetworkBehaviour
{
    private PlayerMovement shooterRat; // ���������������
    public LayerMask wallLayer; // ǽ�����ڵĲ�
    public float speed = 5f; // ������ƶ��ٶ�
    public float detectionRadius = 10f; // �����ҵİ뾶
    public int damage = 10; // ����������ɵ��˺�
    public LayerMask playerLayer; // ������ڵĲ�
    private Vector3 originalScale; // �洢ԭʼ������ֵ
    private Transform target; // �����Ŀ�����
    private Animator animator;
    private bool isActive = false; // ���������Ƿ񼤻�
    private void Start()
    {
        if(shooterRat==null)
        {
            Debug.Log("û��shooter");
        }
        animator = GetComponent<Animator>();
        originalScale = transform.localScale; // �洢ԭʼ������ֵ
        StartCoroutine(ActivateAfterDelay(2f));
        // �ҵ����� "Pistol" ������
        GameObject pistol = GameObject.FindWithTag("RatGun");

        if (pistol != null)
        {
            Debug.Log("���ҵ�RatGun");
            Transform parentObject = pistol.transform.parent; // ��ȡ������
            if (parentObject != null)
            {
                string killerName = parentObject.name; // ��ȡ�����������
                string victimName = "Rat"; // �ܺ��ߵ����֣�����Ը�������޸�
                int killStreak = 1; // ɱ¾������������Ը�������޸�

                // ���û�ɱ֪ͨ�� RpcNotifyKill ����
                KillFeed killFeed = FindObjectOfType<KillFeed>();
                if (killFeed != null)
                {
                    killFeed.RpcNotifyKill(killerName, victimName, killStreak);
                }
            }
        }
    }
    IEnumerator ActivateAfterDelay(float delay)
    {
        // �ȴ�ָ�����ӳ�ʱ��
        yield return new WaitForSeconds(delay);
        animator = GetComponent<Animator>();
        // �����������Ϊ
        isActive = true;
    }
    void Update()
    {
        if (!isActive) return; // �������δ�����ִ�к�������
        //if (!isServer) return;
        //Debug.Log("������");
        // �����������
        DetectNearestPlayer();

        // ����ҵ�Ŀ�꣬��Ŀ���ƶ�
        if (target != null)
        {
            Vector2 direction = (Vector2)(target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            // ������������ƶ�����תͼƬ�����򣬱��ֲ���
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // ��תͼƬ
            }
            else
            {
                transform.localScale = originalScale; // ����תͼƬ
            }
        }
        else
        {
            // �������û��Ŀ�꣬������canMoveΪfalse
            if (animator != null)
            {
                animator.SetBool("canMove", false);
            }
        }
    }

    private void DetectNearestPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        float minDistance = float.MaxValue;
        Debug.Log("��⵽Ŀ��");
        foreach (Collider2D hitCollider in hitColliders)
        {
            // ����������
            if (shooterRat != null && hitCollider.gameObject == shooterRat.gameObject)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = hitCollider.transform; // ����Ϊ�µ�Ŀ��
            }
        }
    }

    public void SetShooter(PlayerMovement shooter)
    {
        shooterRat = shooter; // ���÷��������������
        //Debug.Log("Shooter has been set to: " + shooter.gameObject.name); // ���ڵ���
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �����ײ�Ķ����Ƿ���ǽ�ڲ�
        if ((wallLayer & 1 << collision.gameObject.layer) != 0)
        {
            // �����������ǽ�ڣ��������Լ�
            Destroy(this.gameObject);
            return; // ��ǰ���أ�����������߼�
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log(playerMovement.playerName.ToString());
                Debug.Log("�����������");
                playerMovement.TakeDamage(damage); // ���������˺�
                Debug.Log(playerMovement.currentHP.ToString());
                if (playerMovement.isDead && shooterRat != null) // ���������������з�����
                {
                    Debug.Log("�ҵ��㲥����");

                    // ���ӷ����ߵķ���
                    PlayerMovement killerPlayer = shooterRat.GetComponent<PlayerMovement>();
                    shooterRat.AddScore(1);

                    // ���ӷ����ߵ� killStreak
                    shooterRat.killStreak += 1;

                    // ������ɱ
                    KillFeed killFeed = FindObjectOfType<KillFeed>();

                    if (killFeed != null)
                    {
                        Debug.Log("�ҵ��㲥");
                        string killerName = shooterRat.playerName; // ��ȡ�����ߵ�����
                        string victimName = playerMovement.playerName; // ��ȡ�ܺ��ߵ�����
                        killFeed.RpcNotifyKill(killerName, victimName, shooterRat.killStreak);
                        Debug.Log(shooterRat.killStreak.ToString());
                    }
                    else
                    {
                        Debug.Log("δ�ҵ��㲥");
                    }
                }
                Destroy(this.gameObject);
            }
        }
    }

}
