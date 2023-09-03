using UnityEngine;
using UnityEngine.UI;

public class Staminabar : MonoBehaviour
{
    public static Staminabar Instance { get; private set; } // ����ʵ��
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
    public void SetMaxSta(int maxSta)
    {
        slider.maxValue = maxSta;
        slider.value = maxSta; // ��ʼHPΪ���ֵ
    }

    // ����HP����ʾ

    public void SetSta(int currentSta)
    {
        slider.value = currentSta;
    }
}