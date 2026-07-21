
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
   [Header("Panels")]
   [SerializeField] private GameObject startPanel;       // The panel with the "Play" button
   [SerializeField] private GameObject saveSlotsPanel;   // The new panel holding the 3 slots
   [SerializeField] private GameObject difficultyPanel;  // The original difficulty selection screen

   [Header("Slots Setup")]
   [SerializeField] private SaveSlotUI[] saveSlots;
   
   private int currentlySelectedSlot;
   [SerializeField] private IO_System ioSystem;
   [SerializeField] private AudioMixer audioMixer;
   [SerializeField] private GameObject mainHUD;
   [SerializeField] private GameObject selectDifficultyPanel;
   [SerializeField] private GameObject deathPanel;
   [SerializeField] private GameObject victoryPanel;
   [SerializeField] private GameObject loadingPanel;
   [SerializeField] private GameObject boatSelectionPanel;
   [SerializeField] private Slider loadingSlider;
   [SerializeField] private Slider musicSlider;
   [SerializeField] private Slider sfxSlider;
   [SerializeField] private float GameWidth;
   [SerializeField] private GameOverAnalytics gameOverAnalytics;
   private float easyDifficultyMult =  1.0f;
   private float mediumDifficultyMult = 1.5f;
   private float hardDifficultyMult = 2.0f;
   public float musicVolume;
   public float sfxVolume;
   public static float selectedDifficultyModifier {get; private set;}

   private const string audioMixerHash = "AudioMixer";
   private const string musicVolumeHash = "MusicVolume";
   private const string sfxVolumeHash = "SFXVolume";
   
   public void Start()
   {
      musicVolume = PlayerPrefs.GetFloat(musicVolumeHash);
      sfxVolume = PlayerPrefs.GetFloat(sfxVolumeHash);
      audioMixer.SetFloat(musicVolumeHash, musicVolume);
      audioMixer.SetFloat(sfxVolumeHash,  sfxVolume);
      
      // FIXED: Assigned music and SFX volumes to their correct sliders instead of loadingSlider
      if (musicSlider != null)
         musicSlider.value = musicVolume;
      if (sfxSlider != null)
         sfxSlider.value = sfxVolume;
   }
   
   public void SlotPanelActivation()
   {
      startPanel.SetActive(false);
      saveSlotsPanel.SetActive(true);

      // Initialize all slot components matching their current text files
      foreach (var slot in saveSlots)
      {
         slot.SetupSlot(this);
      }
   }
   
   public void StartGame()
   {
      loadingPanel.SetActive(true); 
      StartCoroutine(LoadLevelAsync());
      Time.timeScale = 1;
      
      audioMixer.GetFloat(musicVolumeHash, out float musicVol);
      audioMixer.GetFloat(sfxVolumeHash, out float sfxVol);
      PlayerPrefs.SetFloat(musicVolumeHash, musicVol);
      PlayerPrefs.SetFloat(sfxVolumeHash, sfxVol);
      
      
   }
   
   public void HandleSlotChoice(int slotId)
   {
      currentlySelectedSlot = slotId;
        
      // Keep track of the active slot for the entire play session so we know where to save later!
      PlayerPrefs.SetInt("ActiveSaveSlot", slotId);

      // Check if data already exists in this slot
      SaveData existingData = IO_System.Instance.GetSlotPreviewData(slotId);

      if (existingData != null)
      {
         Debug.Log($"Save exists. Storing slot ID {slotId} and setting load flag...");
            
         // Tell the next scene: YES, load data from the file on boot
         PlayerPrefs.SetInt("ShouldLoadSaveData", 1);
         PlayerPrefs.Save();
            
         StartGame(); 
      }
      else
      {
         Debug.Log($"Creating a brand new game on slot ID {slotId}...");
         
         // Tell the next scene: NO, do not load data (start a fresh game)
         PlayerPrefs.SetInt("ShouldLoadSaveData", 0);
         PlayerPrefs.Save();

         // Proceed to difficulty selection panel
         saveSlotsPanel.SetActive(false);
         difficultyPanel.SetActive(true);
      }
   }
   
   public void OnDifficultySelected(int chosenDifficultyIndex)
   {
      // FIXED: Route this through your existing DifficultySelection method 
      // so your selectedDifficultyModifier multiplier rules are set up properly!
      DifficultySelection(chosenDifficultyIndex);
        
      StartGame();
   }
   
   IEnumerator LoadLevelAsync()
   {
      AsyncOperation op = SceneManager.LoadSceneAsync(1);
      while (!op.isDone)
      {
         float progress = Mathf.Clamp01(op.progress / 0.9f);
            
         if (loadingSlider != null)
            loadingSlider.value = progress;
         AudioManager.instance.GameMusic();
         AudioManager.instance.PlayerEngineMotor();
         yield return null;
      }

      
   }

   public void MusicVolume(float volume)
   {
      audioMixer.SetFloat(musicVolumeHash, volume);
   }
   
   public void SfxVolume(float volume)
   {
      audioMixer.SetFloat(sfxVolumeHash, volume);
   }
   
   private void Update()
   {
      GameWidth = Screen.width;
   }

   public void QuitGame()
   {
      gameOverAnalytics.GameOverAnalyse();
      audioMixer.GetFloat(musicVolumeHash, out float musicVol);
      audioMixer.GetFloat(sfxVolumeHash, out float sfxVol);
      PlayerPrefs.SetFloat(musicVolumeHash, musicVol);
      PlayerPrefs.SetFloat(sfxVolumeHash, sfxVol);
      
      
      
      #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
      #else
      Application.Quit();
      #endif
   }

   public void RestartGame()
   {
      gameOverAnalytics.GameOverAnalyse();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      AudioManager.instance.GameMusic();
      AudioManager.instance.PlayerEngineMotor();
      Time.timeScale = 1;
   }

   public void PauseGame()
   {
      AudioManager.instance.engineSource.Stop();
      Time.timeScale = 0;
   }

   public void ResumeGame()
   {
      AudioManager.instance.engineSource.Play();
      Time.timeScale = 1;
   }

   public void MainMenu()
   {
      gameOverAnalytics.GameOverAnalyse();
      SceneManager.LoadScene(0);
      AudioManager.instance.PlayMenuMusic();
   }

   public void Death()
   {
      gameOverAnalytics.GameOverAnalyse();
      mainHUD.SetActive(false);
      deathPanel.SetActive(true);
      AudioManager.instance.GameOver();
   }

   public void Finish()
   {
      mainHUD.SetActive(false);
      victoryPanel.SetActive(true);
   }

   public void DifficultySelection(int difficulty)
   {
      ioSystem.SetBaseDifficulty(difficulty);
      switch (difficulty)
      {
         case 0:
         {
            selectedDifficultyModifier = easyDifficultyMult;
            
            break;
         }
         case 1:
         {
            selectedDifficultyModifier = mediumDifficultyMult;
            break;
         }
         case 2:
         {
            selectedDifficultyModifier = hardDifficultyMult;
            break;
         }
      }  
   }
}
