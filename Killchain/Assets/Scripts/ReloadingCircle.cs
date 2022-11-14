using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadingCircle : MonoBehaviour
{
    private Image circleLoader;
    private float elapsedTime;
    private float reloadTime;
    private bool reloading;

    // Start is called before the first frame update
    void Start()
    {
        circleLoader = GetComponent<Image>();
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If the animation is active
        if (reloading)
        {
            // Track time passed between frames
            elapsedTime += Time.deltaTime;
            // If the timer is up
            if (elapsedTime > reloadTime)
            {
                // Reset the circle and stop the animation
                circleLoader.fillAmount = 0;
                reloading = false;
            }
            else
            {
                // Set the fill amount to be proportional to elapsedTime out of total time
                circleLoader.fillAmount = elapsedTime / reloadTime;
            }

        }
    }

    public void BeginAnimation(float tempReloadTime)
    {
        // Begins the animation by setting the elapsedTime to 0 and the total time for the animation to occur over
        reloadTime = tempReloadTime;
        reloading = true;
        elapsedTime = 0;
    }
}
