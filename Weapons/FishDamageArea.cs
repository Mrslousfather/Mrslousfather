using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishDamageArea : MonoBehaviour
{
    public int damage = 20;
    public float knockbackDistance = 1f;  // 添加击退距离
    public float knockbackDuration = 0.5f; // 添加击退持续时间

    private List<Transform> m_HasAttacked = new List<Transform>();

    private void OnEnable()
    {
        m_HasAttacked.Clear(); //清空已被攻击列表
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            if (player != null && !m_HasAttacked.Exists(x => x == player.transform))
            {
                // Damage the player
                player.TakeDamage(damage);
                m_HasAttacked.Add(player.transform);

                // Knockback the player (similar to the SpikeDamageArea logic)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                StartCoroutine(KnockbackPlayer(collision.transform, knockbackDirection, knockbackDistance, knockbackDuration));
            }
        }
        BreakableBox breakableBox = collision.GetComponent<BreakableBox>();
        if (breakableBox != null)
        {
            if (collision.CompareTag("Box"))
            {
                breakableBox.TakeDamage(damage);
                Debug.Log("对箱子造成伤害了");
            }
        }
        Grass grass = collision.GetComponent<Grass>();
        if (grass != null)
        {
            if (collision.CompareTag("Grass"))
            {
                grass.TakeDamage(damage);

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
            RaycastHit2D hit = Physics2D.CircleCast(playerTransform.position, radius, direction, frameDistance, LayerMask.GetMask("Wall", "Water"));

            if (hit.collider != null)
            {
                Debug.Log("Hit the wall or water!");
                break;
            }

            playerTransform.position += (Vector3)(direction * frameDistance);
            movedDistance += frameDistance;

            yield return null;
        }
    }
}
