using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
/*
 * Author:Mario Ortega García
 * Date: August 2024
 * Description: Finds all the scenes present in the project and adds them to the dropdown menu, allowing for navigation between scenes.
 *
 */
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private TMP_Dropdown sceneDropdown;    

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
    }

    private void Start()
    {
        // Make the GameObject persistent across scenes
        DontDestroyOnLoad(gameObject);

        // Register to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Check if TMP_Dropdown is present in the initial scene
        FindDropdownInScene();
    }

    private void OnDestroy()
    {
        // Unregister from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void FindDropdownInScene()
    {
        sceneDropdown = FindObjectOfType<TMP_Dropdown>();

        if (sceneDropdown != null)
        {
            // Populate the dropdown and set initial value
            PopulateDropdown();
            SetInitialDropdownValue();

            // Add listener to handle scene switching
            sceneDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDropdownInScene();        
    }
   
    void PopulateDropdown()
    {
        sceneDropdown.ClearOptions();

        // Get all scene names and add them to the dropdown options
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            sceneDropdown.options.Add(new TMP_Dropdown.OptionData(sceneName));
        }

        // Refresh dropdown
        sceneDropdown.RefreshShownValue();
    }

    void SetInitialDropdownValue()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        int index = sceneDropdown.options.FindIndex(option => option.text == currentSceneName);

        // Set dropdown value only if index is valid
        if (index != -1)
            sceneDropdown.value = index;
        else
            Debug.LogError("Current scene not found in dropdown options.");
    }

    void OnDropdownValueChanged(int index)
    {
        string selectedSceneName = sceneDropdown.options[index].text;
        SceneManager.LoadScene(selectedSceneName);
    }
}

