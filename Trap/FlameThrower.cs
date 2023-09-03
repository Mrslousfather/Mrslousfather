using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    public Collider2D damageArea;
    public SpriteRenderer spriteRenderer;  // Add this line
    private Color originalColor;  // Add this line
    private bool isCountdownActive = false;
    public float delayTime = 1f;
    public float lastTime = 3f;
    public float canNotFireTime = 3f;
    public bool canFire = true;
    public Animator flameAn;
    public SpriteRenderer sprPre;
    private void Start()  // Add this method
    {
        originalColor = spriteRenderer.color;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isCountdownActive && canFire)
            {
                Debug.Log("播放预警动画");
                sprPre.gameObject.SetActive(true);
                StartCoroutine(ActivateDamageAreaAfterDelay());
            }
        }
    }

    private IEnumerator ActivateDamageAreaAfterDelay()
    {
        isCountdownActive = true;
        // Wait for 1 second
        //播放预警动画bool

        
        yield return new WaitForSeconds(delayTime);
        Debug.Log("结束预警动画");
        sprPre.gameObject.SetActive(false);
        // Enable the damage area
        damageArea.gameObject.SetActive(true);

        // Wait for 2 seconds
        yield return new WaitForSeconds(lastTime);
        // Disable the damage area
        damageArea.gameObject.SetActive(false);
        canFire = false;
        isCountdownActive = false;
        spriteRenderer.color = originalColor;  // Change color back to original

        StartCoroutine(FireFire());
    }

    private IEnumerator FireFire()
    {
        yield return new WaitForSeconds(canNotFireTime);
        Debug.Log("无敌时间结束");
        canFire = true;
    }
}
