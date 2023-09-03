using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class SpikeDamageArea : MonoBehaviour
{
    public int damage = 20;
    public float knockbackDistance = 1f;
    public float knockbackDuration = 0.5f;

    private List<Transform> m_HasAttacked = new List<Transform>();


    private void OnEnable()
    {
        m_HasAttacked.Clear();//清空已被攻击列表
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null && !m_HasAttacked.Exists(x => x == player.transform))
            {
                // Damage the player with spike damage
                player.TakeSpikeDamage(damage);
                m_HasAttacked.Add(player.transform);
                Debug.Log("Player has been damaged");

                // Knockback the player
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                StartCoroutine(KnockbackPlayer(collision.transform, knockbackDirection, knockbackDistance, knockbackDuration));
            }
            else
            {
                Debug.Log("Player script is null");
            }
        }
    }


    IEnumerator KnockbackPlayer(Transform playerTransform, Vector2 direction, float distance, float duration)
    {
        float speed = distance / duration;
        float movedDistance = 0;
        float radius = playerTransform.GetComponent<CircleCollider2D>().radius;

        while (movedDistance < distance)
        {
            // Check if playerTransform has been destroyed
            if (playerTransform == null)
            {
                yield break;
            }

            float frameDistance = speed * Time.deltaTime;
            RaycastHit2D hit = Physics2D.CircleCast(playerTransform.position, radius, direction, frameDistance, LayerMask.GetMask("Wall","Water"));

            if (hit.collider != null)
            {
                Debug.Log("Hit the wall!");
                break;
            }

            playerTransform.position += (Vector3)(direction * frameDistance);
            movedDistance += frameDistance;

            yield return null;
        }
    }

}
