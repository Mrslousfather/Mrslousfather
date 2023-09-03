using UnityEngine;
using Mirror;

public class HomingMissile : Bullet
{
    private int initialSpriteIndex; // 保存初始的图片索引
    public float detectionRadius = 5f; // 检测半径
    public LayerMask playerLayer; // 玩家层
    public float rotationSpeed = 200f; // 子弹转向速度
    private Transform target; // 锁定的目标
    public Sprite[] trackBulletSprites;
    [SyncVar]
    private uint targetNetId;
    [SyncVar(hook = nameof(OnChangeBulletSprite))]
    public int currentBulletSprite;
    private Gun myGun;
    public override void Start()
    {
        base.Start();
        if(shotFromGun!=null)
        {
            currentBulletSprite = shotFromGun.gunBulletIndex;  // Update bullet sprite.
        }         
        transform.rotation = Quaternion.identity;
    }
    private void Update()
    {
        // 如果不是服务器，则不执行逻辑
        //if (!isServer)
        //    return;
        Debug.Log("是服务器");
        // 如果没有目标或者目标的网络ID变了，则尝试检测目标
        if (target == null || target.GetComponent<NetworkIdentity>().netId != targetNetId)
        {
            DetectTarget();
        }

        // 如果有目标，则朝目标移动
        if (target != null)
        {
            Vector2 direction = (Vector2)(target.position - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            rigidbody.velocity = transform.right * speed; // 使用继承的 rigidbody 和 speed           
        }
    }
    private void OnChangeBulletSprite(int oldSprite, int newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = trackBulletSprites[newSprite];
    }


    // 检测目标的函数
    // 检测目标的函数
    private void DetectTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            NetworkIdentity targetIdentity = hitCollider.GetComponent<NetworkIdentity>();
            if (targetIdentity != null && targetIdentity.netId == shooterNetId)
            {
                continue; // 如果碰撞体是发射子弹的玩家，则跳过
            }

            // 锁定第一个检测到的目标
            target = hitCollider.transform;
            targetNetId = targetIdentity.netId; // 同步目标的网络ID
            break;
        }
    }


}