using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Receives accelerometer inputs and updates UI.
 *
 */
public class UpdateValues : MonoBehaviour
{
  
    public TextMeshProUGUI accelX;
    public TextMeshProUGUI accelY;
    public TextMeshProUGUI accelZ;

    public TextMeshProUGUI accelXFluxTextST;
    public TextMeshProUGUI accelYFluxTextST;
    public TextMeshProUGUI accelZFluxTextST;
    public TextMeshProUGUI AverageAccelFluxTextST;

    public TextMeshProUGUI accelXFluxTextLT;
    public TextMeshProUGUI accelYFluxTextLT;
    public TextMeshProUGUI accelZFluxTextLT;
    public TextMeshProUGUI AverageAccelFluxTextLT;

    public Vector3 accelValues;
    public FluctuationAnalyzer accelAnalyzer;
    public int windowSize = 128;
     
    float[] shortTermAccelSD = new float[4];

    string currentSceneName;
    private AudioManager audioManager;   
   
   
    // Start is called before the first frame update
    void Start()
    {       
        CreateAccelAnalyzer(windowSize);

        currentSceneName = SceneManager.GetActiveScene().name;            

        audioManager = FindObjectOfType<AudioManager>();    
    }

    private void Update()
    {
        if (SystemInfo.supportsAccelerometer)
        {
            accelValues = Input.acceleration;
            UpdateAccelValues(accelValues);           
        }
    }

    public void UpdateAccelValues(Vector3 values)
    {
        // Add new accelerometer values to short term and long term lists
        accelAnalyzer.AddDataToLists(values);

        // Calculate short term standard deviation
        shortTermAccelSD = accelAnalyzer.ObtainShortTermSD();

        // Send values to Audio Manager
        SendAccel(shortTermAccelSD);
        
        //Update frequent UI Elements 
        accelXFluxTextST.text = "SD X: " + shortTermAccelSD[0];
        accelYFluxTextST.text = "SD Y: " + shortTermAccelSD[1];
        accelZFluxTextST.text = "SD Z: " + shortTermAccelSD[2];
        AverageAccelFluxTextST.text = "Average SD: " + shortTermAccelSD[3];

        accelX.text = "X: " + accelValues.x;
        accelY.text = "Y: " + accelValues.y;
        accelZ.text = "Z: " + accelValues.z;
    }

    public void UpdateLongTermValues(float []longTermAccelSD)
    {
        //Update long term UI Elements
        accelXFluxTextLT.text = "SD X: " + longTermAccelSD[0];
        accelYFluxTextLT.text = "SD Y: " + longTermAccelSD[1];
        accelZFluxTextLT.text = "SD Z: " + longTermAccelSD[2];
        AverageAccelFluxTextLT.text = "Average SD: " + longTermAccelSD[3];
    }

    // Send values to Audio Manager
    public void SendAccel(float []accelValues)
    {         
        audioManager.shortTermAccelSD = accelValues;             
    }

    // Initialize Fluctuation Analyzer
    public void CreateAccelAnalyzer(int size)
    {
       accelAnalyzer = new FluctuationAnalyzer(windowSize: size);      
    }

}
