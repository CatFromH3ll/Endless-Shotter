using UnityEngine;

public class MusicManager : MonoBehaviour
{
   public static MusicManager instance;
   public AudioSource menuMusicSource;
   public AudioClip menuMusicClip;

   public void Awake()
   {
      menuMusicSource.Play();
   }
   
}
