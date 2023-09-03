using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Examples.Benchmark;
using UnityEngine.InputSystem;

public class Gun : NetworkBehaviour
{
    
    public float interval;
    public GameObject bulletPrefab;
    public GameObject shellPrefab;
    public Transform muzzlePos;
    protected Transform shellPos;
    protected Transform playerpos;
    protected Vector2 mousePos;
    protected Transform sightPos;
    protected Vector2 direction;
    public InputAction fireAction;
    protected float timer;
    protected float flipY;
    protected Animator animator;
    public bool canFire = true;
    public int gunBulletIndex;
    float fireValue;
    [SyncVar]
    public bool isUsingGamepad = false;
    public Vector2 Direction

    {
        get { return direction; }
    }
    public InputAction rotateGunAction;
    Vector2 stickInput;
    void Awake()
    {
        fireAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/leftShoulder");
        fireAction.Enable();
        // 初始化 rotateGunAction，将它绑定到手柄的右摇杆
        rotateGunAction = new InputAction(type: InputActionType.Value, binding: "<Gamepad>/rightStick");
        rotateGunAction.Enable();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        gunBulletIndex = -1;
        muzzlePos = transform.Find("Muzzle");
        shellPos = transform.Find("BulletShell");
        sightPos = transform.Find("sight");
        flipY = transform.localScale.y;
    }



    public void Update()
    {
        if (!canFire) // 如果不能开火，直接返回，不处理后续的开火逻辑
        {
            return;
        }
        if (isLocalPlayer)
        {
            // 检查Fire操作的状态
            fireValue = fireAction.ReadValue<float>();

            if (mousePos.x < transform.position.x)
                transform.localScale = new Vector3(flipY, -flipY, 1);
            else
                transform.localScale = new Vector3(flipY, flipY, 1);

            Shoot();

        }

    }

    public void Shoot()
    {


        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad)
            {
                isUsingGamepad = true;
                break;
            }
        }
        //Debug.Log($"是否使用外设" + isUsingGamepad);
        if (isUsingGamepad)
        {
            // 获取摇杆的移动值
            stickInput = rotateGunAction.ReadValue<Vector2>();

            if (stickInput != Vector2.zero) // 只有当 stickInput 不是 (0,0) 时才改变枪口方向
            {
                // 计算枪口的旋转角度
                float angle = Mathf.Atan2(stickInput.y, stickInput.x) * Mathf.Rad2Deg;

                // 旋转枪口
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                direction = stickInput;
            }
        }
        else
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
            transform.right = direction;
        }


        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                timer = 0;
        }

        if (Input.GetButton("Fire1") || (fireValue > 0))
        {
            if (timer == 0)
            {
                timer = interval;
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                CmdFire(mousePos);
            }
        }
    }


    [Command]
    public virtual void CmdFire(Vector2 mousePos)
    {
        if (!isServer)
            return;

        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
        bullet.transform.position = sightPos.position;
        bullet.transform.rotation = sightPos.rotation;

        // Update bullet sprite.
        HomingMissile homingMissile = bullet.GetComponent<HomingMissile>();
        if (homingMissile != null)
        {
            homingMissile.shotFromGun = this;
            gunBulletIndex = (gunBulletIndex + 1) % homingMissile.trackBulletSprites.Length;
        }
        SgeamBullet sgeamBullet = bullet.GetComponent<SgeamBullet>();
        if(sgeamBullet!=null)
        {
            sgeamBullet.shotFromGun = this;
            gunBulletIndex= Random.Range(0, sgeamBullet.trackBulletSprites.Length);
            switch (gunBulletIndex)
            {
                case 0:
                    sgeamBullet.damage = 25;
                    break;
                case 1:
                    sgeamBullet.damage = 50;
                    break;
                case 2:
                    sgeamBullet.damage = 66;
                    break;
                case 3:
                    sgeamBullet.damage = 75;
                    break;
                case 4:
                    sgeamBullet.damage = Random.Range(30, 101); // 生成 30 到 100 之间的随机整数
                    break;
            }
        }

        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.SetShooter(transform.parent.GetComponent<NetworkIdentity>());
            bulletComponent.direction1 = new Vector2(transform.position.x, transform.position.y);
            bulletComponent.mousePos = mousePos;  // 将鼠标位置传递给子弹
                
            
        }

        NetworkServer.Spawn(bullet);//在网络上实例化子弹预制体

        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
        shell.transform.position = shellPos.position;
        shell.transform.rotation = shellPos.rotation;
        NetworkServer.Spawn(shell);

        RpcFireAnimation();
    }
   
    [ClientRpc]
    public void RpcFireAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }
        else
        {
            //Debug.LogError("Animator is null");
        }
    }
}