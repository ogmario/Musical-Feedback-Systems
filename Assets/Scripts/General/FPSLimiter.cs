using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Limits maximum frame rate.
 *
 */
public class FPSLimiter : MonoBehaviour
{
    // Target FPS
    public int targetFPS = 60;

    void Awake()
    {
        // Set the target frame rate
        Application.targetFrameRate = targetFPS;
    }
}