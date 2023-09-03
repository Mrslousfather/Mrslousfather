using UnityEngine;
using Mirror;

public class SgeamBullet : Bullet
{
    public Sprite[] trackBulletSprites;

    [SyncVar(hook = nameof(OnChangeBulletSprite))]
    public int currentBulletSprite;
    public override void Start()
    {
        base.Start();
        if (shotFromGun != null)
        {
            currentBulletSprite = shotFromGun.gunBulletIndex;  // Update bullet sprite.
        }

        // ��ȡ�ӵ��ĳ�ʼ���з���
        Vector2 initialDirection = direction.normalized;

        // ����ӵ���ˮƽ�ٶȷ���
        bool isBulletFlyingLeft = initialDirection.x < 0;

        // ����ӵ��ķ��з�������Ļ��ߣ���ת�ӵ���ͼƬ
        if (isBulletFlyingLeft)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = 180f;
            rotation.y = 180f;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void OnChangeBulletSprite(int oldSprite, int newSprite)
    {
        GetComponent<SpriteRenderer>().sprite = trackBulletSprites[newSprite];
    }

    // ���Ŀ��ĺ���
    
}