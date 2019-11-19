using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public float duration;
    Timer timer;

    public void init(bool facingRight)
    {
        GetComponent<SpriteRenderer>().flipX = !facingRight;
    }

    void Start()
    {
        timer = new Timer(duration);
        timer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.isDone())
        {
            Destroy(this.gameObject);
        }
        timer.Update(Time.deltaTime);
    }
}
