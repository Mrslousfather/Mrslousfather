using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    void Update()
    {
        // ������Ե�����ϣ������תֵ
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
