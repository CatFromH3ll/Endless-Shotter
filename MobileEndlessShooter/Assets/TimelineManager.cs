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

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera executionCamera;
    [SerializeField] private CinemachineBrain mainCameraBrain;

    [Header("Player Control Scripts")]
    //[SerializeField] private MonoBehaviour[] playerControlScripts;
    [SerializeField] private GameObject gameUI;

    [Header("Slow Motion")]
    [SerializeField, Range(0.01f, 1f)]
    private float slowMotionScale = 0.15f;

    [Header("Timeline Track Names")]
    [SerializeField] private string bulletTrackName = "Bullet Track";
    [SerializeField] private string cameraTrackName = "Camera Track";
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

        Animator bulletAnimator =
            bullet.GetComponentInChildren<Animator>();

        if (bulletAnimator == null)
        {
            Debug.LogError(
                "The bullet requires an Animator for Timeline binding.");

            return false;
        }

        TrackAsset bulletTrack = FindTrack(bulletTrackName);
        TrackAsset cameraTrack = FindTrack(cameraTrackName);
        TrackAsset signalTrack = FindTrack(signalTrackName);

        if (bulletTrack == null ||
            cameraTrack == null ||
            signalTrack == null)
        {
            return false;
        }

        currentBullet = bullet;
        currentEnemy = enemy;

        director.playableAsset = executionTimeline;

        // Make the execution camera follow this specific bullet.
        executionCamera.Follow = bullet.transform;
        executionCamera.LookAt = enemy;

        // Required dynamic Timeline bindings.
        director.SetGenericBinding(
            bulletTrack,
            bulletAnimator);

        director.SetGenericBinding(
            cameraTrack,
            mainCameraBrain);

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
         * - Enemy death effect
         */
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

        executionCamera.Follow = null;
        executionCamera.LookAt = null;

        currentBullet = null;
        currentEnemy = null;

        executionPlaying = false;
    }

    private void RestoreGameplay()
    {
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
        
        /*foreach (MonoBehaviour controlScript
                 in playerControlScripts)
        {
            if (controlScript != null)
            {
                controlScript.enabled = enabled;
            }
        }*/
    }
}
