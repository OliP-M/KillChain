using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 10;

    /// 'Hits' the target for a certain amount of damage
    public void Damage(int damage)
    {
        //Deals the given damage and destroys the object if dead
        health -= damage;
        if (health <= 0)
        {
            gameObject.GetComponentInChildren<WeaponLooting>().Loot();
            Destroy(gameObject);
        }
    }
}