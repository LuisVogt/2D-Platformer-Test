using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitbox : MonoBehaviour
{
    public int damage;
    int targetLayer = 9; //put layer of the creature that will be affected;

    public void init(int _targetLayer)
    {
        Debug.Log("initing " + _targetLayer);
        targetLayer = _targetLayer;
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
    }
}
