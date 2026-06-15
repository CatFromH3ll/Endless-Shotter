
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIControler : MonoBehaviour
{
   [SerializeField] private GameObject mainHUD;
   [SerializeField] private GameObject deathPanel;
   [SerializeField] private GameObject victoryPanel;
   [SerializeField] private GameObject loadingPanel;
   [SerializeField] private Slider loadingSlider;
   public void StartGame()
   {
      loadingPanel.SetActive(true); 
      StartCoroutine(LoadLevelAsync());
      Time.timeScale = 1;
   }
   IEnumerator LoadLevelAsync()
   {
      AsyncOperation op = SceneManager.LoadSceneAsync(1);
      while (!op.isDone)
      {
         float progress = Mathf.Clamp01(op.progress / 0.9f);
            
         if (loadingSlider != null)
            loadingSlider.value = progress;
         yield return null;
      }
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
   }

   public void Finish()
   {
      mainHUD.SetActive(false);
      victoryPanel.SetActive(true);
   }
}
