using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Grass : NetworkBehaviour
{
    [SyncVar]
    private int health=1;

    void Update()
    {
        if (health <= 0)
            Destroy(gameObject);
    }
    public void TakeDamage(int damage)
    {

        health -= damage;
    }
}
