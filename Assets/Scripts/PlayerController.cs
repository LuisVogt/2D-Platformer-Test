using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 7;
    public float jumpSpeed = 10;
    public float dashSpeed = 20;

    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;
    private Vector2 targetVelocity;
    private Vector2 targetDashVelocity;
    private RaycastHit2D[] collisions = new RaycastHit2D[16];
    private ContactFilter2D contactFilter = new ContactFilter2D();
    public float raycastDistance = 0.01f;
    private Collider2D collider;
    private Rigidbody2D rigidbody;

    public GameObject bulletPrefab;
    public GameObject slashPrefab;


    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isSlidingOnWall = false;
    private bool isDashing = false;

    public float WallJumpLockdownTime = 1f;
    public float SwordSlashInterval = 0.5f;
    public float DashDuration = 1f;
    public float DashCooldown = 2f;
    public float SlashAnimationDuration = 0.1f;
    public float ShootAnimationDuration = 0.1f;
    public float ShootingCooldownDuration= 2.0f;
    private float gravityScale;

    private Timer WalljumpLockdownTimer;
    private Timer SwordSlashCooldownTimer;
    private Timer DashDurationTimer;
    private Timer SlashAnimationTimer;
    private Timer ShootAnimationTimer;
    private Timer DashCooldownTimer;
    private Timer ShootingCooldown;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        WalljumpLockdownTimer = new Timer(WallJumpLockdownTime);
        SwordSlashCooldownTimer = new Timer(SwordSlashInterval);
        DashDurationTimer = new Timer(DashDuration);
        SlashAnimationTimer = new Timer(SlashAnimationDuration);
        ShootAnimationTimer = new Timer(ShootAnimationDuration);
        DashCooldownTimer = new Timer(DashCooldown);
        ShootingCooldown = new Timer(ShootingCooldownDuration);
        collider = GetComponent<Collider2D>();
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gravityScale = rigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckIfTouchingGround();
        isDashing = false;
        CheckIfSlidingOnWall();

        targetVelocity = Vector2.zero;
        targetVelocity.x = Input.GetAxisRaw("Horizontal") * maxSpeed;

        if(Input.GetButtonDown("Fire1")&&ShootingCooldown.isDone())
        {
            Shoot();
        }

        if(Input.GetButtonDown("Fire2")&&SwordSlashCooldownTimer.isDone())
        {
            SwordStrike();
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            targetVelocity.y += jumpSpeed;
            isGrounded = false;
        }

        if(Input.GetButtonDown("Jump") && isSlidingOnWall)
        {
            WalljumpLockdownTimer.StartTimer();
            isSlidingOnWall = false;
            isFacingRight = !isFacingRight;
            targetVelocity.y += jumpSpeed;
        }

        if(Input.GetButtonDown("Fire3")&&DashCooldownTimer.isDone())
        {
            StartDash();
        }

        if (!WalljumpLockdownTimer.isDone())
        {
            if (isFacingRight)
            {
                targetVelocity.x = maxSpeed;
            }
            else
            {
                targetVelocity.x = -maxSpeed;
            }
        }

        if(!DashDurationTimer.isDone())
        {
            Dash();
        }
        else
        {
            EndDash();
        }

        int collisionsX = rigidbody.Cast(new Vector2(targetVelocity.x,0), contactFilter, collisions, 0.1f);
        int collisionsY = rigidbody.Cast(new Vector2(0,targetVelocity.y), contactFilter, collisions, raycastDistance*0.1f);

        if(collisionsX > 0)
        {
            targetVelocity.x = 0;
            DashDurationTimer.Finish();
        }

        if(targetVelocity.x > 0)
        {
            isFacingRight = true;
        }
        else if(targetVelocity.x < 0)
        {
            isFacingRight = false;
        }

        if(isSlidingOnWall)
        {
            targetVelocity.y = -maxSpeed/3 - rigidbody.velocity.y;
            //DashDurationTimer.Finish();
        }

        setDirection();

        rigidbody.velocity = new Vector2(targetVelocity.x,rigidbody.velocity.y+targetVelocity.y);

        animator.SetFloat("HorizontalSpeed", Mathf.Abs(targetVelocity.x));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSliding", isSlidingOnWall);
        animator.SetBool("isDashing", isDashing);
        animator.SetBool("Slashing", !SlashAnimationTimer.isDone());
        animator.SetBool("Shooting", !ShootAnimationTimer.isDone());

        WalljumpLockdownTimer.Update(Time.deltaTime);
        SwordSlashCooldownTimer.Update(Time.deltaTime);
        DashDurationTimer.Update(Time.deltaTime);
        SlashAnimationTimer.Update(Time.deltaTime);
        ShootAnimationTimer.Update(Time.deltaTime);
        DashCooldownTimer.Update(Time.deltaTime);
        ShootingCooldown.Update(Time.deltaTime);
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
            testRight = !isFacingRight;
        }
        else
        {
            testRight = isFacingRight;
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

    void Shoot()
    {
        ShootingCooldown.StartTimer();
        ShootAnimationTimer.StartTimer();
        Vector2 direction = CheckIfLookingRight() ? Vector2.right : Vector2.left;
        GameObject bullet = Instantiate(bulletPrefab,new Vector3(collider.bounds.center.x + collider.bounds.extents.x * direction.x * 1.3f,collider.bounds.center.y + collider.bounds.extents.y * 0.5f,0),Quaternion.identity);
        bullet.GetComponent<Bullet>().init(direction, LayerMask.NameToLayer("Enemy"));
    }

    void SwordStrike()
    {
        SlashAnimationTimer.StartTimer();
        SwordSlashCooldownTimer.StartTimer();
        int direction = CheckIfLookingRight() ? 1 : -1;
        GameObject slash = Instantiate(slashPrefab, new Vector3(transform.position.x + collider.bounds.extents.x*2*direction,transform.position.y,0),Quaternion.identity,transform);
        //slash.GetComponent<SpriteRenderer>().flipX = !facingRight;
        slashPrefab.GetComponent<DamageHitbox>().init(LayerMask.NameToLayer("Enemy"));
    }

    void StartDash()
    {
        DashDurationTimer.StartTimer();
        DashCooldownTimer.StartTimer();
        targetVelocity.y = -rigidbody.velocity.y;
        rigidbody.gravityScale = 0;
        int direction = CheckIfLookingRight() ? 1 : -1;
        targetDashVelocity.x = dashSpeed * direction;
    }

    void Dash()
    {
        targetVelocity.x = targetDashVelocity.x;
        isDashing = true;
    }

    void EndDash()
    {
        rigidbody.gravityScale = gravityScale;
    }

    void setDirection()
    {
        float direction = CheckIfLookingRight() ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * direction, transform.localScale.y, transform.localScale.z);
    }

    void CheckIfSlidingOnWall()
    {
        isTouchingWall = CheckIfTouchingWall();
        if(!isTouchingWall || isGrounded)
        {
            isSlidingOnWall = false;
            return;
        }
        if (isSlidingOnWall)
        {
            return;
        }
        if(Input.GetAxisRaw("Horizontal") == 0)
        {
            return;
        }
        bool isPressingRight = Input.GetAxisRaw("Horizontal") > 0 ? true : false;
        if(isPressingRight == isFacingRight)
        {
            isSlidingOnWall = true;
        }

    }

    bool CheckIfLookingRight()
    {
        if(isSlidingOnWall)
        {
            return !isFacingRight;
        }
        return isFacingRight;
    }

}
