using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spike : MonoBehaviour
{
    public Collider2D damageArea;
    public SpriteRenderer spriteRenderer;
    public Sprite inSpikeSprite;
    public Sprite outSpikeSprite; // ÓÃÓÚ´æ´¢OutSpikeµÄÍ¼Æ¬

    void Start()
    {
        StartCoroutine(SpikeActivity());
    }

    IEnumerator SpikeActivity()
    {
        while (true)
        {
            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Enable the damage area (spike thrusts)
            damageArea.gameObject.SetActive(true);

            // Set the Sprite Renderer sprite to OutSpike's sprite
            spriteRenderer.sprite = outSpikeSprite;

            // Wait for 0.5 seconds
            yield return new WaitForSeconds(1.5f);

            // Disable the damage area (spike retracts)
            damageArea.gameObject.SetActive(false);
            spriteRenderer.sprite = inSpikeSprite;
            // Set the Sprite Renderer sprite back to its original sprite (if needed)
            // spriteRenderer.sprite = originalSprite; // Uncomment and assign the original sprite if needed
        }
    }
}
