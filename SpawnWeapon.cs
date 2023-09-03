using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnWeapon : NetworkBehaviour
{
    public List<GameObject> weaponList = new List<GameObject>();
    private int randomNum;
    public bool isEmpty = true;
    public float waitingTime = 5f;
    public float initialSpawnDelay; // 初始生成延迟
    private Coroutine countdownCoroutine;
    private bool isFirstSpawn = true;

    public override void OnStartServer()
    {
        if (!isEmpty)
        {
            SpawnWeaponOnServer();
        }
    }

    private void Update()
    {
        if (isEmpty && countdownCoroutine == null)
        {
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    private IEnumerator StartCountdown()
    {
        if (isServer && isFirstSpawn)
        {
            isFirstSpawn = false;
            
            yield return new WaitForSeconds(initialSpawnDelay); // 初始生成延迟
        }
       
        yield return new WaitForSeconds(waitingTime);
        isEmpty = false;
        countdownCoroutine = null;
        if (!isServer) yield break;
     
        SpawnWeaponOnServer();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isEmpty)
        {
            isEmpty = true;
        }
    }

    private void SpawnWeaponOnServer()
    {
        Debug.Log("执行到我了");
        randomNum = Random.Range(0, weaponList.Count);
        GameObject weapon = Instantiate(weaponList[randomNum], transform.position, Quaternion.identity);
        weapon.name = weaponList[randomNum].name;

       

        // Set the parent after spawning
        weapon.transform.SetParent(transform);

        // Sync the parent to all clients
        SyncParent syncParent = weapon.GetComponent<SyncParent>();
       
        if (syncParent != null)
        {
            syncParent.parentNetId = this.netIdentity;
        }
        else
        {
            Debug.LogError("Failed to sync parent: SyncParent component not found.");
        }
        NetworkServer.Spawn(weapon);
    }
}
