using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLooting : MonoBehaviour
{
    private GameObject player;

    // Start is called before the first frame update
    public void Loot()
    {
        player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().SetNewWeapon(gameObject);    // Calls a function that destroys the active gun
            // Then moves the gun this script is attached to to the correct position
    }
}
