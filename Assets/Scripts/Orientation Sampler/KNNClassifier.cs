using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Trains a KNN algorithm with the variables stored in a VectorLists gameObject, then classifies the accelerometer current values into one of those positions.
 *
 */
public class KNNClassifier : MonoBehaviour
{
    // List to hold training data
    List<DataPoint> trainingData = new List<DataPoint>();
    private VectorLists vectorLists;
    public int k=5;
    public bool classify=true;
    private int currentGroup=1;
    private int previousGroup=1;
    private int positions;
    public TextMeshProUGUI groupText;
    public Image panel;

    public TextMeshProUGUI xAccel;
    public TextMeshProUGUI yAccel;
    public TextMeshProUGUI zAccel;
    public KNNSampler sampler;

    public static Color[] colors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.green,     
        Color.yellow,
        new Color(0.6f, 0.0f, 1.0f), //Violet
    };

    // Start is called before the first frame update
    void Start()
    {
        vectorLists = FindObjectOfType<VectorLists>();
        // Get the total number of classification groups (positions)
        positions = GetAmountOfPositions(vectorLists.positionLists, vectorLists.maxListSize);
        // Train algorithm upon starting
        Train(vectorLists.positionLists, positions);
    }

    private void Update()
    {
        // Every frame, classify current accelerometer values into one of the groups
        if (classify)
        {         
            Vector3 accel = Input.acceleration;
            currentGroup = Classify(accel);
            if (currentGroup != previousGroup)
            {
                //Updayte UI
                groupText.text = "Position " + currentGroup.ToString();
                // Groups start at 1, arrays at 0, so substract 1
                int index = currentGroup - 1;
                // Change Background color
                panel.color = colors[index]; 
                // Play corresponding audio sample
                sampler.PlayAudio(index);                
            }
            previousGroup = currentGroup;
            UpdateUI(accel);
        }
    }

    private int GetAmountOfPositions(List<Vector3>[] dataArray, int maxListSize)
    {
        int count = 0;

        // Loop through each list in the array
        for (int i = 0; i < dataArray.Length; i++)
        {
            // Check if the current list exists and its item count is equal to maxListSize
            if (dataArray[i] != null && dataArray[i].Count == maxListSize)
            {
                count++; // Increment the count of lists meeting the condition
            }
        }

        return count; // Return the number of lists to use
    }

    // Define a class to represent a data point
    public class DataPoint
    {
        public Vector3 coordinates;
        public int group;

        public DataPoint(Vector3 coords, int group)
        {
            this.coordinates = coords;
            this.group = group;
        }
    }

    public void Train(List<Vector3>[] dataListArray, int numLists)
    {
        // Validate input to ensure numberOfListsToUse is within valid range
        if (numLists < 0 || numLists > dataListArray.Length)
        {
            Debug.LogError("Invalid numberOfListsToUse provided.");
            return;
        }

        // Clear the existing training data before populating it with new data
        trainingData.Clear();

        // Loop through each list up to numberOfListsToUse and process the data
        for (int i = 0; i < numLists; i++)
        {
            List<Vector3> dataList = dataListArray[i];
            int groupIndex = i + 1; // Group index starts from 1

            // Add data points from the current list with the corresponding group index
            foreach (Vector3 coords in dataList)
            {
                trainingData.Add(new DataPoint(coords, groupIndex));
            }
        }
    }

    public int Classify(Vector3 newDataPoint)
    {
        if (trainingData.Count == 0)
        {
            Debug.LogWarning("No training data available.");
            return 0; // Return default group or handle gracefully
        }

        // Create a list to store distances between the new data point and training data points
        List<KeyValuePair<float, int>> distances = new List<KeyValuePair<float, int>>();
        
        // Calculate distances between the new data point and each training data point
        foreach (DataPoint dataPoint in trainingData)
        {
            float distance = Vector3.Distance(newDataPoint, dataPoint.coordinates);
            distances.Add(new KeyValuePair<float, int>(distance, dataPoint.group));
        }

        // Sort distances in ascending order
        distances.Sort((x, y) => x.Key.CompareTo(y.Key));

        // Count occurrences of each group among the k nearest neighbors
        Dictionary<int, int> counts = new Dictionary<int, int>();
        for (int i = 0; i < k; i++)
        {
            int group = distances[i].Value;
            if (!counts.ContainsKey(group))
            {
                counts[group] = 0;
            }
            counts[group]++;
        }

        // Find the group with the maximum count
        int maxCount = 0;
        int predictedGroup = 0;
        foreach (KeyValuePair<int, int> pair in counts)
        {
            if (pair.Value > maxCount)
            {
                maxCount = pair.Value;
                predictedGroup = pair.Key;
            }
        }

        return predictedGroup;
    }

    //Updates accelerometer readings in the UI
    public void UpdateUI(Vector3 vector)
    {
        xAccel.text = "X: " + vector.x;
        yAccel.text = "Y: " + vector.y;
        zAccel.text = "Z: " + vector.z;
    }
}
