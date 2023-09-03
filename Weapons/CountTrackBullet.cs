using UnityEngine;
using Mirror;

public class CountTrackBullet : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSpriteIndexChanged))]
    public int currentSpriteIndex = 0;

    public Sprite[] bulletSprites;

    private void OnSpriteIndexChanged(int oldIndex, int newIndex)
    {
        // �� currentSpriteIndex �ı�ʱ����������ᱻ����
        if (bulletSprites != null && bulletSprites.Length > newIndex)
        {
            GetComponent<SpriteRenderer>().sprite = bulletSprites[newIndex];
        }
    }
}
