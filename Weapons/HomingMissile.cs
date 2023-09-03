using UnityEngine;
using Mirror;

public class HomingMissile : Bullet
{
    private int initialSpriteIndex; // �����ʼ��ͼƬ����
    public float detectionRadius = 5f; // ���뾶
    public LayerMask playerLayer; // ��Ҳ�
    public float rotationSpeed = 200f; // �ӵ�ת���ٶ�
    private Transform target; // ������Ŀ��
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
        // ������Ƿ���������ִ���߼�
        //if (!isServer)
        //    return;
        Debug.Log("�Ƿ�����");
        // ���û��Ŀ�����Ŀ�������ID���ˣ����Լ��Ŀ��
        if (target == null || target.GetComponent<NetworkIdentity>().netId != targetNetId)
        {
            DetectTarget();
        }

        // �����Ŀ�꣬��Ŀ���ƶ�
        if (target != null)
        {
            Vector2 direction = (Vector2)(target.position - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            rigidbody.velocity = transform.right * speed; // ʹ�ü̳е� rigidbody �� speed           
        }
    }
    private void OnChangeBulletSprite(int oldSprite, int newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = trackBulletSprites[newSprite];
    }


    // ���Ŀ��ĺ���
    // ���Ŀ��ĺ���
    private void DetectTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            NetworkIdentity targetIdentity = hitCollider.GetComponent<NetworkIdentity>();
            if (targetIdentity != null && targetIdentity.netId == shooterNetId)
            {
                continue; // �����ײ���Ƿ����ӵ�����ң�������
            }

            // ������һ����⵽��Ŀ��
            target = hitCollider.transform;
            targetNetId = targetIdentity.netId; // ͬ��Ŀ�������ID
            break;
        }
    }


}