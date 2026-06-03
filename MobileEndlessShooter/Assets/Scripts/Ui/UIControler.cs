
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControler : MonoBehaviour
{
   [SerializeField] private GameObject mainHUD;
   [SerializeField] private GameObject deathPanel;
   [SerializeField] private GameObject victoryPanel;
   public void StartGame()
   {
      SceneManager.LoadScene(1);
      Time.timeScale = 1;
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
