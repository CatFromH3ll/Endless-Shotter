using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    public int slotId; // Set to 1, 2, or 3 in the Inspector

    [Header("UI Elements")]
    [SerializeField] private GameObject orgenizer;
    [SerializeField] private GameObject emptySlotText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private RawImage thumbnailDisplay;

    [SerializeField] private UIControler uiController;

    public void SetupSlot(UIControler controller)
    {
        uiController = controller;
        
        // Grab preview records from the IO System
        SaveData data = IO_System.Instance.GetSlotPreviewData(slotId);

        if (data != null)
        {
            emptySlotText.SetActive(false);
            orgenizer.SetActive(true);

            waveText.text = $"Wave: {data.currentWaveIndex + 1}";
            
            // Format raw seconds into standard minutes and seconds display
            int minutes = Mathf.FloorToInt(data.savedTime / 60);
            int seconds = Mathf.FloorToInt(data.savedTime % 60);
            
            // Appends the survival time cleanly right next to the score
            scoreText.text = $"Score: {data.score}     Time survived ({minutes:00}:{seconds:00})";

            // Fetch and apply the saved screenshot pairing matrix
            Texture2D screenshot = IO_System.Instance.GetSlotScreenshot(slotId);
            if (screenshot != null)
            {
                thumbnailDisplay.texture = screenshot;
                thumbnailDisplay.enabled = true;
            }
            else
            {
                thumbnailDisplay.enabled = false;
            }
        }
        else
        {
            // Empty slot visual state configuration
            emptySlotText.SetActive(true);
            orgenizer.SetActive(false);
            thumbnailDisplay.enabled = false;
        }
    }

    // Triggered by the Button's OnClick event
    public void OnSlotSelected()
    {
        uiController.HandleSlotChoice(slotId);
    }
}