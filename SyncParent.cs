using UnityEngine;
using Mirror;

public class SyncParent : NetworkBehaviour
{
    [SyncVar(hook = nameof(SetParent))]
    public NetworkIdentity parentNetId;

    private void SetParent(NetworkIdentity oldParent, NetworkIdentity newParent)
    {
        if (newParent != null)
        {
            transform.SetParent(newParent.transform);
            name = name.Replace("(Clone)", "").Trim();
        }
    }

    public override void OnStartClient()
    {
        SetParent(null, parentNetId);
    }
}
