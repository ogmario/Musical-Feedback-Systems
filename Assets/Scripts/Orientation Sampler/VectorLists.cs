using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Initializes 5 lists in which accelerometers values will be stored. This gameObject won't be destroyed when changing scenes.
 *
 */
public class VectorLists : MonoBehaviour
{
    public int maxListSize = 30; // Maximum size for each list

    public List<Vector3>[] positionLists = new List<Vector3>[5];

    public static VectorLists Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy this instance because another one already exists
            return;
        }
        Instance = this; // Set this as the singleton instance

        // Make the GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);

        // Initialize 5 lists, one for each position
        positionLists = new List<Vector3>[5];
        for (int i = 0; i < positionLists.Length; i++)
        {
            positionLists[i] = new List<Vector3>();
        }       

    }

}
