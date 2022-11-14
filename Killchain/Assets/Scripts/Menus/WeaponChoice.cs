using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChoice : MonoBehaviour
{
    public GameObject[] weapons;
    public Text highScoreText;

    private Dropdown weaponDropdown;
    private GameObject newWeapon;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        weaponDropdown = GetComponentInChildren<Dropdown>();
        // Pauses the game
        Time.timeScale = 0;
        // Loads the players high score
        highScoreText.text = PlayerPrefs.GetInt("High Score", 0).ToString();
    }

    public void Begin()
    {
        // Locks the mouse to the screen
        Cursor.lockState = CursorLockMode.Locked;
        // Loads the chosen weapon and sets it as the players new weapon
        newWeapon = Object.Instantiate(weapons[weaponDropdown.value]);
        player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().SetNewWeapon(newWeapon);
        // Unpauses the game
        Time.timeScale = 1;
        // Destroys itself as it's no longer needed
        Destroy(this.gameObject);
    }
}
