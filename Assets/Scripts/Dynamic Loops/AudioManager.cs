using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Changes music loops of each instrument depeding on the Long Term SD values. Changes mixer levels according to Short Term SD values.
 * Also in charge of playing and pausing music. *
 */

public class AudioManager : MonoBehaviour
{    
    public AudioSource bassSource;
    public AudioSource pianoSource;
    public AudioSource melodySource;
    public AudioSource drumSource;
    public AudioSource[] instrumentSources = new AudioSource[4];

    
    public AudioClip[] bassClips;
    public AudioClip[] pianoClips;
    public AudioClip[] melodyClips;
    public AudioClip[] drumsClips;   

    bool finishedPlaying;
    
    public float[] shortTermAccelSD = new float[4];
    public float[] longTermAccelSD = new float[4];

    private int bassIndex;
    private int pianoIndex;
    private int melodyIndex;
    private int drumsIndex;

    private bool[] changeClip = new bool[4];

    private UpdateValues updateValues;

    public TextMeshProUGUI buttonText;
    public bool activePlayback;

    public AudioMixer audioMixer;
    public bool audioLevelMode;

    public TextMeshProUGUI accelXFluxTextLT;
    public TextMeshProUGUI accelYFluxTextLT;
    public TextMeshProUGUI accelZFluxTextLT;
    public TextMeshProUGUI totalAccelFluxTextLT;


    // Start is called before the first frame update
    void Start()
    {
        audioLevelMode = false;
        
        instrumentSources[0] = bassSource;
        instrumentSources[1] = pianoSource;
        instrumentSources[2] = melodySource;
        instrumentSources[3] = drumSource;

        updateValues = FindObjectOfType<UpdateValues>();

        PreloadAudioData(bassClips);
        PreloadAudioData(pianoClips);
        PreloadAudioData(drumsClips);
        PreloadAudioData(melodyClips);
    }

    // Update is called once per frame
    void Update()
    {   
        // If the current loop has finished playing and music is active, update audio clips
        if (finishedPlaying&&activePlayback)
        {
            finishedPlaying = false;


            if (changeClip[0])
            {
                bassSource.clip = bassClips[bassIndex];
            }
            if(changeClip[1])
            {
                pianoSource.clip = pianoClips[pianoIndex];
            }
            if(changeClip[2])
            {
                melodySource.clip = melodyClips[melodyIndex];
            }
            if(changeClip[3])
            {
                drumSource.clip = drumsClips[drumsIndex];
            }        
                         
            foreach (AudioSource audioSource in instrumentSources)
            {
                audioSource.Play();                
            }          
            //Start timers again
            StartCoroutine(CheckClipEnd());
            StartCoroutine(GetLongTermFlux());
        }

        // If dynamic audio levels are active, update mixer levels every frame according to short term SD
        if(audioLevelMode)
        {
            UpdateDBLevel("BassLevel", shortTermAccelSD[0]);
            UpdateDBLevel("PianoLevel", shortTermAccelSD[1]);
            UpdateDBLevel("MelodyLevel", shortTermAccelSD[2]);
            UpdateDBLevel("DrumsLevel", shortTermAccelSD[3]);
        }
    }

    private void PreloadAudioData(AudioClip[] clips)
    {
        foreach (AudioClip audio in clips)
        {
            audio.LoadAudioData();
        }
    }

    // Check if current audio clip has ended if an amount of time equal to the duration of the clip passes
    IEnumerator CheckClipEnd()
    {
        yield return new WaitForSeconds(8);
        finishedPlaying = true;
    }

    // Get the audio clip intesnity indexes for next round before the current audio clip ends
    IEnumerator GetLongTermFlux()
    {
        yield return new WaitForSeconds(7.5f);
        longTermAccelSD = updateValues.accelAnalyzer.ObtainLongTermSD();
              
        bassIndex = NeedsToChangeClip(bassIndex, 0);
        pianoIndex = NeedsToChangeClip(pianoIndex, 1);
        melodyIndex = NeedsToChangeClip(melodyIndex, 2);
        drumsIndex = NeedsToChangeClip(drumsIndex, 3);

        updateValues.UpdateLongTermValues(longTermAccelSD);       
    }

    // Check if an instrument needs to change its audio clip depending on long term standard deviation
    private int NeedsToChangeClip(int clipIndex, int arrayIndex)
    {
        int currentClipIndex = UpdateIndexes(longTermAccelSD[arrayIndex]);
        changeClip[arrayIndex] = false;
        if (clipIndex != currentClipIndex)
        {
            changeClip[arrayIndex] = true;
        }
        return currentClipIndex;
    }

    // Obtain an index depending on the range of long term fluctuation
    public int UpdateIndexes(float fluctuation)
    {
        int index;

        if (fluctuation < 0.3f)
        {
            index = 0;
        }
        else if (fluctuation > 0.3f && fluctuation < 0.9f)
        {
            index = 1;
        }
        else if (fluctuation > 0.9f && fluctuation < 1.5f)
        {
            index = 2;
        }
        else
        {
            index = 3;
        }
        return index;
    }

    // Play/Stop the music
    public void ToggleAudio()
    {
        if (activePlayback)
        {
            foreach (var audioSource in instrumentSources)
            {
                audioSource.Stop();                
            }
            StopAllCoroutines();
            finishedPlaying = false;
            buttonText.text = "Play";
        }
        else
        {
            ResetIntensityIndexes();
            foreach (var audioSource in instrumentSources)
            {
                audioSource.Play();                
            }
            StartCoroutine(CheckClipEnd());
            StartCoroutine(GetLongTermFlux());
            updateValues.accelAnalyzer.ClearLTLists();
            finishedPlaying = false;
            buttonText.text = "Stop";            
        }

        activePlayback = !activePlayback;
    }

    // Set all audio clips to the lowest intensity
    private void ResetIntensityIndexes()
    {
        bassIndex = 0;
        pianoIndex = 0;
        melodyIndex = 0;
        drumsIndex = 0;

        bassSource.clip = bassClips[0];
        pianoSource.clip = pianoClips[0];
        melodySource.clip = melodyClips[0];
        drumSource.clip = drumsClips[0];
    }

    // Activate dynamic mixer levels
    public void ActivateDynamicAudioLevel()
    {
        audioLevelMode = !audioLevelMode;
        ResetMixerLevels();
    }

    // Update mixer level of a certain channel
    public void UpdateDBLevel(string channelName, float fluctuation)
    {
        audioMixer.SetFloat(channelName, LerpValue(fluctuation));
    }

    // Adjust mixer levels between 0 dB and -6 dB using linear interpolation, depeding on the incoming value
    private float LerpValue(float value)
    {
        if (value <= 0f) return -6f;
        if (value >= 1.5f) return 0f;

        float mappedValue = Mathf.Lerp(-6f, 0f, value / 1.5f);

        return mappedValue;
    }

    // Set all mixer channels back to 0 dB
    private void ResetMixerLevels()
    {
        audioMixer.SetFloat("BassLevel", 0);
        audioMixer.SetFloat("PianoLevel", 0);
        audioMixer.SetFloat("MelodyLevel", 0);
        audioMixer.SetFloat("DrumsLevel", 0);
    }


    

}
