using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncActive : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSetActive))]
    public bool isActive;

    // �� isActive �ı�ʱ��Mirror ������������
    void OnSetActive(bool oldIsActive, bool newIsActive)
    {
        gameObject.SetActive(newIsActive);
    }
}
