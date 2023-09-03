using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TogglePanel : NetworkBehaviour
{
    public GameObject panelUI; // 主UI面板，仅在本地玩家上显示
    public GameObject secondaryUI; // 第二个UI面板，所有玩家都可以看到
    public Image childImage; // UI子对象的Image组件
    public Sprite[] images; // 存储要切换的图片的数组

    [SyncVar]
    private float timer;

    [SyncVar]
    private bool isSecondaryUIActive;

    [SyncVar]
    private int imageIndex;

    void Update()
    {
        if (isLocalPlayer)
        {
            // 如果按下Tab键
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                panelUI.SetActive(true); // 在本地玩家上显示面板
            }

            // 如果释放Tab键
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                panelUI.SetActive(false); // 在本地玩家上隐藏面板
            }

            // 如果第二个UI未激活或计时器已到
            for (int i = 1; i <= 8; i++)
            {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i)))
                {
                    CmdChangeSecondaryUI(true, i - 1, 3f);
                    break;
                }
            }
        }

        // 更新第二个UI的状态
        secondaryUI.SetActive(isSecondaryUIActive);
        childImage.sprite = images[imageIndex];

        // 如果第二个UI活动，则开始倒计时
        if (isSecondaryUIActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                CmdChangeSecondaryUI(false, 0, 0f);
            }
        }
    }

    [Command]
    private void CmdChangeSecondaryUI(bool active, int index, float time)
    {
        isSecondaryUIActive = active;
        imageIndex = index;
        timer = time;
    }
}
