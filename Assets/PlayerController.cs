using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 7;
    public float jumpSpeed = 10;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private Vector2 targetVelocity;
    private RaycastHit2D[] collisions = new RaycastHit2D[16];
    private ContactFilter2D contactFilter = new ContactFilter2D();
    public float raycastDistance = 0.01f;
    private Collider2D collider;
    private Rigidbody2D rigidbody;

    public float WallJumpLockdownTime = 1f;

    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isSlidingOnWall = false;

    private Timer WalljumpLockdownTimer;

    // Start is called before the first frame update
    void Start()
    {
        WalljumpLockdownTimer = new Timer(WallJumpLockdownTime);
        collider = GetComponent<Collider2D>();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckIfTouchingGround();
        isSlidingOnWall = CheckIfSlidingOnWall();

        targetVelocity = Vector2.zero;
        targetVelocity.x = Input.GetAxisRaw("Horizontal") * maxSpeed;

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            targetVelocity.y += jumpSpeed;
            isGrounded = false;
        }

        if(Input.GetButtonDown("Jump") && isSlidingOnWall)
        {
            WalljumpLockdownTimer.StartTimer();
            isSlidingOnWall = false;
            facingRight = !facingRight;
            targetVelocity.y += jumpSpeed;
        }

        if (!WalljumpLockdownTimer.isDone())
        {
            if (facingRight)
            {
                targetVelocity.x = maxSpeed;
            }
            else
            {
                targetVelocity.x = -maxSpeed;
            }
        }

        int collisionsX = rigidbody.Cast(new Vector2(targetVelocity.x,0), contactFilter, collisions, raycastDistance*0.1f);
        int collisionsY = rigidbody.Cast(new Vector2(0,targetVelocity.y), contactFilter, collisions, raycastDistance*0.1f);
        if(collisionsX > 0)
        {
            targetVelocity.x = 0;
        }
        if (collisionsY > 0)
        {
            targetVelocity.y = 0;
        }

        if(targetVelocity.x > 0)
        {
            facingRight = true;
        }
        else if(targetVelocity.x < 0)
        {
            facingRight = false;
        }

        if(isSlidingOnWall)
        {
            targetVelocity.y = -maxSpeed/3 - rigidbody.velocity.y;
        }
        



        spriteRenderer.flipX = !facingRight;
        rigidbody.velocity = new Vector2(targetVelocity.x,rigidbody.velocity.y+targetVelocity.y);

        WalljumpLockdownTimer.Update(Time.deltaTime);
    }

    bool CheckIfTouchingGround()
    {
        Bounds colliderBounds = collider.bounds;
        Vector2 point = new Vector2(colliderBounds.center.x - colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * raycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        { 
            return true;
        }
        point = new Vector2(colliderBounds.center.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * raycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }
        point = new Vector2(colliderBounds.center.x + colliderBounds.extents.x, colliderBounds.center.y - colliderBounds.extents.y);
        if (Physics2D.Linecast(point, point + Vector2.down * raycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }

        return false;
    }

    bool CheckIfTouchingWall(bool facingForwards = true)
    {
        bool touch1 = false;
        bool touch2 = false;
        Bounds colliderBounds = collider.bounds;

        bool testRight;

        if(facingForwards)
        {
            testRight = !facingRight;
        }
        else
        {
            testRight = facingRight;
        }

        Vector2 facing = testRight ? Vector2.left : Vector2.right;

        Vector2 point = new Vector2(colliderBounds.center.x + colliderBounds.extents.x * facing.x, colliderBounds.center.y - colliderBounds.extents.y * 0.8f);

        if (Physics2D.Linecast(point, point + facing * raycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            touch1 = true;
        }
        point = new Vector2(colliderBounds.center.x + colliderBounds.extents.x * facing.x, colliderBounds.center.y + colliderBounds.extents.y * 0.8f);
        if (Physics2D.Linecast(point, point + facing * raycastDistance, 1 << LayerMask.NameToLayer("Ground")))
        {
            touch2 = true;
        }


        return touch1 && touch2;
    }

    bool CheckIfSlidingOnWall()
    {
        if(CheckIfTouchingWall() && !isGrounded)
        {
            return true;
        }
        return false;
    }
}
