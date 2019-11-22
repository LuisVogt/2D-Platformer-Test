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
    public GameObject explosionPrefab;
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

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }

    public float CalculateHealthPercentage()
    {
        return (float)currentHP / (float)maxHP;
    }

    public void TakeDamage(int damage)
    {
        Instantiate(explosionPrefab, this.transform);
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
