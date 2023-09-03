using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Benchmark;
using UnityEngine.InputSystem;

public class mirror : NetworkBehaviour
{
    [SyncVar]
    public int durability = 500;

    public void start()
    {
        durability = 500;
    }
    public void DecreaseDurability(int amount)
    {
        durability -= amount;

        // ����;ö�С�ڵ���0�����پ���
        if (durability <= 0)
        {
            gameObject.GetComponent<SyncActive>().isActive = false;
        }
    }
   
}

