using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAI : MonoBehaviour
{
    public float rotateStrength = 2f;
    public int gunDamage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float hitForce = 100f;
    public int pellets = 1;
    public int maxAmmo = 20;
    public float offset = 0.05f;
    public float reloadRate = 2f;

    private Transform player;
    private Transform capsuleTransform;
    private Quaternion targetRotation;
    private float rotate;
    private int ammo;
    private float nextFire;
    private float nextReload;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private LineRenderer laserLine;
    public Transform gunEnd;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        capsuleTransform = GetComponent<Transform>();
        laserLine = GetComponentInChildren<LineRenderer>();
        //gunEnd = transform.Find("Assault").Find("Gun");
        ammo = maxAmmo;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        targetRotation = Quaternion.LookRotation(player.position - capsuleTransform.position);
        rotate = Mathf.Min(rotateStrength * Time.deltaTime, 1);
        capsuleTransform.rotation = Quaternion.Lerp(capsuleTransform.rotation, targetRotation, rotate);

        Vector3 toPlayer = (player.position - capsuleTransform.position);
        if (Vector3.Angle(capsuleTransform.forward, toPlayer) < 10 && Time.time > nextFire && ammo > 0)
        {
            // Sets values enrelated to shot calculation
            ammo -= 1;
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());

            // Calculates the current centre of the camera.
            Vector3 rayOrigin = gunEnd.position;
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

                    if (health != null)
                    {
                        health.Damage(gunDamage);
                    }

                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * hitForce);
                    }
                }
                else
                {
                    laserLine.SetPosition(1, rayOrigin + (capsuleTransform.forward * weaponRange));
                }
            }
        }
        else if ((Vector3.Angle(capsuleTransform.forward, toPlayer) > 25 && Time.time > nextFire && ammo != maxAmmo) || (Time.time > nextFire && ammo == 0))
        {
            // StartCoroutine(ReloadEffect());
            // will eventually have a reload
            nextReload = Time.time + reloadRate;
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
