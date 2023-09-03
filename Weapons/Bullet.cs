
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public Gun shotFromGun;
    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector2 syncedPosition;//子弹同步位置
    public Transform muzzlePos;
    public GameObject owner;
    public float speed;
    [SyncVar]
    public Vector2 mousePos;
    public GameObject explosionPrefab;
    new protected Rigidbody2D rigidbody; // 使用 new 关键字以隐藏基类中的字段
    [SyncVar(hook = nameof(OnSpriteIndexChanged))]
    public int syncedSpriteIndex = 0;
    public int damage;
    [SyncVar]
    public Vector2 direction;
    [SyncVar]
    public Vector2 direction1;
    public NetworkIdentity shooter;// 发射这个子弹的玩家
    public PlayerMovement shotBy;  // 记录这个子弹是由谁发射的
    [SyncVar]
    public uint shooterNetId;

    // 设置 shooter 属性时，同时设置 shooterNetId 属性
    public void SetShooter(NetworkIdentity shooterIdentity)
    {
        shooter = shooterIdentity;
        shooterNetId = shooterIdentity.netId;
    }
    void Awake()
    {
        //isLocalBullet = shooter == NetworkClient.localPlayer.GetComponent<NetworkIdentity>();
        rigidbody = GetComponent<Rigidbody2D>();
    }
    public  void OnSpriteIndexChanged(int oldIndex, int newIndex)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        CountTrackBullet countTrackBullet = GetComponent<CountTrackBullet>();

        if (countTrackBullet && countTrackBullet.bulletSprites.Length > newIndex)
        {
            spriteRenderer.sprite = countTrackBullet.bulletSprites[newIndex];
        }
    }


    private void OnPositionChanged(Vector2 oldValue, Vector2 newValue)
    {
        if (!isServer)
        {
            rigidbody.position = newValue;
        }
    }
    public virtual void Start()
    {
        direction = (mousePos - direction1).normalized;
        float angel = Random.Range(-5f, 5f);
        rigidbody.velocity = Quaternion.AngleAxis(angel, Vector3.forward) * direction * speed;
        OnSpriteIndexChanged(-1, syncedSpriteIndex);
    }


    public virtual void Update()
    {
        if (isServer)
        {
            // 更新同步的位置
            syncedPosition = rigidbody.position;
        }
    }


    public virtual void OnTriggerEnter2D(Collider2D other)//碰撞要在服务器上运行
    {
        if (!isServer) // 如果不是在服务器上，直接返回。
        {
            return;
        }
        if (other.gameObject.name == "mirror") 
        {
            int mirrorBulletLayer = LayerMask.NameToLayer("mirrorbullet");
            gameObject.layer = mirrorBulletLayer;
           
            Vector2 reflectedDirection = Quaternion.Euler(0f, 0f, Random.Range(175f, 185f)) * rigidbody.velocity;       
            rigidbody.velocity = reflectedDirection;
            syncedPosition = rigidbody.position;
           
            return;
        }
        else
        {
            // 处理击中事件
            CmdDealDamage(other.gameObject);

            GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
            exp.transform.position = transform.position;
            NetworkServer.Spawn(exp);
            //ObjectPool.Instance.PushObject(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
  

    public virtual void CmdDealDamage(GameObject hitPlayer)
    {
        
        if (!isServer)
            return;
        if (shooter == null)
        {
            Debug.LogError("Shooter is 空");
            return;
        }
        //if (hitPlayer.GetComponent<NetworkIdentity>() == shooter)
        //    return;
        PlayerMovement playerMovement = hitPlayer.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            if (hitPlayer.CompareTag("Player") && playerMovement.currentHP > 0)
            {
                playerMovement.lastDamagedBy = shooter; // 记录击中玩家的子弹的发射者
                playerMovement.TakeDamage(damage);
                if (playerMovement.isDead) // If the player is dead after taking damage
                {
                    PlayerMovement killerPlayer = shooter.GetComponent<PlayerMovement>();
                    shooter.GetComponent<PlayerMovement>().AddScore(1); // Add 1 point to the shooter
                }
            }
        }
        BreakableBox breakableBox = hitPlayer.GetComponent<BreakableBox>();
        if (breakableBox != null)
        {
            if (hitPlayer.CompareTag("Box"))
            {
                breakableBox.TakeDamage(damage);
            }
        }
        Grass grass = hitPlayer.GetComponent<Grass>();
        if (grass != null)
        {
            if (hitPlayer.CompareTag("Grass"))
            {
                grass.TakeDamage(damage);

            }
        }
    }
}



