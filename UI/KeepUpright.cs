using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    void Update()
    {
        // 这里可以调整你希望的旋转值
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
