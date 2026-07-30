using System.IO;
using System.Collections;
using UnityEngine;

public class IO_System : MonoBehaviour
{
    public static IO_System Instance;

    [Header("Scene Component References")] [SerializeField]
    private PlayerMovement playerMovement;

    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private Score scoreSystem;
    
    [Header("Screenshot Settings")]
    [SerializeField] private GameObject pauseMenuPanel;

    // Used to remember what base difficulty setting the session is playing on
    private int activeSelectedDifficultyIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Only run this if we are in the gameplay scene (verified by checking if components are assigned)
        if (playerHealth != null || playerMovement != null)
        {
            // Read our new temporary flag instead of clearing out the slot ID
            int shouldLoad = PlayerPrefs.GetInt("ShouldLoadSaveData", 0);
            int activeSlot = PlayerPrefs.GetInt("ActiveSaveSlot", 1);

            if (shouldLoad == 1)
            {
                Debug.Log($"Gameplay Scene started! Active slot {activeSlot} detected. Restoring session state...");

                LoadGameSlot(activeSlot);

                // Clear ONLY the temporary load flag so normal restarts don't loop-load data.
                // This keeps "ActiveSaveSlot" safe so you can overwrite it when saving!
                PlayerPrefs.SetInt("ShouldLoadSaveData", 0);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log($"Gameplay Scene started! Fresh game session initialized on slot {activeSlot}.");
            }
        }
    }

    
    public void SaveCurrentSession()
    {
        int currentSlot = PlayerPrefs.GetInt("ActiveSaveSlot", 1);
        Debug.Log($"Auto-saving current game session into slot: {currentSlot}");
        SaveGameSlot(currentSlot);
    }
    
    public void SetBaseDifficulty(int difficultyIndex)
    {
        activeSelectedDifficultyIndex = difficultyIndex;
    }

    public void SaveGameSlot(int slotId)
    {
        SaveData freshData = TakeDataFromGame();

        // Serialize data structure to JSON format string
        string json = JsonUtility.ToJson(freshData, true);
        string savePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");
        Debug.Log(savePath);

        // Execute file I/O operations
        File.WriteAllText(savePath, json);
        Debug.Log($"Game data written to target destination: {savePath}");

        // Handle runtime pause menu visual frame capture paired to this file slot
        StartCoroutine(CaptureScreenshotRoutine(slotId));
    }

    // ==========================================
    // BACKEND SAVE DATA GATHERING (JSON)
    // ==========================================

    public SaveData TakeDataFromGame()
    {
        SaveData data = new SaveData();

        // 1. Gather Player State from actual scripts
        if (playerHealth != null)
        {
            data.currentHealth = playerHealth.CurrentHealth;
            data.maxHealth = playerHealth.MaxHealth;
        }

        if (playerMovement != null)
        {
            data.position = playerMovement.transform.position;
        }

        // Read the currently active visual boat index tracking key
        data.boatModelIndex = PlayerPrefs.GetInt("SelectedBoatIndex", 0);

        if (scoreSystem != null)
        {
            data.score = scoreSystem.score;
        }

        // 2. Gather World State from EnemySpawner script fields
        if (enemySpawner != null)
        {
            data.currentWaveIndex = enemySpawner.currentDifficultyIndex;
            data.stageMult = enemySpawner.stageMult;
        }
        
        // 3. Gather Time from your GameTimer script
        if (gameTimer != null)
        {
            data.savedTime = gameTimer.CurrentTime; // <-- ADD THIS LINE
        }

        data.selectedDifficulty = activeSelectedDifficultyIndex;
        
        return data;
    }

    // ==========================================
    // BACKEND DATA LOADING & RESTORATION
    // ==========================================
    public void LoadGameSlot(int slotId)
    {
        string loadPath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");

        if (!File.Exists(loadPath))
        {
            Debug.LogWarning($"Target save path file slot {slotId} empty!");
            return;
        }

        // Read and restore raw string text
        string json = File.ReadAllText(loadPath);
        SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

        // Transition values back directly into live components
        RestoreDataToActiveSession(loadedData);
    }

    private void RestoreDataToActiveSession(SaveData data)
    {
        // 1. Restore Player Setup
        if (playerHealth != null)
        {
            playerHealth.MaxHealth = data.maxHealth;
            playerHealth.CurrentHealth = data.currentHealth;
        }

        if (playerMovement != null)
        {
            // Set exact coordinates where they left off
            playerMovement.transform.position = data.position;
        }

        // Update selected tracking visuals key for model selectors on refresh 
        PlayerPrefs.SetInt("SelectedBoatIndex", data.boatModelIndex);
        PlayerPrefs.Save();

        // 2. Restore World State Matrix settings back to Spawner
        activeSelectedDifficultyIndex = data.selectedDifficulty;
        if (enemySpawner != null)
        {
            // Pass wave, stage configurations back directly to live values
            enemySpawner.currentDifficultyIndex = data.currentWaveIndex;
            enemySpawner.stageMult = data.stageMult;
        }

        if (scoreSystem != null)
        {
            // Push the saved score back into your Score script
            scoreSystem.UpdateScore(data.score);
        }
        // 4. Restore Timer progress back to your component
        if (gameTimer != null)
        {
            gameTimer.CurrentTime = data.savedTime; // <-- ADD THIS LINE
        }
        Debug.Log("Exact game state session records successfully restored!");
    }

    // ==========================================
    // RUNTIME SCREENSHOT ENGINE (PAIRING ENGINE)
    // ==========================================
    private IEnumerator CaptureScreenshotRoutine(int slotId)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Must wait until frame completely finishes processing or file corrupts/errors out
        yield return new WaitForEndOfFrame();
        // Must wait until frame completely finishes processing or file corrupts/errors out
        

        Texture2D frameCapture = ScreenCapture.CaptureScreenshotAsTexture();
        byte[] imgBinaryBytes = frameCapture.EncodeToPNG();

        string targetImgPath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}_img.png");
        File.WriteAllBytes(targetImgPath, imgBinaryBytes);

        // Manually flush resource from garbage data stack layer
        Destroy(frameCapture);
        Debug.Log($"Paired snapshot linked to slot target directory location: {targetImgPath}");
    }

    public SaveData GetSlotPreviewData(int slotId)
    {
        string loadPath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}.json");
        if (!File.Exists(loadPath)) return null;

        string json = File.ReadAllText(loadPath);
        return JsonUtility.FromJson<SaveData>(json);
    }

// Add this helper method to load the thumbnail texture
    public Texture2D GetSlotScreenshot(int slotId)
    {
        string imagePath = Path.Combine(Application.persistentDataPath, $"save_slot_{slotId}_img.png");
        if (!File.Exists(imagePath)) return null;

        byte[] bytes = File.ReadAllBytes(imagePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes); // Auto-resizes the texture dimensions matching the binary image
        return texture;
    }
    
}