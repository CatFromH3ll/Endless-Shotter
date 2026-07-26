using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineManager : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector director;
    [SerializeField] private TimelineAsset executionTimeline;
    [SerializeField] private SignalReceiver signalReceiver;

    [Header("Camera")]
    [SerializeField] private FollowProjectile followprojectileCamera;

    [Header("Player Control Scripts")]
    [SerializeField] private GameObject gameUI;

    [Header("Slow Motion")]
    [SerializeField, Range(0.01f, 1f)]
    private float slowMotionScale = 0.15f;

    [Header("Timeline Track Names")]
    [SerializeField] private string bulletTrackName = "Bullet Track";
    [SerializeField] private string signalTrackName = "Signal Track";

    private GameObject currentBullet;
    private Transform currentEnemy;

    private float previousTimeScale;
    private float previousFixedDeltaTime;

    private bool executionPlaying;
    private bool slowMotionActive;

    public bool ExecutionPlaying => executionPlaying;

    private void Awake()
    {
        // Timeline continues normally while gameplay is slowed.
        director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;

        // Safety cleanup when the Timeline stops.
        director.stopped += OnTimelineStopped;
    }

    private void OnDestroy()
    {
        director.stopped -= OnTimelineStopped;
    }

    public bool StartExecution(GameObject bullet, Transform enemy)
    {
        if (executionPlaying)
            return false;

        if (bullet == null || enemy == null)
        {
            Debug.LogWarning(
                "Execution requires a bullet and an enemy.");

            return false;
        }
        
        TrackAsset bulletTrack = FindTrack(bulletTrackName);
        TrackAsset signalTrack = FindTrack(signalTrackName);

        if (bulletTrack == null ||
            signalTrack == null)
        {
            return false;
        }

        currentBullet = bullet;
        currentEnemy = enemy;

        director.playableAsset = executionTimeline;
        followprojectileCamera.UpdateProjectile(bullet);
        // Make the execution camera follow this specific bullet.
        
        

        // Required dynamic Timeline bindings.
      

        director.SetGenericBinding(
            signalTrack,
            signalReceiver);

        executionPlaying = true;

        director.time = 0;
        director.Play();

        return true;
    }

    private TrackAsset FindTrack(string trackName)
    {
        TrackAsset track = executionTimeline
            .GetOutputTracks()
            .FirstOrDefault(
                timelineTrack =>
                    timelineTrack.name == trackName);

        if (track == null)
        {
            Debug.LogError(
                $"Timeline track '{trackName}' was not found.");
        }

        return track;
    }

    // Connect this to the Begin Execution Signal.
    public void BeginExecution()
    {
        if (slowMotionActive)
            return;

        slowMotionActive = true;

        previousTimeScale = Time.timeScale;
        previousFixedDeltaTime = Time.fixedDeltaTime;

        SetPlayerControls(false);

        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime =
            previousFixedDeltaTime * slowMotionScale;
    }

    // Connect this to the Impact Signal.
    public void ExecutionImpact()
    {
        /*
         * Add cinematic impact effects here later:
         *
         * - Explosion particles
         * - Impact sound
         * - Camera shake
         * - Enemy death effect*/
         
    }

    // Connect this to the End Execution Signal.
    public void EndExecution()
    {
        FinishExecution();

        if (director.state == PlayState.Playing)
        {
            director.Stop();
        }
    }

    private void OnTimelineStopped(
        PlayableDirector stoppedDirector)
    {
        FinishExecution();
    }

    private void FinishExecution()
    {
        if (!executionPlaying && !slowMotionActive)
            return;

        RestoreGameplay();

       
        currentBullet = null;
        currentEnemy = null;

        executionPlaying = false;
    }

    private void RestoreGameplay()
    {
        followprojectileCamera.EndOfExecution();
        
        if (!slowMotionActive)
            return;

        Time.timeScale = previousTimeScale;
        Time.fixedDeltaTime = previousFixedDeltaTime;

        SetPlayerControls(true);

        slowMotionActive = false;
    }

    private void SetPlayerControls(bool enabled)
    {
        
        if(gameUI != null) gameUI.SetActive(enabled);
        
    }
    public void ForceStopExecution()
    {
        if (executionPlaying)
        {
            // 1. Instantly restore normal time
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // Unity's default fixed time

            // 2. Stop the timeline from running in the background
            if (director != null && director.state == UnityEngine.Playables.PlayState.Playing)
            {
                director.Stop();
            }

            // 3. Clear the camera targets so it doesn't look at a destroyed object
            
            executionPlaying = false;
        }
    }
}
