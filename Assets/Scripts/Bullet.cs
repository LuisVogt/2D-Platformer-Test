using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rigidbody;
    private Vector2 direction;
    public float speed;
    public int damage;
    int targetLayer; //put layer of the creature that will be affected;
    public float duration;
    Timer DestroyTimer;

    public void init(Vector2 _direction, int _targetLayer)
    {
        direction = _direction;
        targetLayer = _targetLayer;
    }

    // Start is called before the first frame update
    void Start()
    {
        DestroyTimer = new Timer(duration);
        transform.localScale = new Vector3(transform.localScale.x * direction.x,transform.localScale.y,transform.localScale.z);
        rigidbody = GetComponent<Rigidbody2D>();
        DestroyTimer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(DestroyTimer.isDone())
        {
            Destroy(this.gameObject);
        }
        rigidbody.velocity = direction * speed;
        DestroyTimer.Update(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        if (collision.gameObject.layer == targetLayer)
        {
            if (collided.GetComponent<Creature>())
            { 
                collided.GetComponent<Creature>().TakeDamage(damage);
            }
        }
        Destroy(this.gameObject);
    }
}
