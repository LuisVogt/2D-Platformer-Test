using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    private SpriteRenderer spriteRenderer;

    private bool facingRight = true;

    public float WalljumpLockTime = 1f;
    private Timer WalljumpLockTimer;

    private Vector2 previousMove = Vector2.zero;
    //private Animator animator;

    // Use this for initialization
    void Awake()
    {
        WalljumpLockTimer = new Timer(WalljumpLockTime);
        WalljumpLockTimer.StartTimer();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    private void Update()
    {
        WalljumpLockTimer.Update(Time.deltaTime);
        base.Update();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        if (WalljumpLockTimer.isDone())
        {
            move.x = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            move.x = previousMove.x;
        }
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * 0.5f;
            }
        }

        if(Input.GetButtonDown("Jump")&& isSliding)
        {
            velocity.y = jumpTakeOffSpeed;
            WalljumpLockTimer.StartTimer();
            move.x = -Input.GetAxisRaw("Horizontal");
        }

        if (move.x > 0)
        {
            facingRight = false;
        }
        else if (move.x < 0)
        {
            facingRight = true;
        }
        spriteRenderer.flipX = facingRight;

        targetVelocity = move * maxSpeed;
        previousMove = move;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position,transform.position+new Vector3(targetVelocity.x,targetVelocity.y,0));
    }
}