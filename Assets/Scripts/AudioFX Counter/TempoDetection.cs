using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Adjusts counter time window according to song tempo.
*
*/
public class TempoDetection : MonoBehaviour
{

    private int bpm;
    private int realBpm;
    private int maxBpm = 160;
    private int minBpm = 60;
    public AudioFXManager audioFxManager;
    public Toggle tempoToggle;
    public TextMeshProUGUI bpmText;
    bool activeTempoDetection = false;

    // Detect tempo if checkbox is on, otherwise reset timer
    public void TempoCheckBox()
    {
        activeTempoDetection = tempoToggle.isOn;
        if (activeTempoDetection)
        {
            DetectTempo(); 
        }
        else
        {
            ResetTimerTime();
        }      
    }
    
    public void DetectTempo()
    {
        // Obtain tempo
        realBpm = UniBpmAnalyzer.AnalyzeBpm(audioFxManager.songSource.clip);
        Debug.Log("BPM is " + realBpm);
        bpm = realBpm;
        // Divide or multiply real tempo if it exceeds the tempo range
        if (bpm > maxBpm)
        {
            while (bpm > maxBpm)
            {
                bpm = bpm / 2;
            }
        }
        else if (bpm<minBpm)
        {
            while (bpm<minBpm)
            {
                bpm = bpm* 2;
            }
        }
        float newTimertime = Mathf.Round(8f/(bpm/60f));  //what's the duration in seconds for 8 beats at the current tempo
        Debug.Log("bpm is " + bpm + " calculated time is " + newTimertime);
        audioFxManager.timerTime = newTimertime; //Update counter time window
        bpmText.text = "BPM Detection: " + realBpm + " BPM (" + audioFxManager.timerTime + "s)";
    }

    // Set timer back to 4 seconds
    public void ResetTimerTime()
    {
        audioFxManager.timerTime = 4f;
        bpmText.text = "BPM Detection";
    }

}
