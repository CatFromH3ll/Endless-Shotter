using UnityEngine;

public class ModelSelector : MonoBehaviour
{
    [SerializeField] private GameObject[] boatModels;
    [SerializeField] public Animator PlayerAnimator { get; set; }
    
    void Start()
    {
        // Read the saved boat index.
        // If nothing was saved yet, use 0 as the default boat.
        int selectedBoatIndex = PlayerPrefs.GetInt("SelectedBoatIndex", 0);

        // Safety check so the game does not crash if the index is wrong.
        if (selectedBoatIndex < 0 || selectedBoatIndex >= boatModels.Length)
        {
            selectedBoatIndex = 0;
        }

        // Activate only the selected boat model.
        for (int i = 0; i < boatModels.Length; i++)
        {
            boatModels[i].SetActive(i == selectedBoatIndex);
            PlayerAnimator = GetComponentInChildren<Animator>();
            if (PlayerAnimator == null)
                Debug.LogError("No active child Animator found.");
        }

        Debug.Log("Gameplay boat model loaded: " + selectedBoatIndex);
    }
}
