using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Fish : NetworkBehaviour
{
    public Transform parentTransform; // �������Transform
    public float distanceFromParent = 2f; // �������븸����ľ���
    protected Animator animator;
    protected float timer;
    public List<GameObject> DamageAreas;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Rush();
    }
    public void Rush()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 parentPosition = parentTransform.position;

        Vector3 directionToMouse = mousePosition - parentPosition;
        directionToMouse.z = 0; // �� Z ������㣬ȷ����ƽ���ϼ�������

        Vector3 targetPosition = parentPosition + directionToMouse.normalized * distanceFromParent;
        transform.position = targetPosition;

        // ������ת�Ƕ�
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        // ����λ�õ��� Y ����
        if (directionToMouse.x < 0)
        {
            transform.localScale = new Vector3(1.2f, -1.2f, 1);
        }
        else
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CmdAttack();
            for(int i=0;i<DamageAreas.Count;i++)
            {
                DamageAreas[i].gameObject.SetActive(true);
            }          
            StartCoroutine(HideDamageArea());
        }
    
    }
    IEnumerator HideDamageArea()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < DamageAreas.Count; i++)
        {
            DamageAreas[i].gameObject.SetActive(false);
        }
    }
    [Command]
    public void CmdAttack()
    {
        if (!isServer)
            return;
        RpcFireAnimation();
    }

    [ClientRpc]
    public void RpcFireAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            //Debug.LogError("Animator is null");
        }
    }
}

