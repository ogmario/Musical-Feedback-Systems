using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Increases a counter value whenever a significant movement is detected.
*
*/
public class Counter : MonoBehaviour
{
    private Vector3 lastFrameAcc;
    private bool hasSignChanged = false;
    public TextMeshProUGUI counterText;
    public int counter;

    public TextMeshProUGUI xAccel;
    public TextMeshProUGUI yAccel;
    public TextMeshProUGUI zAccel;

    private float timeSinceLastIncrement = 0f;
    public  float delayThreshold = 0.3f;
    public float movementThreshold = 0.3f;  // Threshold for movement detection

    private CalibrationValues calibValues;
    public bool countReps = false;
    public bool calibrating = false;
    

    void Start()
    {
        lastFrameAcc = Input.acceleration;
        //If there are calibration values for the thresholds, update them
        calibValues = FindObjectOfType<CalibrationValues>();
        if (calibValues != null)
        {
            delayThreshold = calibValues.delayThreshold;
            movementThreshold = calibValues.movementThreshold;            
        }
        //Check if this is the calibration scene or the AudioFX scene
        if(FindObjectOfType<CalibrateCounter>() != null)
        {
            calibrating = true;
        }

    }

    void Update()
    {
        if(countReps) //If counter active
        {
            Vector3 currentAcc = Input.acceleration; // get current accel values
            timeSinceLastIncrement += Time.deltaTime; //add to the time since last increment

            // Check for significant sign change on any axis
            if ((SignificantSignChanged(lastFrameAcc.x, currentAcc.x) ||
                SignificantSignChanged(lastFrameAcc.y, currentAcc.y) ||
                SignificantSignChanged(lastFrameAcc.z, currentAcc.z)) && timeSinceLastIncrement >= delayThreshold)
            {
                if (!hasSignChanged) 
                {
                    hasSignChanged = true; // Significant movement
                    OnSignChange();  // Increase counter
                    timeSinceLastIncrement = 0f; // Reset delay tiemer
                }
            }
            else
            {
                hasSignChanged = false; // No significant movement
            }

            lastFrameAcc = currentAcc;

            if(!calibrating)
            {
                UpdateUI(currentAcc); //updateUI
            }            
        }
        
    }

    //Check if movement threshold was crossed
    private bool SignificantSignChanged(float lastValue, float currentValue)
    {
        // Check both the sign change and if the change exceeds the threshold
        return ((lastValue >= 0 && currentValue < 0) || (lastValue < 0 && currentValue >= 0)) &&
               Mathf.Abs(currentValue - lastValue) > movementThreshold;
    }

    // Increase counter
    private void OnSignChange()
    {
        counter++;
        counterText.text = counter.ToString();       
    }

    // Reset counter
    public void ResetCounter()
    {
        counter = 0;
        counterText.text = counter.ToString();
        timeSinceLastIncrement = 0f;  // Reset the delay timer on counter reset
    }

    // Stop counting
    public void PauseCounter()
    {
        countReps = false;
    }
    
    //Cotinue counting
    public void ActivateCounter()
    {
        countReps = true;
    }

    //Update UI accelerometer readings
    public void UpdateUI(Vector3 vector)
    {
        xAccel.text = "X: " + vector.x;
        yAccel.text = "Y: " + vector.y;
        zAccel.text = "Z: " + vector.z;
    }
}



