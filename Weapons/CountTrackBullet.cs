using UnityEngine;
using Mirror;

public class CountTrackBullet : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSpriteIndexChanged))]
    public int currentSpriteIndex = 0;

    public Sprite[] bulletSprites;

    private void OnSpriteIndexChanged(int oldIndex, int newIndex)
    {
        // 当 currentSpriteIndex 改变时，这个函数会被调用
        if (bulletSprites != null && bulletSprites.Length > newIndex)
        {
            GetComponent<SpriteRenderer>().sprite = bulletSprites[newIndex];
        }
    }
}
