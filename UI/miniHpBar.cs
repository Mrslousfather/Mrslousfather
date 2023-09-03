using UnityEngine;
using UnityEngine.UI;

public class miniHpBar : MonoBehaviour
{
    public PlayerMovement playerMovement;
    //public static miniHpBar Instance { get; private set; } // ����ʵ��
    public Slider slider; // ����������ʾHP��Slider���

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(gameObject); // ����Ѵ���ʵ�����������´�����ʵ��
    //    }
    //}
    private void Start()
    {
        SetMaxHP(100);
    }
    private void Update()
    {
        SetHP(playerMovement.currentHP);
    }
    // ����HP�����ֵ
    public void SetMaxHP(int maxHP)
    {
        slider.maxValue = maxHP;
        slider.value = maxHP; // ��ʼHPΪ���ֵ
    }

    // ����HP����ʾ

    public void SetHP(int currentHP)
    {
        Debug.Log(currentHP);
        slider.value = currentHP;

    }
}