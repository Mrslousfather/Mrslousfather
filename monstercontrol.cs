using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monstercontrol : MonoBehaviour
{
    public GameObject monsterPrefab; // 你的怪物预制体
    public float spawnInterval = 2f; // 生成间隔

    void Start()
    {
        StartCoroutine(SpawnMonster());
    }

    IEnumerator SpawnMonster()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 这里将怪物生成在Spawner对象的位置
            Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        }
    }
}
