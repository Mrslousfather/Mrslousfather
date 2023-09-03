using UnityEngine;

public class DamageArea : MonoBehaviour
{
    private float accumulatedDamage = 0f;
    public float damagePerSecond = 10f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement pMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (pMovement != null)
            {
                accumulatedDamage += damagePerSecond * Time.deltaTime;
                if (accumulatedDamage >= 1f)
                {
                    int damageToApply = (int)accumulatedDamage;
                    pMovement.TakeFireDamage(damageToApply); // ʹ���·�����������˺�
                    accumulatedDamage -= damageToApply;
                }
            }
            else
            {
                Debug.Log("Player script is null");
            }
        }
    }

}
