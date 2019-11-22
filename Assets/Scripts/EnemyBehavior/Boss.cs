using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public bool movingRight = true;
    public float RaycastDistance = 0.1f;
    Rigidbody2D rb2D;
    private GameObject player;
    private Animator animator;

    Vector2 targetVelocity;
    bool isCharging = false;
    bool isJumping;
    bool isGrounded;

    public float verticalVelocity=20f;
    public float chargeDuration;
    public float chargeCooldown= 5.0f;
    private Timer chargeTimer;
    private ContactFilter2D contactFilter;
    private Timer chargeCooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        player = GameObject.FindGameObjectWithTag("Player");
        rb2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        chargeTimer = new Timer(chargeDuration);
        animator = GetComponent<Animator>();
        chargeCooldownTimer = new Timer(chargeCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        targetVelocity.y = 0;
        if(CheckIfTouchingGround()&&!isCharging&&chargeCooldownTimer.isDone())
        {
            isJumping = false;
            StartCharging();
        }
        if(chargeTimer.isDone()&&!isJumping)
        {
            isCharging = false;
            StartJumping();
        }

        int collisionsX = rb2D.Cast(new Vector2(targetVelocity.x, 0), contactFilter, new RaycastHit2D[16], RaycastDistance);

        if (collisionsX > 0)
        {
            targetVelocity.x = 0;
        }

        animator.SetBool("isCharging", isCharging);

        rb2D.velocity = new Vector2(targetVelocity.x,rb2D.velocity.y + targetVelocity.y);
        chargeTimer.Update(Time.deltaTime);
        chargeCooldownTimer.Update(Time.deltaTime);
    }

    void StartCharging()
    {
        targetVelocity = Vector2.zero;
        chargeTimer.StartTimer();
        isCharging = true;
    }

    void StartJumping()
    {
        float distance = this.transform.position.x - player.transform.position.x;
        float time = 2 * verticalVelocity / (rb2D.gravityScale * Physics2D.gravity.y);
        targetVelocity.x = distance / time;
        targetVelocity.y = verticalVelocity;
        chargeCooldownTimer.StartTimer();
        isJumping = true;

    }

    bool CheckIfTouchingGround()
    {
        Bounds colliderBounds = boxCollider.bounds;
        Vector2 point = new Vector2(colliderBounds.center.x - colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * RaycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }
        point = new Vector2(colliderBounds.center.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * RaycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }
        point = new Vector2(colliderBounds.center.x + colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * RaycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }

        return false;
    }

    void Jump()
    {

    }

    bool CheckForEndOfPath()
    {
        bool result = false;
        Bounds bounds = boxCollider.bounds;
        float direction = movingRight ? -1 : 1;
        Vector2 start = new Vector2(bounds.center.x - bounds.extents.x * direction, bounds.center.y - bounds.extents.y);
        if (!Physics2D.Linecast(start, start + Vector2.down * RaycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            result = true;
        }
        if (Physics2D.Linecast(start, start + Vector2.left * RaycastDistance * direction, 1 << LayerMask.NameToLayer("Ground")))
        {
            result = true;
        }
        return result;
    }
}
