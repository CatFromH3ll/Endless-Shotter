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
   public AudioClip shieldSound;
   public AudioClip shieldHit;
   public AudioClip shieldDeactivated;
   public AudioClip shieldReady;
   public AudioClip spotlightTurnOn;
   public AudioClip spotlightTurnOff;
   public AudioClip splashSound;
   
   [Header("Engine Settings")]
   [SerializeField] private float normalPitch = 1f;
   [SerializeField] private float throttlePitch = 1.25f;
   [SerializeField] private float pitchChangeSpeed = 2f;

   private float targetEnginePitch;
   private bool spotlightOn = false;



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
      //sfxSource.Play();
   }

   public void Update()
   {
      if(sfxSource.clip == shieldSound) return;
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
      engineSource.Stop();
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
      SwitchEngineSound(playerEngineMotor);
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
   
   private void SwitchEngineSound(AudioClip newClip)
   {
      if (engineSource.clip == newClip) return;

      engineSource.Stop();
      engineSource.clip = newClip;
      engineSource.loop = true;
      engineSource.Play();
   }

   public void RepairSound()
   {
      sfxSource.PlayOneShot(repairSound);
   }

   public void ShieldSound()
   {
      SwitchEngineSound(shieldSound);
   }

   public void ShieldHitSound()
   {
      sfxSource.PlayOneShot(shieldHit);
   }

   public void ShieldDeactivatedSound()
   {
      sfxSource.PlayOneShot(shieldDeactivated);
   }

   public void ShieldReadySound()
   {
      sfxSource.PlayOneShot(shieldReady);
   }

   public void TogglePlayerHeadlights()
   {
      if (spotlightOn)
      {
         sfxSource.PlayOneShot(spotlightTurnOff);
         spotlightOn = false;
      }
      else
      {
         sfxSource.PlayOneShot(spotlightTurnOn);
         spotlightOn = true;
      }
   }

   public void Splash()
   {
      sfxSource.PlayOneShot(splashSound);
   }

}
