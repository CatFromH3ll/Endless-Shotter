using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager instance;
   [Header("Sources")] 
   public AudioSource musicSource;
   public AudioSource sfxSource;
   public AudioSource engineSource;

   [Header("Clips")] 
   public AudioClip menuMusicClip;
   public AudioClip gameMusicClip;
   public AudioClip gameOverClip;
   public AudioClip playerEngineMotor;
   public AudioClip playerDeath;
   public AudioClip playerHit;
   public AudioClip basicSpawn;
   public AudioClip playerProjectileHit;
   public AudioClip playerJump;
   public AudioClip collectCoin;
   public AudioClip repairSound;
   
   [Header("Engine Settings")]
   [SerializeField] private float normalPitch = 1f;
   [SerializeField] private float throttlePitch = 1.25f;
   [SerializeField] private float pitchChangeSpeed = 2f;

   private float targetEnginePitch;



   public void Awake()
   {
      if (instance == null)
      {
         instance = this;
         DontDestroyOnLoad(gameObject);
      }
   }

   public void Start()
   {
      PlayMenuMusic();
      sfxSource.Play();
   }

   public void Update()
   {
      //constantly smoothly transition the pitch of the engine
      engineSource.pitch = Mathf.MoveTowards(
         engineSource.pitch,
         targetEnginePitch,
         pitchChangeSpeed * Time.deltaTime
      );
   }

   public void PlayMenuMusic()
   {
      musicSource.clip = menuMusicClip;
      musicSource.loop = true;
      musicSource.Play();
   }

   public void GameMusic()
   {
      musicSource.clip = gameMusicClip;
      musicSource.loop = true;
      musicSource.Play();
   }

   public void GameOver()
   {
      musicSource.clip = gameOverClip;
      musicSource.loop = true;
      musicSource.Play();
   }

   public void PlayerJump()
   {
      sfxSource.PlayOneShot(playerJump);
   }

   public void PlayerEngineMotor()
   {
      engineSource.clip = playerEngineMotor;
      engineSource.pitch = normalPitch;
      engineSource.loop = true;
      engineSource.Play();
      
      targetEnginePitch = normalPitch;
   }
   
   public void SetEngineThrottle(float horizontalInput)
   {
      // Converts both left and right input into a positive 0–1 value,Based on joystick input
      float throttleAmount = Mathf.Clamp01(
         Mathf.Abs(horizontalInput) //if the value is -1, it will count as 1 regardless
      );
      
      targetEnginePitch = Mathf.Lerp(
         normalPitch,
         throttlePitch,
         throttleAmount
      );
   }

   public void PlayerDeath()
   {
      engineSource.Stop();
      sfxSource.PlayOneShot(playerDeath);
   }

   public void PlayerHit()
   {
      sfxSource.PlayOneShot(playerHit);
   }

   public void PlayerBasicShotSound()
   {
      sfxSource.PlayOneShot(basicSpawn);
   }

   public void PlayerProjectileHitSound()
   {
      sfxSource.PlayOneShot(playerProjectileHit);
   }

   public void CollectCoinSound()
   {
      sfxSource.PlayOneShot(collectCoin);
   }

   public void RepairSound()
   {
      sfxSource.PlayOneShot(repairSound);
   }

}
