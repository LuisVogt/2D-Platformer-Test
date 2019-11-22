using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpillar : MonoBehaviour
{
    BoxCollider2D boxCollider;
    public float moveTime = 5.0f;
    public float moveSpeed = 1.0f;
    public bool movingRight = true;
    Timer moveTimer;
    public float RaycastDistance = 0.1f;
    Rigidbody2D rb2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        moveTimer = new Timer(moveTime);
        boxCollider = GetComponent<BoxCollider2D>();
        UpdateDirection();
    }

    public void setSpeed(float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
    }

    bool CheckForEndOfPath()
    {
        bool result = false;
        Bounds bounds = boxCollider.bounds;
        float direction = movingRight ? -1 : 1;
        Vector2 start = new Vector2(bounds.center.x - bounds.extents.x*direction, bounds.center.y - bounds.extents.y);
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

    void UpdateDirection()
    {
        moveTimer.StartTimer();
        if(movingRight)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(moveTimer.isDone() || CheckForEndOfPath())
        {
            movingRight = !movingRight;
            UpdateDirection();
        }
        float direction = movingRight ? 1 : -1;

        rb2D.velocity = new Vector2(moveSpeed * direction, rb2D.velocity.y);
        moveTimer.Update(Time.deltaTime);
    }

}
