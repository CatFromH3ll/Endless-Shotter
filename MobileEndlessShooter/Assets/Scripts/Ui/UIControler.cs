
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControler : MonoBehaviour
{
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
}
