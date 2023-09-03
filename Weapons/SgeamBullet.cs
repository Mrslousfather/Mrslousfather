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

        // 获取子弹的初始飞行方向
        Vector2 initialDirection = direction.normalized;

        // 检查子弹的水平速度方向
        bool isBulletFlyingLeft = initialDirection.x < 0;

        // 如果子弹的飞行方向朝向屏幕左边，旋转子弹的图片
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

    // 检测目标的函数
    
}