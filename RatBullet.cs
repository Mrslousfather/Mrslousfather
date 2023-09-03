using UnityEngine;
using System.Collections;
using Mirror;

public class RatBullet : Bullet
{
    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector2 syncedPosition;
    public float maxDistance = 20f; // 子弹的最大射程
    public float detectionRadius = 5f; // 检测半径
    public LayerMask playerLayer; // 玩家层
    public float rotationSpeed = 200f; // 子弹转向速度
    public float waitingTime = 3f; // 在原地等待的时间

    private Transform target; // 锁定的目标
    private Vector3 startPosition; // 子弹的初始位置
    private bool hasReachedMaxDistance = false; // 是否达到最大距离
    private float timeSinceReachedMaxDistance = 0f; // 达到最大距离后的经过时间
    public LayerMask targetLayer; // 指定目标 Layer
    [SyncVar]
    private uint targetNetId;
    Animator mouseMove;
    // 在类的顶部添加一个变量来存储每次移动的方向
    private Vector2 currentMoveDirection;
    bool canMove = true;
    private AnimatorStateInfo stateInfo; // 用于存储状态信息
    [ClientRpc]
    private void RpcUpdateBulletRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
    public override void Start()
    {
        base.Start();
        mouseMove = GetComponent<Animator>();
        startPosition = transform.position;
        currentMoveDirection = Random.insideUnitCircle.normalized;
    }
    private void OnPositionChanged(Vector2 oldValue, Vector2 newValue)
    {
        if (!isServer)
        {
            rigidbody.position = newValue;
        }
    }
    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // 调用父类的 OnTriggerEnter2D 方法，执行父类的逻辑

        // 在这里添加子类独有的逻辑
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer))) // 检查碰撞体的 Layer 是否匹配目标 Layer
        {
            Destroy(gameObject); // 销毁自己
        }
    }
    IEnumerator MouseMove()
    {
        canMove = false;
        yield return new WaitForSeconds(0.5f);
        mouseMove.SetBool("canMove", true);

        currentMoveDirection = Random.insideUnitCircle.normalized;

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        float currentRotationY = eulerAngles.y;

        // 在服务器上进行状态切换逻辑
        if (isServer)
        {
            // 如果子弹的旋转角度为180度且随机方向与当前朝向在x轴相反，更改旋转角度为0度
            if (Mathf.Approximately(currentRotationY, 180f) && currentMoveDirection.x * transform.right.x < 0f)
            {
                RpcChangeRotation(Quaternion.Euler(0f, 0f, 0f));
            }
            // 如果子弹的旋转角度为0度且随机方向与当前朝向在x轴相反，更改旋转角度为180度
            else if (Mathf.Approximately(currentRotationY, 0f) && currentMoveDirection.x * transform.right.x < 0f)
            {
                RpcChangeRotation(Quaternion.Euler(0f, 180f, 0f));
            }
        }

        rigidbody.velocity = currentMoveDirection * speed * 0.2f;
        yield return new WaitForSeconds(1f);
        mouseMove.SetBool("canMove", false);
        rigidbody.velocity = Vector2.zero;
        canMove = true;
    }

    [ClientRpc]
    void RpcChangeRotation(Quaternion newRotation)
    {
        // 在客户端上设置新的旋转角度
        transform.rotation = newRotation;
    }



    private void Update()
    {
        if (isServer)
        {
            // 更新同步的位置
            syncedPosition = rigidbody.position;
        }
        if (hasReachedMaxDistance)
        {
            // 已达到最大距离后的逻辑
            timeSinceReachedMaxDistance += Time.deltaTime;
            if (timeSinceReachedMaxDistance >= waitingTime)
            {
                // 如果等待时间到了，则尝试检测和追踪目标
                DetectAndTrackTarget();
                if (canMove)
                {
                    StartCoroutine(MouseMove());
                }
            }
        }

        else
        {
            // 未达到最大距离时的逻辑
            float distanceTraveled = Vector3.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                hasReachedMaxDistance = true;
                rigidbody.velocity = Vector2.zero; // 停止移动

                // 确保 shooter 已被分配
                if (shooter != null)
                {
                    // 检测子弹是否在玩家的左边
                    if (transform.position.x < shooter.transform.position.x)
                    {
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f); // 设置Y轴的旋转值为180
                    }
                    else
                    {
                        transform.rotation = Quaternion.identity; // 重置旋转
                    }
                    // 同步修正后的旋转值到所有客户端
                    RpcUpdateBulletRotation(transform.rotation);
                }
                else
                {
                    Debug.LogWarning("Shooter is not assigned");
                }
            }
        }
    }


    private void DetectAndTrackTarget()
    {
        if (target == null || target.GetComponent<NetworkIdentity>().netId != targetNetId)
        {
            DetectTarget();
        }

        if (target != null)
        {
            mouseMove.SetBool("canMove", true);
            Vector2 direction = (Vector2)(target.position - transform.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
            rigidbody.velocity = transform.right * speed;
        }
    }

    private void DetectTarget()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (Collider2D hitCollider in hitColliders)
        {
            NetworkIdentity targetIdentity = hitCollider.GetComponent<NetworkIdentity>();
            if (targetIdentity != null && targetIdentity.netId == shooterNetId)
            {
                continue;
            }

            target = hitCollider.transform;
            targetNetId = targetIdentity.netId;
            break;
        }
    }
}
