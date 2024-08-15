using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;
/*
* Author:Mario Ortega García
* Date: August 2024
* Description: Updates Audio Fx according to number of movements whenever a looping timer ends. Also plays and stops music.
*/
public class AudioFXManager : MonoBehaviour
{
    public AudioSource songSource;
    public AudioMixer mixer;
    bool activePlayback;
    public float timerTime;
       
    public float maxReps = 8;
    public float minReps = 0;

    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI distText;
    public TextMeshProUGUI cutoffText;
    
    public Counter counter;

    public FilePicker filePicker;
    public string oldFilePath;

    private float maxExponent = Mathf.Log10(22000);


    // Start is called before the first frame update

    void Start()
    {
        activePlayback = false;
        oldFilePath = null;
        timerTime = 4f;

        // If values have been change in calibration, update target movements
        CalibrationValues calibValues = FindObjectOfType<CalibrationValues>();
        if (calibValues != null)
        {
            maxReps = calibValues.maxReps;           
        }

    }
    // Play and stop music
    public void ToggleAudio()
    {
        if (activePlayback)
        {
            StopMusic();  
        }
        else
        {
            PlayMusic(); 
        }

        activePlayback = !activePlayback;
    }
    //Stop music
    public void StopMusic()
    {
        songSource.Stop();
        buttonText.text = "Play";
        filePicker.path.text = "";
        counter.PauseCounter();
        StopAllCoroutines();
    }

    //Prepare to play music
    public void PlayMusic()
    {
        if(oldFilePath!=filePicker.filePath) // Check if a new audio file path has been chosen
        {
            filePicker.PlayAudioFromFile();
            oldFilePath = filePicker.filePath;
        }

        Invoke("ActuallyPlay", 0.5f); // Start playing song only after everything has been set up
        counter.ActivateCounter();
        StartCoroutine(IncrementTracker());
        buttonText.text = "Stop";
        filePicker.path.text = "Now Playing: " + filePicker.filename;
    }
    // Actually play music
    public void ActuallyPlay()
    {
        songSource.Play();
    }

    // Looping timer, updates effects whenever it ends
    IEnumerator IncrementTracker()
    {
        float timer=0;

        while (true)
        {
            // Check if the timer is over
            if (timer >= timerTime)
            {
                // Update FX if time is up
                UpdateFX(counter.counter);

                // Reset count and timer
                counter.ResetCounter();
                timer = 0f;
            }

            // Increment timer
            timer += Time.deltaTime;

            yield return null;
        }
    }

    // Calculate FX parameters and update UI
    public void UpdateFX(int value)
    {
        float dist = DistNormalization(value);
        float cutoff = FilterNormalization(value);

        mixer.SetFloat("Dist", dist);
        mixer.SetFloat("Cutoff", cutoff);

        distText.text = "Distortion: " + dist;
        cutoffText.text = "Cutoff Freq: " + cutoff;
    }

    //Map distortion levels from 0 to 0.5 depending on counter
    public float DistNormalization(int value)
    {
        float distLevel;
        if (value >= maxReps)
        {
            distLevel = 0;
        }
        else
        {
            distLevel = 0.5f*(1 - (value / maxReps));
        }
        
        return distLevel;
    }

    //Map filter cutoff logarithmically from 100 to 22000 depending on counter
    float FilterNormalization(int input)
    {
        input = Mathf.Clamp(input, 0, 8);
        float exponent = Mathf.Lerp(2, maxExponent, input / maxReps);
        float output = Mathf.Clamp(Mathf.Pow(10, exponent), 100, 22000);
        
        return output;
    }
}
