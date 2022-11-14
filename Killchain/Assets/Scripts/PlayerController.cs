using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 10.0f;
    public float jumpHeight = 8.0f;
    public float gravity = 9.0f;
    public int maxHealth = 10;
    public float maxStamina = 100f;
    public float staminaDrain = 10f;
    public float staminaRegen = 10f;
    public float sprintMultiplier = 2f;
    public float lootDelay = 2f;
    public float damageReset = 1f;
    public int maxDamage = 15;

    private GameObject activeWeapon;
    private Vector3 move;
    private float nextFire;
    private Camera fpsCam;
    private CameraController camController;
    private int health;
    private float stamina;
    private Slider staminaBar;
    private Slider healthBar;
    private int damageTaken;
    private float damageTimer;
    private AudioSource hitSound;
    private ReloadingCircle reloadCircleScript;
    private bool weaponLooted;
    private float tempNextFire;
    private WeaponControl activeWeaponControl;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        fpsCam = GetComponentInChildren<Camera>();
        camController = GetComponentInChildren<CameraController>();
        camController.mouseSensitivity = PlayerPrefs.GetInt("Sensitivity");
        hitSound = GetComponent<AudioSource>();
        health = maxHealth;
        stamina = maxStamina;
        activeWeapon = GetComponentInChildren<WeaponControl>().transform.gameObject;
        staminaBar = GameObject.Find("StaminaBar").GetComponent<Slider>();
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        reloadCircleScript = GameObject.Find("ReloadCircle").GetComponent<ReloadingCircle>();
        weaponLooted = false;
    }

    void Update()
    {
        // If the Esc key is pressed
        if (Input.GetKey(KeyCode.Escape))
        {
            // Release the mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Load the main menu
            SceneManager.LoadScene(0);
        }
        
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

    public void SetNewWeapon(GameObject newWeapon)
    {
        // Sets the newWeapons parent to be the camera
        newWeapon.transform.SetParent(this.transform.GetChild(0), false);
        // Sets the newWeapons position and rotation to where the current weapon is
        newWeapon.transform.SetPositionAndRotation(activeWeapon.transform.position, activeWeapon.transform.rotation);
        // Delete the current weapon
        Destroy(activeWeapon);
        // Set the newWeapon as the active one
        activeWeapon = newWeapon;
        // Get reference to the newWeapons control script
        activeWeaponControl = activeWeapon.GetComponent<WeaponControl>();
        // Disable the shadow on the newWeapon as the player has no shadow so their gun shouldn't either
        activeWeapon.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        // Only adds a time delay if it isn't the weapon being set at the start
        if (Time.timeScale != 0)
        {
            // Tells the Attacking function if a weapon was looted this frame
            weaponLooted = true;
            nextFire = Time.time + lootDelay;
            // Activates the 'loading' animation
            reloadCircleScript.BeginAnimation(lootDelay);
        }
        // Updates the ammo counter to display the ammo of the new gun
        activeWeaponControl.DisplayAmmo();
    }

    void Movement()
    {
        // Only allows the player to change direction if they are touching the floor
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate move direction directly from axes
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Creates a movement vector for the player
            move = transform.right * x + transform.forward * z;
            move *= speed;
            // If the player is sprinting
            if (Input.GetKey(KeyCode.LeftShift) == true && stamina > 0 && move != Vector3.zero)
            {
                // Increase the players speed
                move *= sprintMultiplier;
                // Drain the players stamina
                stamina -= staminaDrain * Time.fixedDeltaTime;
                if (stamina < 0)
                {
                    stamina = 0;
                }
            }
            else if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                // If they aren't sprinting then regenerate the players stamina
                stamina += staminaRegen * Time.fixedDeltaTime;
                // Caps the stamina at 100
                if (stamina > 100)
                {
                    stamina = 100;
                }
            }

            // Add a jump vector to movement if the player jumps
            if (Input.GetButton("Jump"))
            {
                move += transform.up * jumpHeight;
            }
        }

        // Applies gravity to the player
        move.y -= gravity * Time.fixedDeltaTime;

        // Move the player
        characterController.Move(move * Time.fixedDeltaTime);
        // Updates the stamina bar
        staminaBar.value = stamina;
    }

    void Attacking()
    {
        // Checks for reloading or shooting and does whichever the player is doing (if either)
        if (Input.GetKeyDown("r") && Time.time > nextFire)
        {
            // Reloads the gun
            nextFire = activeWeaponControl.Reload();
            // If the gun is actually reloading (as if it is already at max ammo it doesn't do anything and returns the current Time)
            if (nextFire != Time.time)
            {
                // Activate reload animation
                reloadCircleScript.BeginAnimation(nextFire - Time.time);
            }
            // Updates the ammo counter on the hud
            activeWeaponControl.DisplayAmmo();
        }
        else if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            // Shoots forward from the centre of the camera
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
            // Tells the gun to fire
            tempNextFire = activeWeaponControl.Attack(rayOrigin);
            // If a weapon was looted this frame
            if (weaponLooted)
            {
                // Reset the weaponLooted variable
                weaponLooted = false;
            }
            else // If a weapon was not looted then continue on as normal
            {
                nextFire = tempNextFire;
                // If the gun auto-reloaded as a result of being out of ammo then activate the reload animation
                if (nextFire == Time.time + activeWeaponControl.reloadRate)
                {
                    reloadCircleScript.BeginAnimation(activeWeaponControl.reloadRate);
                }
            }
            // Updates the ammo counter on the hud
            activeWeaponControl.DisplayAmmo();
        }
    }

    public void Damage(int damage)
    {
        // Makes it so that per second, the player can only take a certain amount of damage
        if (damageTimer < Time.time)
        {
            damageTimer = Time.time + 1f;
            damageTaken = 0;
        }

        // If the damage threshold hasn't been reached then take damage
        if (damageTaken < maxDamage)
        {
            damageTaken += damage;
            // Takes damage and checks if the player is dead
            health -= damage;
            if (health <= 0)
            {
                // Currently only destroys the current weapon for testing purposes
                // Destroy(activeWeapon);
                GameObject.Find("LevelController").GetComponent<LevelController>().EndRun();
            }
            // Updates health bar and the tick timer
            healthBar.value = health;
        }
    }

    public void HitNoise()
    {
        // Plays the hitSound
        hitSound.Play();
    }
}