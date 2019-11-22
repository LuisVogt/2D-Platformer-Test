using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public Vector2 respawnPoint;
    public int damage;
    public int targetLayer;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        Creature creature = collided.GetComponent<Creature>();
        if (creature)
        {
            creature.TakeDamage(damage);
        }
        if (collision.gameObject.layer == targetLayer)
        {
            collided.transform.SetPositionAndRotation(respawnPoint, Quaternion.identity);
        }
    }
}
