using UnityEngine;
using System.Collections;
using Mirror;

public class RatBullet : Bullet
{
    [SyncVar(hook = nameof(OnPositionChanged))]
    private Vector2 syncedPosition;
    public float maxDistance = 20f; // �ӵ���������
    public float detectionRadius = 5f; // ���뾶
    public LayerMask playerLayer; // ��Ҳ�
    public float rotationSpeed = 200f; // �ӵ�ת���ٶ�
    public float waitingTime = 3f; // ��ԭ�صȴ���ʱ��

    private Transform target; // ������Ŀ��
    private Vector3 startPosition; // �ӵ��ĳ�ʼλ��
    private bool hasReachedMaxDistance = false; // �Ƿ�ﵽ������
    private float timeSinceReachedMaxDistance = 0f; // �ﵽ�������ľ���ʱ��
    public LayerMask targetLayer; // ָ��Ŀ�� Layer
    [SyncVar]
    private uint targetNetId;
    Animator mouseMove;
    // ����Ķ������һ���������洢ÿ���ƶ��ķ���
    private Vector2 currentMoveDirection;
    bool canMove = true;
    private AnimatorStateInfo stateInfo; // ���ڴ洢״̬��Ϣ
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
        base.OnTriggerEnter2D(other); // ���ø���� OnTriggerEnter2D ������ִ�и�����߼�

        // ���������������е��߼�
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer))) // �����ײ��� Layer �Ƿ�ƥ��Ŀ�� Layer
        {
            Destroy(gameObject); // �����Լ�
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

        // �ڷ������Ͻ���״̬�л��߼�
        if (isServer)
        {
            // ����ӵ�����ת�Ƕ�Ϊ180������������뵱ǰ������x���෴��������ת�Ƕ�Ϊ0��
            if (Mathf.Approximately(currentRotationY, 180f) && currentMoveDirection.x * transform.right.x < 0f)
            {
                RpcChangeRotation(Quaternion.Euler(0f, 0f, 0f));
            }
            // ����ӵ�����ת�Ƕ�Ϊ0������������뵱ǰ������x���෴��������ת�Ƕ�Ϊ180��
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
        // �ڿͻ����������µ���ת�Ƕ�
        transform.rotation = newRotation;
    }



    private void Update()
    {
        if (isServer)
        {
            // ����ͬ����λ��
            syncedPosition = rigidbody.position;
        }
        if (hasReachedMaxDistance)
        {
            // �Ѵﵽ���������߼�
            timeSinceReachedMaxDistance += Time.deltaTime;
            if (timeSinceReachedMaxDistance >= waitingTime)
            {
                // ����ȴ�ʱ�䵽�ˣ����Լ���׷��Ŀ��
                DetectAndTrackTarget();
                if (canMove)
                {
                    StartCoroutine(MouseMove());
                }
            }
        }

        else
        {
            // δ�ﵽ������ʱ���߼�
            float distanceTraveled = Vector3.Distance(startPosition, transform.position);
            if (distanceTraveled >= maxDistance)
            {
                hasReachedMaxDistance = true;
                rigidbody.velocity = Vector2.zero; // ֹͣ�ƶ�

                // ȷ�� shooter �ѱ�����
                if (shooter != null)
                {
                    // ����ӵ��Ƿ�����ҵ����
                    if (transform.position.x < shooter.transform.position.x)
                    {
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f); // ����Y�����תֵΪ180
                    }
                    else
                    {
                        transform.rotation = Quaternion.identity; // ������ת
                    }
                    // ͬ�����������תֵ�����пͻ���
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
