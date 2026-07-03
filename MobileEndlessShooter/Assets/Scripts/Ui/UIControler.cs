
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
   [SerializeField] private GameObject mainHUD;
   [SerializeField] private GameObject selectDifficultyPanel;
   [SerializeField] private GameObject deathPanel;
   [SerializeField] private GameObject victoryPanel;
   [SerializeField] private GameObject loadingPanel;
   [SerializeField] private GameObject boatSelectionPanel;
   [SerializeField] private Slider loadingSlider;
   [SerializeField] private float GameWidth;
   private float easyDifficulty =  1.0f;
   private float mediumDifficulty = 1.5f;
   private float hardDifficulty = 2.0f;
   public static float selectedDifficultyModifier {get; private set;}
   
   public void StartGame()
   {
      loadingPanel.SetActive(true); 
      StartCoroutine(LoadLevelAsync());
      ;
      Time.timeScale = 1;
   }

   public void enterSelectedDifficultyPanel()
   {
      selectedDifficultyModifier = easyDifficulty;
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
   

   private void Update()
   {
      GameWidth = Screen.width;
   }

   public void QuitGame()
   {
      #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
      #else
      Application.Quit();
      #endif
   }

   public void RestartGame()
   {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      AudioManager.instance.GameMusic();
      AudioManager.instance.PlayerEngineMotor();
      Time.timeScale = 1;
   }

   public void PauseGame()
   {
      Time.timeScale = 0;
   }

   public void ResumeGame()
   {
      Time.timeScale = 1;
   }

   public void LoadMenu()
   {
      SceneManager.LoadScene(0);
   }

   public void Death()
   {
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
      switch (difficulty)
      {
         case 0:
         {
            selectedDifficultyModifier = easyDifficulty;
            
            break;
         }
         case 1:
         {
            selectedDifficultyModifier = mediumDifficulty;
            break;
         }
         case 2:
         {
            selectedDifficultyModifier = hardDifficulty;
            break;
         }
      }
   }
}
