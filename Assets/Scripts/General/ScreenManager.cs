using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Keeps the screen permanently awake.
 *
 */
public class ScreenManager : MonoBehaviour
{
    void Start()
    {
        // Keeps the screen awake
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }
}

