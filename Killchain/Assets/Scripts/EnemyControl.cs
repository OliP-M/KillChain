using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
    public float rotateStrength = 2f;
    public int health = 10;

    // Weighting variables
    [Header("Weight Variables")]
    public int relativeCoverWeight = 20;
    public int coverWeight = 5;
    public int travelWeight = 1;
    [Header("Parabolic Distance Variables - Edit with caution")]
    public float distanceIntensity = 0.2f;
    public int preferredDistance = 5;
    public int distanceWeight = 30;

    private Transform player;
    private PlayerController playerScript;
    private Transform capsuleTransform;
    private Quaternion targetRotation;
    private float rotate;
    private float nextFire;
    private WeaponControl weapon;
    private Transform gunEnd;
    private float weaponRange;
    private NavMeshAgent agent;
    private LevelController controlScript;
    private Vector3[] posArray;
    private int[] cover;
    private int destination;
    private float reassess;
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        playerScript = player.GetComponent<PlayerController>();
        capsuleTransform = GetComponent<Transform>();
        weapon = GetComponentInChildren<WeaponControl>();
        weaponRange = weapon.weaponRange;
        gunEnd = weapon.transform.GetChild(0);
        agent = GetComponent<NavMeshAgent>();
        controlScript = GameObject.Find("LevelController").GetComponent<LevelController>();
        GetPositions();
        reassess = Time.time + 0.5f;
    }

    // LateUpdate is called once per frame after all update calls
    void LateUpdate()
    {
        // If the game is paused then dont run the attacking code
        if (Time.timeScale != 0)
        {
            Attacking();
        }
    }

    // FixedUpdate is called a set amount of times per second and is used for Physics calculations
    void FixedUpdate()
    {
        // If the game is paused then don't run the movement code
        if (Time.timeScale != 0)
        {
            Movement();
        }
    }

    void Movement()
    {
        // Shoots a ray towards the player
        Vector3 toPlayer = (player.position - capsuleTransform.position);
        Physics.Raycast(capsuleTransform.position, toPlayer, out RaycastHit hit, weaponRange);
        // If the player can be seen and is within the weapon range (determined by the raycast length)
        if (hit.transform == player)
        {
            // Releases the destination if it can see the player and it isnt close to its destination
            if (destination != -1 && Vector3.Distance(capsuleTransform.position, posArray[destination]) > 3)
            {
                controlScript.ReleasePosition(destination);
                destination = -1;
                // Stops the enemys movement if they aren't close to their destination
                agent.destination = capsuleTransform.position;
            }

            // Rotates the enemy to face the player
            targetRotation = Quaternion.LookRotation(player.position - capsuleTransform.position);
            rotate = Mathf.Min(rotateStrength * Time.deltaTime, 1);
            capsuleTransform.rotation = Quaternion.Lerp(capsuleTransform.rotation, targetRotation, rotate);

            // if the player is within 25deg cone infront of the enemy
            // rotate the gun to face the player
            // Note: was originally intended for vertical use and may not be required in a 2d map
            // Currently causes the enemies to swing their weapons towards the player quicker
            if (Vector3.Angle(capsuleTransform.forward, toPlayer) < 25)
            {
                targetRotation = Quaternion.LookRotation(player.position - gunEnd.position);
                rotate = Mathf.Min(rotateStrength * Time.deltaTime, 0.5f);
                weapon.transform.rotation = Quaternion.Lerp(gunEnd.rotation, targetRotation, rotate);
            }
        }
        else
        {
            // AI reassesses the situation every second
            // this prevents calculations being excessively performed every frame
            if (Time.time > reassess)
            {
                reassess = Time.time + 1;

                // Releases the current destination so the below code can recheck its weight
                controlScript.ReleasePosition(destination);

                // Sets the initial variables
                float weight;
                float chosenWeight = -10000;

                // Gets the list of taken positions
                List<int> taken = controlScript.GetTaken();
                // Loops through all possible positions and assesses them
                for (int i = 0; i < posArray.Length; i++)
                {
                    // If position is already taken then don't bother checking it
                    if (!taken.Contains(i))
                    {
                        // Calculates the weight of the current position
                        weight = (cover[i]) + WeightCalculation(posArray[i]);
                        // Finds the highest weight of all the positions and sets the destination variable to point to that position
                        if (weight > chosenWeight)
                        {
                            chosenWeight = weight;
                            destination = i;
                        }
                    }
                }

                // Claims the chosen position and checks it isn't already chosen
                // Note: this should always return true but its here just in case
                if (controlScript.ClaimPosition(destination))
                {
                    // Sets the navmesh agent destination to be the chosen destination
                    agent.destination = posArray[destination];
                }
                else
                {
                    // if it returns false, something has gone wrong
                    // Stops movement and re-assesses next frame instead of next second
                    destination = -1;
                    agent.destination = capsuleTransform.position;
                    reassess = Time.time;
                }
            }
        }
    }

    void Attacking()
    {
        // If the enemies gun can be used
        if (Time.time > nextFire)
        {
            // Shoots a ray towards the player from the gun end
            Vector3 toPlayer = (player.position - gunEnd.position);
            Physics.Raycast(gunEnd.position, toPlayer, out RaycastHit hit, Mathf.Infinity);

            // if the player isn't in a 25deg cone forwards then the enemy reloads
            if (Vector3.Angle(gunEnd.forward, toPlayer) > 25)
            {
                nextFire = weapon.Reload();

            }   // if the player is in a 10deg cone forwards and can be seen by the gun then it shoots
            else if (Vector3.Angle(gunEnd.forward, toPlayer) < 10 && hit.transform == player && hit.distance < weaponRange)
            {
                Vector3 rayOrigin = gunEnd.position;
                nextFire = weapon.Attack(rayOrigin);
            }
        }
    }

    float WeightCalculation(Vector3 pos)
    {
        float weight = 0;

        // Maps the distance to the player onto a parabola, giving a preferred distance, with values near to it being weighted best
        weight += (-distanceIntensity) * (Vector3.Distance(pos, player.position) - preferredDistance) + distanceWeight;

        // Distance from the Enemies current location
        weight -= Vector3.Distance(pos, capsuleTransform.position) * travelWeight;

        // Cover relative to player
        Vector3 coverPos = pos + Vector3.up * 0.75f;
        Vector3 toPlayer = (player.position - coverPos);
        // Raycasts towards the player a small distance
        if (Physics.Raycast(coverPos, toPlayer, out RaycastHit hit, 1.5f))
        {
            // If the raycast hits a bit of map then there is relative cover
            if (hit.transform.CompareTag("Map"))
            {
                weight += relativeCoverWeight;
            }
        }

        return weight;
    }

    public void Damage(int damage)
    {
        // Returns if the enemy is already dead
        if (dead) { return; }

        // Deals the given damage
        health -= damage;

        // Plays a hit noise so the player knows they've hit the enemy
        playerScript.HitNoise();

        // If the shot has killed them
        if (health <= 0)
        {
            // Ensures that an enemy cannot be "killed" more than once
            dead = true;
            // Sends the weapon to the player
            playerScript.SetNewWeapon(weapon.gameObject);
            // Releases the claimed position
            controlScript.ReleasePosition(destination);
            // Tells the LevelController that it has been killed
            controlScript.EnemyKilled();
            // Destroys itself
            Destroy(gameObject);
        }
    }

    void GetPositions()
    {
        // Gets the positions list and all the cover values for each position
        cover = controlScript.GetCover();
        for (int i = 0; i < cover.Length; i++)
        {
            cover[i] *= coverWeight;
        }
        posArray = controlScript.GetPositions();
    }
}