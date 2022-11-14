using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject volumeSlider;
    public GameObject mouseSlider;
    private float volume;
    private int sensitivity;
    // Start is called before the first frame update
    void Start()
    {
        volume = PlayerPrefs.GetFloat("Volume", 0.5f);
        sensitivity = PlayerPrefs.GetInt("Sensitivity", 5);
        volumeSlider.GetComponent<Slider>().value = volume;
        mouseSlider.GetComponent<Slider>().value = sensitivity;
    }

    public void SaveSettings()
    {
        // Gets the values of each slider
        volume = volumeSlider.GetComponent<Slider>().value;
        sensitivity = (int)mouseSlider.GetComponent<Slider>().value;
        // Saves the values in player prefs
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetInt("Sensitivity", sensitivity);
        // Sets the global volume
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
    }
}
