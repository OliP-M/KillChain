using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health;
    private string weapon;

    /// 'Hits' the target for a certain amount of damage
    public string Damage(float damage)
    {
        //Deals the given damage and destroys the object if dead
        health -= damage;
        if (health <= 0)
        {
            weapon = GameObject.FindGameObjectsWithTag("Weapon")[0].name;
            Destroy(gameObject);
            return weapon;
        }
        return null;
    }
}