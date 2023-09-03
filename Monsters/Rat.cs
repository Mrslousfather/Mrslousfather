using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class Rat : NetworkBehaviour
{
    private PlayerMovement shooterRat; // 发射这个老鼠的玩家
    public LayerMask wallLayer; // 墙壁所在的层
    public float speed = 5f; // 老鼠的移动速度
    public float detectionRadius = 10f; // 检测玩家的半径
    public int damage = 10; // 老鼠对玩家造成的伤害
    public LayerMask playerLayer; // 玩家所在的层
    private Vector3 originalScale; // 存储原始的缩放值
    private Transform target; // 老鼠的目标玩家
    private Animator animator;
    private bool isActive = false; // 控制老鼠是否激活
    private void Start()
    {
        if(shooterRat==null)
        {
            Debug.Log("没有shooter");
        }
        animator = GetComponent<Animator>();
        originalScale = transform.localScale; // 存储原始的缩放值
        StartCoroutine(ActivateAfterDelay(2f));
        // 找到带有 "Pistol" 的物体
        GameObject pistol = GameObject.FindWithTag("RatGun");

        if (pistol != null)
        {
            Debug.Log("已找到RatGun");
            Transform parentObject = pistol.transform.parent; // 获取父物体
            if (parentObject != null)
            {
                string killerName = parentObject.name; // 获取父物体的名字
                string victimName = "Rat"; // 受害者的名字，你可以根据情况修改
                int killStreak = 1; // 杀戮连击数，你可以根据情况修改

                // 调用击杀通知的 RpcNotifyKill 方法
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
        // 等待指定的延迟时间
        yield return new WaitForSeconds(delay);
        animator = GetComponent<Animator>();
        // 激活老鼠的行为
        isActive = true;
    }
    void Update()
    {
        if (!isActive) return; // 如果老鼠未激活，则不执行后续代码
        //if (!isServer) return;
        //Debug.Log("服务器");
        // 检测最近的玩家
        DetectNearestPlayer();

        // 如果找到目标，朝目标移动
        if (target != null)
        {
            Vector2 direction = (Vector2)(target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            // 如果老鼠向右移动，则反转图片；否则，保持不变
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z); // 反转图片
            }
            else
            {
                transform.localScale = originalScale; // 不反转图片
            }
        }
        else
        {
            // 如果老鼠没有目标，则设置canMove为false
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
        Debug.Log("检测到目标");
        foreach (Collider2D hitCollider in hitColliders)
        {
            // 跳过发射者
            if (shooterRat != null && hitCollider.gameObject == shooterRat.gameObject)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, hitCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                target = hitCollider.transform; // 设置为新的目标
            }
        }
    }

    public void SetShooter(PlayerMovement shooter)
    {
        shooterRat = shooter; // 设置发射这个老鼠的玩家
        //Debug.Log("Shooter has been set to: " + shooter.gameObject.name); // 用于调试
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 检测碰撞的对象是否在墙壁层
        if ((wallLayer & 1 << collision.gameObject.layer) != 0)
        {
            // 如果老鼠碰到墙壁，则销毁自己
            Destroy(this.gameObject);
            return; // 提前返回，避免后续的逻辑
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Debug.Log(playerMovement.playerName.ToString());
                Debug.Log("老鼠碰到玩家");
                playerMovement.TakeDamage(damage); // 对玩家造成伤害
                Debug.Log(playerMovement.currentHP.ToString());
                if (playerMovement.isDead && shooterRat != null) // 如果玩家死亡并且有发射者
                {
                    Debug.Log("找到广播了吗");

                    // 增加发射者的分数
                    PlayerMovement killerPlayer = shooterRat.GetComponent<PlayerMovement>();
                    shooterRat.AddScore(1);

                    // 增加发射者的 killStreak
                    shooterRat.killStreak += 1;

                    // 播报击杀
                    KillFeed killFeed = FindObjectOfType<KillFeed>();

                    if (killFeed != null)
                    {
                        Debug.Log("找到广播");
                        string killerName = shooterRat.playerName; // 获取发射者的名字
                        string victimName = playerMovement.playerName; // 获取受害者的名字
                        killFeed.RpcNotifyKill(killerName, victimName, shooterRat.killStreak);
                        Debug.Log(shooterRat.killStreak.ToString());
                    }
                    else
                    {
                        Debug.Log("未找到广播");
                    }
                }
                Destroy(this.gameObject);
            }
        }
    }

}
