using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstercontrol : MonoBehaviour
{
    public GameObject monsterPrefab; // ��Ĺ���Ԥ����
    public float spawnInterval = 2f; // ���ɼ��

    void Start()
    {
        StartCoroutine(SpawnMonster());
    }

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // ���ｫ����������Spawner�����λ��
            Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        }
    }
}
