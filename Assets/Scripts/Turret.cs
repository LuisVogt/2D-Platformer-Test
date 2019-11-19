using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    BoxCollider2D collider;
    public float shootTime;
    Timer shootTimer;
    public GameObject bulletPrefab;
    // Start is called before the first frame update
    void Start()
    {
        shootTimer = new Timer(shootTime);
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(shootTimer.isDone())
        {
            shootTimer.StartTimer();
            GameObject bullet = Instantiate(bulletPrefab, new Vector3(collider.bounds.center.x + collider.bounds.extents.x * 1.3f, collider.bounds.center.y, 0), Quaternion.identity);
            bullet.GetComponent<Bullet>().init(Vector2.right + Vector2.up, LayerMask.NameToLayer("Player"));
        }
        shootTimer.Update(Time.deltaTime);
        
    }
}
