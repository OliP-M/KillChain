using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponControl : MonoBehaviour
{
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public int pellets = 1;
    public int maxAmmo = 20;
    public float offset = 0.05f;
    public float reloadRate = 2f;
    
    public GameObject bulletHole;
    public AudioSource gunAudio;
    public AudioSource reloadSound;

    private Transform gunEnd;
    private ParticleSystem muzzleFlash;
    private int ammo;
    private Text ammoText;

    // Start is called before the first frame update
    void Awake()
    {
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        ammoText = GameObject.Find("AmmoCount").GetComponent<Text>();
        gunEnd = muzzleFlash.transform;
        ammo = maxAmmo;
    }

    public float Attack(Vector3 rayOrigin)
    {
        // Checks if the gun is loaded
        if (ammo > 0)
        {
            // Plays the audio attached to the gun and creates a muzzleflash effect
            gunAudio.Play();
            muzzleFlash.Play();

            // Calculates a hit for each pellet/bullet shot
            for (int i = 0; i < pellets; i++)
            {
                // Creates a (constrained) random vector3 and maps it to the gunEnd rotation (aiming it forward)
                Vector3 dir = new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), 1f);
                Vector3 sprayDir = gunEnd.TransformVector(dir);

                // Shoots a raycast from the desired point of origin and checks for hits
                if (Physics.Raycast(rayOrigin, sprayDir, out RaycastHit hit, weaponRange))
                {
                    // Checks if the hit object is an enemy and deals damage
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        hit.collider.GetComponent<EnemyControl>().Damage(gunDamage);
                    }
                    // Checks if the hit object is the player and deals damage
                    else if (hit.transform.CompareTag("Player"))
                    {
                        hit.collider.GetComponent<PlayerController>().Damage(gunDamage);
                    }
                    else
                    {
                        // if the object hit was neither a player or an enemy, spawn a bullethole at the hit position
                        GameObject obj = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                        obj.transform.position += obj.transform.forward / 1000;
                        // The bullethole is told to despawn after 15s
                        Destroy(obj, 15f);
                    }
                }
            }

            // Reduces ammo in the gun and sets the delay before the gun can next be fired
            ammo -= 1;
            return (Time.time + fireRate);
        }
        else // If the gun is fired and there is no ammo then reload the gun
        {
            return (Reload());
        }
    }

    public float Reload()
    {
        // Reloads the gun
        if (ammo != maxAmmo)
        {
            // Plays a reload sound
            reloadSound.Play();
            ammo = maxAmmo;
            // Sets the delay before the gun can next be fired
            return (Time.time + reloadRate);
        }
        // If the gun already has full ammo then do nothing
        return (Time.time);
    }

    public void DisplayAmmo()
    {
        // Updates the ammo counter
        ammoText.text = ammo.ToString() + "/" + maxAmmo.ToString();
    }
}