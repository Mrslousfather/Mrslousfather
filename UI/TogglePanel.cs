using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TogglePanel : NetworkBehaviour
{
    public GameObject panelUI; // ��UI��壬���ڱ����������ʾ
    public GameObject secondaryUI; // �ڶ���UI��壬������Ҷ����Կ���
    public Image childImage; // UI�Ӷ����Image���
    public Sprite[] images; // �洢Ҫ�л���ͼƬ������

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
            // �������Tab��
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                panelUI.SetActive(true); // �ڱ����������ʾ���
            }

            // ����ͷ�Tab��
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                panelUI.SetActive(false); // �ڱ���������������
            }

            // ����ڶ���UIδ������ʱ���ѵ�
            for (int i = 1; i <= 8; i++)
            {
                if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + i)))
                {
                    CmdChangeSecondaryUI(true, i - 1, 3f);
                    break;
                }
            }
        }

        // ���µڶ���UI��״̬
        secondaryUI.SetActive(isSecondaryUIActive);
        childImage.sprite = images[imageIndex];

        // ����ڶ���UI�����ʼ����ʱ
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
