using UnityEngine;
using UnityEngine.UI;

public class Staminabar : MonoBehaviour
{
    public static Staminabar Instance { get; private set; } // 单例实例
    public Slider slider; // 引用用于显示HP的Slider组件

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 如果已存在实例，则销毁新创建的实例
        }
    }

    // 设置HP的最大值
    public void SetMaxSta(int maxSta)
    {
        slider.maxValue = maxSta;
        slider.value = maxSta; // 初始HP为最大值
    }

    // 更新HP的显示

    public void SetSta(int currentSta)
    {
        slider.value = currentSta;
    }
}