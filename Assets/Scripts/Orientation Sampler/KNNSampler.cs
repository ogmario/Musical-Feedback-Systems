using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KNNSampler : MonoBehaviour
{
    public AudioSource[] source = new AudioSource[5];
    public AudioClip[] drumClips = new AudioClip[5];
    public AudioClip[] pianoClips = new AudioClip[5];
    public Image pianoImage;
    public Image drumsImage;
       
    public void PlayAudio(int index)
    {
        source[index].Play();
    }

    public void SwapInstrument(AudioClip[] newInstrument)
    {
        for(int i=0; i<5; i++)
        {
            source[i].clip = newInstrument[i];
        }
    }

    public void ChangeToPiano()
    {
        SwapInstrument(pianoClips);
        ChangeIconOpacity(0.3f, pianoImage);
        ChangeIconOpacity(0, drumsImage);
    }

    public void ChangeToDrums()
    {
        SwapInstrument(drumClips);
        ChangeIconOpacity(0.3f, drumsImage);
        ChangeIconOpacity(0, pianoImage);
    }

    public void ChangeIconOpacity(float newOpacity, Image image)
    {
        // Get the current color of the pianoImage
        Color color = image.color;

        // Set the alpha channel to the new opacity value
        color.a = newOpacity;

        // Apply the new color back to the pianoImage
        image.color = color;
    }



}
