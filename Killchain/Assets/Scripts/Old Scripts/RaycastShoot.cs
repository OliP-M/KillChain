using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour
{
    // these are the variables that make each gun different
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public int pellets = 1;
    public int maxAmmo = 20;
    public float offset = 0.05f;
    public float reloadRate = 2f;
    public GameObject[] weapons;

    private Transform gunEnd;
    private Camera fpsCam;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private AudioSource gunAudio;
    private LineRenderer laserLine;
    private int ammo;
    private float nextFire;

    void Start()
    {
        laserLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        fpsCam = GetComponentInParent<Camera>();
        gunEnd = transform.Find("GunEnd");
        ammo = maxAmmo;
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire && ammo > 0)
        {
            // Sets values enrelated to shot calculation
            ammo -= 1;
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());

            // Calculates the current centre of the camera.
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit hit;

            laserLine.SetPosition(0, gunEnd.position);
            //Calculates a hit for each pellet/bullet shot
            for (int i = 0; i < pellets; i++)
            {
                //Creates a (constrained) random vector3 and maps it to the gunEnd rotation (aiming it forward)
                Vector3 dir = new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), 1f);
                Vector3 sprayDir = gunEnd.TransformVector(dir);

                //Shoots a raycast from the centre of the camera and checks for hits
                if (Physics.Raycast(rayOrigin, sprayDir, out hit, weaponRange))
                {
                    laserLine.SetPosition(1, hit.point);

                    //Deals damage and applies a force to any object hit (if it is viable)
                    Target health = hit.collider.GetComponent<Target>();
                    string weapon;

                    if (health != null)
                    {
                        weapon = health.Damage(gunDamage);
                        Debug.Log(weapon);
                        if (weapon != null)
                        {
                            weapons[0].SetActive(true);
                            gameObject.SetActive(false);
                        }
                    }

                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * hitForce);
                    }
                }
                else
                {
                    laserLine.SetPosition(1, rayOrigin + (sprayDir * weaponRange));
                }
            }
        }
        //Checks for the reload key, or attempting to shoot an empty weapon, then reloads it
        else if ((Input.GetKeyDown("r") && Time.time > nextFire  && ammo != maxAmmo) || (Input.GetButton("Fire1") && Time.time > nextFire && ammo == 0))
        {
            // StartCoroutine(ReloadEffect());
            // will eventually have a reload
            nextFire = Time.time + reloadRate;
            ammo = maxAmmo;
            // put in a delay here for the reload animation
        }
    }

    private IEnumerator ShotEffect()
    {
        //gunAudio.Play();

        laserLine.enabled = true;
        yield return shotDuration;
        laserLine.enabled = false;
    }
}