using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public static HpBar Instance { get; private set; } // ����ʵ��
    public Slider slider; // ����������ʾHP��Slider���

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ����Ѵ���ʵ�����������´�����ʵ��
        }
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
        slider.value = currentHP;
    }
}