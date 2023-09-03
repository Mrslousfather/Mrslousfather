using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Benchmark;
using UnityEngine.InputSystem;
public class Shotgun : Gun
{
    public int bulletsCount = 5;
    public float spreadAngle = 60f; // 将散射角度增大

    [Command]
    public override void CmdFire(Vector2 mousePos)
    {
        if (!isServer)
            return;

        // Calculate the step between each bullet
        float step = spreadAngle / (bulletsCount - 1);

        // Create the bullets with spread
        for (int i = 0; i < bulletsCount; i++)
        {
            // Calculate rotation for this bullet
            float rotationAngle = step * (i - (bulletsCount - 1) / 2f);
            Quaternion rotation = Quaternion.Euler(0, 0, rotationAngle*5);

            GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
            bullet.transform.position = sightPos.position;
            bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotation * direction);


            // Update bullet sprite.
            HomingMissile homingMissile = bullet.GetComponent<HomingMissile>();
            if (homingMissile != null)
            {
                homingMissile.shotFromGun = this;
                gunBulletIndex = (gunBulletIndex + 1) % homingMissile.trackBulletSprites.Length;
            }

            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetShooter(transform.parent.GetComponent<NetworkIdentity>());
                bulletComponent.direction1 = new Vector2(transform.position.x, transform.position.y);

                if (isUsingGamepad)
                    bulletComponent.mousePos = new Vector2(sightPos.position.x, sightPos.position.y);
                else
                {
                    if (mousePos != null)
                    {
                        bulletComponent.mousePos = mousePos;  // 将鼠标位置传递给子弹
                    }
                }
            }

            NetworkServer.Spawn(bullet);
        }

        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
        shell.transform.position = shellPos.position;
        shell.transform.rotation = shellPos.rotation;
        NetworkServer.Spawn(shell);

        RpcFireAnimation();
    }
}


