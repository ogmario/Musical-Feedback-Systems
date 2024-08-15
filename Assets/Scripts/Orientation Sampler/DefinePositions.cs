using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Upon pressing a button, a countdown will start, and then accelerometer values will be stored in a list.
 *
 */
public class DefinePositions : MonoBehaviour
{
    private VectorLists vectorLists;

    public TextMeshProUGUI[] listText = new TextMeshProUGUI[5];

    private int countdownTime = 3;

    public Button[] button = new Button[5];
 

    // Start is called before the first frame update
    void Start()
    {
        vectorLists = FindObjectOfType<VectorLists>(); //Find VectorLists gameObject
        if (vectorLists == null)
        {
            Debug.LogError("VectorLists not found in the scene.");
            // Handle error condition (e.g., display a message to the user)
        }
    }
    
    // Store accelerometer values in a list
    private void StoreInList(int index)
    {
        if (vectorLists.positionLists[index] != null)
        {
            while (vectorLists.positionLists[index].Count < vectorLists.maxListSize)
            {
                vectorLists.positionLists[index].Add(Input.acceleration);
                listText[index].text = "Storing position";
            }           
        }
        listText[index].text = "Position stored";
    }

    //Count down from 3, then call StoreInList
    IEnumerator Countdown(int index)
    {
        countdownTime = 3;
        while (countdownTime > 0)
        {
            listText[index].text = countdownTime.ToString();
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        StoreInList(index);
    }

    // When  pressing a certain button, clear its corresponding list, start the countdown and activate the next button
    public void OnButtonPress(int index)
    {
        vectorLists.positionLists[index].Clear();
        StartCoroutine(Countdown(index));

        if(index<4)
        {
            button[index + 1].interactable = true;
        }       
    }



}


