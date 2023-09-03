using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShotGunBullet : Bullet
{
    public float initialSpeedDrop = 1f;
    public float speedDropDecreaseRate = 0.2f;
    private float currentSpeedDrop;

    // Parameters for random Y movement
    public float minYVelocity = -1f;
    public float maxYVelocity = 1f;
    public float yVelocityChangeInterval = 0.5f;

    public override void Start()
    {
        base.Start();
        currentSpeedDrop = initialSpeedDrop;
        Destroy(this.gameObject, 8);

        // Start random Y movement after 3 seconds
        Invoke("StartRandomYMovement", 1f);
    }

    private void StartRandomYMovement()
    {
        StartCoroutine(RandomYMovementCoroutine());
    }

    private IEnumerator RandomYMovementCoroutine()
    {
        while (true)
        {
            float randomYVelocity = Random.Range(minYVelocity, maxYVelocity);
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, randomYVelocity);
            yield return new WaitForSeconds(yVelocityChangeInterval);
        }
    }

    public override void Update()
    {
        base.Update();

        if (rigidbody.velocity.magnitude > 0)
        {
            rigidbody.velocity = Vector2.MoveTowards(rigidbody.velocity, Vector2.zero, currentSpeedDrop * Time.deltaTime);
        }

        currentSpeedDrop = Mathf.Max(0, currentSpeedDrop - speedDropDecreaseRate * Time.deltaTime);
    }
}
