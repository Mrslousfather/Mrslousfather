using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncActive : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSetActive))]
    public bool isActive;

    // 当 isActive 改变时，Mirror 会调用这个方法
    void OnSetActive(bool oldIsActive, bool newIsActive)
    {
        gameObject.SetActive(newIsActive);
    }
}
