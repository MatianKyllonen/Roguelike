using UnityEngine;

public class DroneBasic : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float followDistance = 2f;
    public float smoothFactor = 0.1f;
    private SpriteRenderer spriteRenderer;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        FollowTarget();
        FlipSprite();
    }

    void FollowTarget()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > followDistance)
        {

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition,ref velocity, smoothFactor / followSpeed);
        }
    }

    void FlipSprite()
    {
        if (target == null) return;

        float relativePositionX = target.position.x - transform.position.x;
        spriteRenderer.flipX = relativePositionX > 0;
    }
}
