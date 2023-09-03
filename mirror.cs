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

        // 如果耐久度小于等于0，销毁镜子
        if (durability <= 0)
        {
            gameObject.GetComponent<SyncActive>().isActive = false;
        }
    }
   
}

