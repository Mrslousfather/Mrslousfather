using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
public class PlayerMovement : NetworkBehaviour
{
    [SyncVar]
    public int characterIndex;
    public Animator animator;
    public NetworkIdentity lastDamagedBy;
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;
    [SyncVar(hook = nameof(OnScoreChanged))]
    public int score = 0;
    public float speed;
    private Vector2 input;
    private Vector2 mousePos;
    private SpriteRenderer spriteRenderer;
    new private Rigidbody2D rigidbody;
    private int gunNum;
    [SyncVar]
    public int currentHP;
    public static int currentStamina;
    public int maxHP = 100;
    public int maxStamina = 100;
    public int staminaRecoveryPerSecond = 10;
    private bool isDashing = false;
    public int dashcost;
    private NetworkAnimator networkAnimator;
    public InputAction dashAction;
    private Text txtScore;
    private SpawnWeapon spawnWeaponScript;
    public static List<PlayerMovement> allPlayers = new List<PlayerMovement>();
    [HideInInspector]
    public GameUIController gameUIController;
    [HideInInspector]
    public Text endTitle;
    public bool canMove = true;
    public TMP_Text txtShowName;
    private KillFeed killFeed;
    private NetworkRoomManagerExt networkManager;
    [SyncVar]
    public int killStreak = 0;
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color playerColor = Color.white;
    private Image FruitImageEffect;  // 图片的引用，需要在Unity编辑器中设置
    private float fruitBulletEffectAlpha = 0;  // 透明度
    private int fruitBulletHitCount = 0;  // 子弹击中计数器
    private bool isBlinking = false;
    private int framesPerDecay = 3;  // 每隔几帧进行一次衰减
    private int frameCounter = 0;
    [ClientRpc]
    private void RpcSetColor(Color color)
    {
        playerColor = color;
        spriteRenderer.color = color;
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        spriteRenderer.color = newColor;
    }
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (isLocalPlayer)
        {
            
            if (NetworkRoomManagerExt.playerCharacterIndexes.TryGetValue(NetworkClient.connection, out int index))
            {
                // Set the character index
                characterIndex = index;
                // ... other code
            }
            else
            {
                Debug.LogWarning("No character index found for the current connection");
            }
        
        networkManager = NetworkManager.singleton as NetworkRoomManagerExt;
            if (networkManager)
            {
                CmdSetAnimatorController(characterIndex);
            }
        }
    }
    [ClientRpc]
    public void RpcResetKillStreak()
    {
        killStreak = 0;
    }

    [Command]
    public void CmdSetAnimatorController(int index)
    {
        RpcSetAnimatorController(index);
    }

    // 在所有客户端上执行的方法，用于设置 Animator Controller
    [ClientRpc]
    public void RpcSetAnimatorController(int index)
    {
        NetworkRoomManagerExt rmanager = FindObjectOfType<NetworkRoomManagerExt>();
        RuntimeAnimatorController[] controllers = rmanager.characterControllers;

        if (index >= 0 && index < controllers.Length)
        {
            animator.runtimeAnimatorController = controllers[index];
        }
    }
    
    [Server]
    public void AddScore(int scoreToAdd)
    {
        PlayerMovement killerPlayer = GetKillerPlayer();
        if (killerPlayer != this)
        {
            Debug.Log($"<color=red><b>{killerPlayer}</b></color>");
            score += scoreToAdd;
        }

    }

    [Command]
    public void CmdSetPlayerName(string newName)
    {
        playerName = newName;
        RpcShowName();  // 更新头顶的名字文本
    }
    [ClientRpc]
    public void RpcShowName()
    {
        txtShowName.text = playerName;
    }
    void OnNameChanged(string oldName, string newName)
    {
        // 开始显示名字的 Coroutine
        StartCoroutine(ShowNameWithDelay(newName));
    }
    IEnumerator ShowNameWithDelay(string name)
    {
        // 等待0.1秒
        yield return new WaitForSeconds(0.2f);

        // 显示名字
        txtShowName.text = name;
    }
    private void OnScoreChanged(int oldScore, int newScore)
    {
        // If this is the local player, update the score display
        UpdateScore(newScore);
    }
    public void UpdateScore(int newScore)
    {
        if (isLocalPlayer)
        {
            txtScore.text = newScore.ToString();
            Debug.Log("已更新分数");
        }
    }
    void Awake()
    {
        // 将冲刺按钮绑定到手柄的R1键
        dashAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/rightShoulder");
        dashAction.Enable();
    }
    void Start()
    {
        FruitImageEffect = GameObject.Find("Main Camera/Canvas/FruitImage").GetComponent<Image>();
        FruitImageEffect.color = new Color(1, 1, 1, 0);  // 设置图片为完全透明
        spriteRenderer = GetComponent<SpriteRenderer>();
        allPlayers.Add(this);
        killFeed = FindObjectOfType<KillFeed>();
        if(killFeed!=null)
        {
            Debug.Log("killfeed不为空");
        }
        if (isLocalPlayer)
        {
            string playerName = PlayerPrefs.GetString("PlayerName", "Default Name");
            CmdSetPlayerName(playerName);
        }
        gameUIController = GameObject.Find("GameUICanvas").GetComponent<GameUIController>();
        GameObject scoreObject = GameObject.Find("NowScore");
        if (scoreObject != null)
        {
            txtScore = scoreObject.GetComponent<Text>();
            // 你现在可以使用 txtScore 对象
        }
        else
        {
            Debug.LogError("未找到名为 'NowScore' 的对象");
        }

        networkAnimator = GetComponent<NetworkAnimator>();
        HpBar.Instance.SetMaxHP(maxHP);
        Staminabar.Instance.SetMaxSta(maxStamina);
        currentHP = maxHP;
        currentStamina = maxStamina;
        if (isLocalPlayer)
            StartCoroutine(RecoverStamina());
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(DecreaseFruitBulletEffect());
    }
    public void HideNonCorrespondingEndTitle()
    {
        // 如果这不是本地玩家，就隐藏 endTitle
        if (!isLocalPlayer)
        {
            endTitle.gameObject.SetActive(false);
        }
    }
    void OnDestroy()
    {
        allPlayers.Remove(this);
    }

    void FixedUpdate()
    {
        if (gameUIController.timeRemaining <= 0)
        {
            StartCoroutine(DelayFind());
        }
        if (!canMove) // 如果不能移动，直接返回，不处理后续的玩家输入
        {
            return;
        }
        if (isLocalPlayer && !isDead)
        {
            Staminabar.Instance.SetSta(currentStamina);
            HpBar.Instance.SetHP(currentHP);

            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            rigidbody.velocity = input.normalized * speed;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));               
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));        
            }
            if (input != Vector2.zero)
                networkAnimator.animator.SetBool("isMoving", true);
            else
                networkAnimator.animator.SetBool("isMoving", false);

            if ((Input.GetButton("Dash") || dashAction.triggered) && !isDashing)
            {
                if (currentStamina < dashcost && input != Vector2.zero)
                    return;

                StartCoroutine(Dash()); // 开始冲刺协程
            }
            if (currentHP <= 40 && currentHP > 20 && isBlinking==false)
            {
                StartCoroutine(BlinkRed1());
            }
            else if (currentHP <= 20 && isBlinking == false)
            {
                StartCoroutine(BlinkRed2());
            }

        }
        else if (isDead)
        {
            rigidbody.velocity = Vector2.zero; // 如果玩家已经死亡，将速度设置为0\

            //隐藏所有枪
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child.tag == "playerweapon")
                {
                    child.GetComponent<SyncActive>().isActive = false;
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
    private IEnumerator BlinkRed1()
    {
        isBlinking = true;
        HurtChangeColorRed(); // 切换颜色
        yield return new WaitForSeconds(1f);
        isBlinking = false;
    }
    private IEnumerator BlinkRed2()
    {
        isBlinking = true;
        HurtChangeColorRed(); // 切换颜色
        yield return new WaitForSeconds(0.5f);
        isBlinking = false;
    }

    public IEnumerator DelayFind()
    {
        yield return new WaitForSeconds(0.5f);
        FindEndPanel();
    }
    void FindEndPanel()
    {
        int playerIndex = allPlayers.IndexOf(this) + 1;  // 从 1 开始计数，而不是从 0 开始
        GameObject endTitleObject = GameObject.Find("WinOrLoseTitle" + playerIndex);
        if (endTitleObject != null)
        {
            endTitle = endTitleObject.GetComponent<Text>();
            // 你现在可以使用 endTitle 对象
        }
    }
    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        // 确保血量不超过最大值
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
        // 更新血量显示
        RpcUpdateHP(currentHP);
    }
    [ClientRpc]
    private void RpcAddScoreA(int scoreToAdd)
    {
        // 获取玩家积分管理组件，并增加积分
        PlayerScore playerScore = GetComponent<PlayerScore>();
        if (playerScore != null)
        {
            playerScore.AddScore(scoreToAdd);
        }
    }
    [SyncVar]
    public bool isDead = false;
    private IEnumerator RestoreColor()
    {
        yield return new WaitForSeconds(0.1f); // 等待0.1秒
        RpcChangeToWhite();
    }
    [ClientRpc]
    private void RpcChangeToWhite()
    {
        // 还原为原始颜色
        spriteRenderer.color = Color.white;
    }
    public void HurtChangeColor()
    {
        // 设置新颜色
        playerColor = Color.red;
        RpcSetColor(playerColor);
        StartCoroutine(RestoreColor());
    }
    public void HurtChangeColorRed()
    {
        // 设置新颜色
        playerColor = Color.red;
        RpcSetColor(playerColor);
        StartCoroutine(RestoreColor());
    }
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return; // 如果玩家已经死亡，那么他们不能再次受到伤害
        }

        Debug.Log($"<color=red><b>{damage}</b></color>");
        currentHP -= damage;

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;           
            PlayerMovement killerPlayer = GetKillerPlayer();
            if (killFeed != null && killerPlayer!=null)
            {
                if (killerPlayer != this)
                {
                   
                    killerPlayer.killStreak += 1;
                  
                    killFeed.RpcNotifyKill(killerPlayer.playerName, playerName, killerPlayer.killStreak);
               
                }
            }
            killStreak = 0;  // Reset the kill streak when the player dies
            StartCoroutine(SetDeath());
            StartCoroutine(Respawn()); // 开始复活的Coroutine
            
        }

        RpcUpdateHP(currentHP);
        // 设置新颜色
        HurtChangeColor();
    }
    
    public void TakeSpikeDamage(int damage)
    {
        if (isDead) return; // 如果玩家已经死亡，不再受到伤害

        currentHP -= damage;

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;
            killStreak = 0; // 重置击杀连击数

            if (killFeed != null)
            {
                killFeed.RpcNotifySpikeKill(playerName); // 播报地刺击杀
            }

            StartCoroutine(SetDeath());
            StartCoroutine(Respawn()); // 开始复活的Coroutine
        }

        RpcUpdateHP(currentHP);
        // 设置新颜色
        HurtChangeColor();
    }
    public void TakeFireDamage(int damage)
    {
        if (isDead) return; // 如果玩家已经死亡，不再受到伤害

        currentHP -= damage;

        if (currentHP <= 0)
        {
            isDead = true;
            currentHP = 0;
            killStreak = 0; // 重置击杀连击数

            if (killFeed != null)
            {
                killFeed.RpcNotifyFireKill(playerName); // 播报火焰喷射器击杀
            }

            StartCoroutine(SetDeath());
            StartCoroutine(Respawn()); // 开始复活的Coroutine
        }

        RpcUpdateHP(currentHP);
        // 设置新颜色
        HurtChangeColor();
    }

    private PlayerMovement GetKillerPlayer()
    {
        if(lastDamagedBy!=null)
        {
            return lastDamagedBy.GetComponent<PlayerMovement>();
        }
        else
        {
            return null;
        }
    }
    public IEnumerator SetDeath()
    {
        if (!isServer)
        {
            yield break;
        }
        yield return new WaitForSeconds(0.3f);
        RpcSetetAnimation();
    }
    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2f); // 等待2秒

        if (isServer)
        {
            // 获取所有 NetworkStartPosition 对象
            NetworkStartPosition[] startPositions = FindObjectsOfType<NetworkStartPosition>();

            // 如果没有定义 NetworkStartPosition 对象，使用当前位置
            Vector3 spawnPosition = startPositions.Length > 0
                ? startPositions[Random.Range(0, startPositions.Length)].transform.position
                : transform.position;

            // 设置新的玩家位置
            transform.position = spawnPosition;
            yield return new WaitForSeconds(0.5f); // 添加延迟以解决闪烁问题

            // 玩家复活，血量恢复
            currentHP = maxHP;
            isDead = false;
            RpcUpdateHP(currentHP);

            RpcResetAnimation();
            RpcRespawn(spawnPosition); // Call the RpcRespawn method on all clients
            RpcResetKillStreak(); // 重置击杀连击数
            RpcActivatePistol();
        }
    }

    [ClientRpc]
    public void RpcSetetAnimation()
    {
        networkAnimator.animator.SetBool("isDead", true);
    }

    [ClientRpc]
    public void RpcResetAnimation()
    {
        networkAnimator.animator.SetBool("isDead", false);
        networkAnimator.animator.Rebind();
    }

   
    public void RpcActivatePistol()
    {
        // 显示手枪
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.name == "Pistol")
            {
                child.GetComponent<SyncActive>().isActive = true;
                child.SetActive(true);
            }
        }
    }
    [ClientRpc]
    public void RpcRespawn(Vector3 newPosition)
    {
        transform.position = newPosition; // Update the player's position on the client
    }



    [Command]
    public void RpcUpdateHP(int newHP)
    {
        currentHP = newHP;
        HpBar.Instance.SetHP(currentHP);
    }

    IEnumerator RecoverStamina()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); //暂停0.1秒

            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRecoveryPerSecond;

                // 防止超过最大耐力值
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }
    }
    IEnumerator Dash()
    {
        currentStamina -= dashcost;
        isDashing = true; // 设置正在冲刺的状态

        float dashSpeed = 7f; // 冲刺速度
        float dashDuration = 0.5f; // 冲刺持续时间

        float originalSpeed = speed; // 记录原始速度
        speed = dashSpeed; // 设置冲刺速度

        yield return new WaitForSeconds(dashDuration); // 冲刺持续时间

        speed = originalSpeed; // 恢复原始速度

        isDashing = false; // 重置冲刺状态
    }
    private IEnumerator DecreaseFruitBulletEffect()
    {
        while (true)
        {
            frameCounter++;
            if (frameCounter >= framesPerDecay)
            {
                frameCounter = 0;

                if (fruitBulletEffectAlpha > 0)
                {
                    fruitBulletEffectAlpha -= 0.01f;  // 衰减值
                    if (fruitBulletEffectAlpha < 0) fruitBulletEffectAlpha = 0;
                    UpdateFruitImageEffect();
                }
            }

            yield return null;
        }
    }
    
    [Command]
    private void CmdPlayerHitByFruitBullet()
    {
        // 服务器通知所有客户端，但只有被击中的本地玩家会更新图片效果
        RpcUpdateFruitImageEffectForLocalPlayer();
    }

    [ClientRpc]
    private void RpcUpdateFruitImageEffectForLocalPlayer()
    {
        if (hasAuthority)  // 确保只有被击中的客户端执行以下逻辑
        {
            // 增加计数器和透明度的逻辑
            fruitBulletHitCount++;

            fruitBulletEffectAlpha += 0.2f * fruitBulletHitCount;
            if (fruitBulletEffectAlpha > 1) fruitBulletEffectAlpha = 1;

            UpdateFruitImageEffect();
        }
    }
    void UpdateFruitImageEffect()
    {
        Color newColor = FruitImageEffect.color;
        newColor.a = fruitBulletEffectAlpha;
        FruitImageEffect.color = newColor;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // 如果是本地玩家并且被"果实子弹"击中，则通知服务器
        if (hasAuthority && other.gameObject.CompareTag("FruitBullet"))
        {
            Debug.Log("被果实击中了");
            CmdPlayerHitByFruitBullet();
        }
        if (!isServer) return;
        // 检测碰到的是否是武器
        if (other.gameObject.CompareTag("Weapon") && other.gameObject.name!= "Bandage")
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("playerweapon"))
                {

                    child.GetComponent<SyncActive>().isActive = false;

                }

            }
            Debug.Log("碰到武器");
        }
        if (other.gameObject.name == "Gun")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Gun")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "Pistol")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Pistol")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "Mirror")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Mirror")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "catgun")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "catgun")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "textgun")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "textgun")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "shotgun")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "ShotGun")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "Sgeam")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "Sgeam")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
        if (other.gameObject.name == "FruitGun")
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.name == "FruitGun")
                {
                    child.GetComponent<SyncActive>().isActive = true;
                    child.gameObject.SetActive(true);
                    NetworkServer.Destroy(other.gameObject);
                    break;

                }

            }
        }
    }
}