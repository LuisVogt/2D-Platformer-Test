using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int maxHP;
    private int currentHP;
    private Animator animator;
    public float damageAnimationDuration = 0.1f;
    private Timer damageAnimationTimer;
    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHP = maxHP;
        damageAnimationTimer = new Timer(damageAnimationDuration);
    }

    public void Update()
    {
        if (animator)
        {
            animator.SetBool("tookDamage", !damageAnimationTimer.isDone());
        }
        damageAnimationTimer.Update(Time.deltaTime);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void TakeDamage(int damage)
    {
        if(animator)
        {
            damageAnimationTimer.StartTimer();
        }
        currentHP-=damage;
        if(currentHP<=0)
        {
            Die();
        }
    }
}
