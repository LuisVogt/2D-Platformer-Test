using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    BoxCollider2D collider;
    public float shootTime;
    Timer shootTimer;
    public GameObject bulletPrefab;
    private GameObject player;
    public float shootingDistance = 10f;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        shootTimer = new Timer(shootTime);
        collider = GetComponent<BoxCollider2D>();
    }

    public Vector2 directionToPlayer()
    {
        Vector2 difference = player.transform.position - this.transform.position;
        return difference.normalized;
    }

    public bool CheckIfCloseEnough()
    {
        Vector2 difference = player.transform.position - this.transform.position;
        if(difference.magnitude <= shootingDistance)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isCloseEnough = CheckIfCloseEnough();
        animator.SetBool("IsInRange", isCloseEnough);
        if (shootTimer.isDone()&&isCloseEnough)
        {
            shootTimer.StartTimer();
            Vector2 direction = directionToPlayer();
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(collider.bounds.center.x, collider.bounds.center.y, 0), Quaternion.identity);
            bullet.GetComponent<Bullet>().init(direction, LayerMask.NameToLayer("Player"));
        }
        shootTimer.Update(Time.deltaTime);
        
    }
}
