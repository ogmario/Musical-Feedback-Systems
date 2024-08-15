using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Controls calibration screen UI elements
*
*/
public class CalibrateCounter : MonoBehaviour
{
    public Image panel;
    public Slider accelSlider;
    public Slider timeSlider;
    public Slider repSlider;
    public TextMeshProUGUI thText;
    public TextMeshProUGUI delText;
    public TextMeshProUGUI repText;

    private CalibrationValues calibValues;
    private Counter counter;


    void Start()
    {
        counter = FindObjectOfType<Counter>();
        calibValues = FindObjectOfType<CalibrationValues>();
        counter.delayThreshold = calibValues.delayThreshold;
        counter.movementThreshold = calibValues.movementThreshold;
        counter.ActivateCounter();
    }

    private void Update()
    {
        // Change background colors from red to blue and viceversa with every movement
        if (counter.counter % 2 == 0)
        {
            panel.color = Color.red; // If even, set color to red
        }
        else
        {
            panel.color = Color.blue; // If odd, set color to blue
        }
    }

    // Change minimum accelerometer threshold according to the slider
    public void AdjustThreshold()
    {
        calibValues.movementThreshold = accelSlider.value;
        counter.movementThreshold = calibValues.movementThreshold;
        thText.text = "" + System.Math.Round(counter.movementThreshold,2);
    }
    // Change delay threshold according to the slider
    public void AdjustDelay()
    {
        calibValues.delayThreshold = timeSlider.value;
        counter.delayThreshold = calibValues.delayThreshold;
        delText.text = "" + System.Math.Round(counter.delayThreshold, 2);
    }
    // Change target movements according to the slider
    public void AdjustMaxReps()
    {
        calibValues.maxReps = (int)repSlider.value;
        repText.text = "" + calibValues.maxReps;               
    }
}



