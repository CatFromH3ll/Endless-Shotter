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
    [SerializeField] private FollowPlayer mainCameraFollowPlayer;
    private Vector3 previousCameraPosition;
    private Quaternion previousCameraRotation;

    [Header("Player Control")]
    [SerializeField] private GameObject gameUI;

    [Header("Slow Motion")]
    [SerializeField, Range(0.01f, 1f)]
    private float slowMotionScale = 0.05f;

    [Header("Timeline Track Names")]
    [SerializeField] private string bulletTrackName = "Bullet Track";
    [SerializeField] private string cameraTrackName = "Camera Track";
    [SerializeField] private string signalTrackName = "Signal Track";

    private GameObject currentBullet;
    private Transform currentEnemy;
    

    private TrackAsset bulletTrack;
    private TrackAsset cameraTrack;
    private TrackAsset signalTrack;

    private float previousTimeScale;
    private float previousFixedDeltaTime;
    
    private bool slowMotionActive;

    public float SlowMotionScale => slowMotionScale;
    public bool ExecutionPlaying { get; private set; }

    private void Awake()
    {
        if (director == null)
        {
            Debug.LogError("PlayableDirector is not assigned.", this);
            enabled = false;
            return;
        }

        director.timeUpdateMode =
            DirectorUpdateMode.UnscaledGameTime;

        director.stopped += OnTimelineStopped;

        // Normal camera controls the game initially.
        if (mainCameraBrain != null)
            mainCameraBrain.enabled = false;

        if (executionCamera != null)
            executionCamera.enabled = false;

        if (mainCameraFollowPlayer != null)
            mainCameraFollowPlayer.enabled = true;
    }

    private void OnDestroy()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineStopped;
        }
    }

    public bool StartExecution(
        GameObject bullet,
        Transform enemy)
    {
        if (ExecutionPlaying)
            return false;
        

        if (bullet == null || enemy == null)
        {
            Debug.LogWarning(
                "Execution requires a bullet and an enemy.",
                this);

            return false;
        }

        bulletTrack = FindTrack(bulletTrackName);
        cameraTrack = FindTrack(cameraTrackName);
        signalTrack = FindTrack(signalTrackName);

        if (bulletTrack == null ||
            cameraTrack == null ||
            signalTrack == null)
        {
            return false;
        }

        currentBullet = bullet;
        currentEnemy = enemy;

        director.playableAsset = executionTimeline;

        // Give the execution camera its runtime targets.
        executionCamera.Follow = currentBullet.transform;
        executionCamera.LookAt = currentEnemy;

        // Dynamically bind Timeline tracks.
        director.SetGenericBinding(
            bulletTrack,
            currentBullet);

        director.SetGenericBinding(
            cameraTrack,
            mainCameraBrain);

        director.SetGenericBinding(
            signalTrack,
            signalReceiver);
        
        previousCameraPosition =
            mainCameraBrain.transform.position;

        previousCameraRotation =
            mainCameraBrain.transform.rotation;

        // Stop normal camera script from moving the Main Camera.
        mainCameraFollowPlayer.enabled = false;

        // Enable Cinemachine control.
        executionCamera.enabled = true;
        mainCameraBrain.enabled = true;

        ExecutionPlaying = true;

        director.time = 0;
        ExecutionPlaying = true;
        director.Play();

        return true;
    }

    private bool ValidateReferences()
    {
        if (director == null)
        {
            Debug.LogError(
                "PlayableDirector is not assigned.",
                this);

            return false;
        }

        if (executionTimeline == null)
        {
            Debug.LogError(
                "Execution Timeline is not assigned.",
                this);

            return false;
        }

        if (signalReceiver == null)
        {
            Debug.LogError(
                "Signal Receiver is not assigned.",
                this);

            return false;
        }

        if (executionCamera == null)
        {
            Debug.LogError(
                "Execution Cinemachine Camera is not assigned.",
                this);

            return false;
        }

        if (mainCameraBrain == null)
        {
            Debug.LogError(
                "Main Camera Cinemachine Brain is not assigned.",
                this);

            return false;
        }

        if (mainCameraFollowPlayer == null)
        {
            Debug.LogError(
                "Main Camera FollowPlayer is not assigned.",
                this);
        }

        return true;
    }

     TrackAsset FindTrack(string trackName)
    {
        TrackAsset track = executionTimeline
            .GetOutputTracks()
            .FirstOrDefault(
                timelineTrack =>
                    timelineTrack.name == trackName);

        if (track == null)
        {
            Debug.LogError(
                $"Timeline track '{trackName}' was not found.",
                this);
        }

        return track;
    }

    // First Timeline Signal.
    void BeginExecution()
    {
        if (!ExecutionPlaying || slowMotionActive)
            return;
        ExecutionPlaying = true;
        previousTimeScale = Time.timeScale;
        previousFixedDeltaTime = Time.fixedDeltaTime;

        SetPlayerControls(false);

        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime =
            previousFixedDeltaTime * slowMotionScale;

        slowMotionActive = true;
    }

    // Impact Timeline Signal.
     void ExecutionImpact()
    {
        // Add particles, sound or camera shake later.
        
    }

    // Final Timeline Signal.
     public void EndExecution()
    {
        if (!ExecutionPlaying && !slowMotionActive)
            return;

        ExecutionPlaying = false;

        if (director != null &&
            director.state == PlayState.Playing)
        {
            director.Stop();
        }
        
        FinishExecution();
    }

    void OnTimelineStopped(
        PlayableDirector stoppedDirector)
    {
        if (stoppedDirector != director)
            return;

        FinishExecution();
    }

     void FinishExecution()
    {
        if (!ExecutionPlaying && !slowMotionActive)
            return;

        RestoreGameplay();

        // Remove the previous bullet and enemy targets.
        if (executionCamera != null)
        {
            executionCamera.Follow = null;
            executionCamera.LookAt = null;
        }

        // Remove old Timeline bindings.
        if (director != null)
        {
            if (bulletTrack != null)
                director.ClearGenericBinding(bulletTrack);

            if (cameraTrack != null)
                director.ClearGenericBinding(cameraTrack);

            if (signalTrack != null)
                director.ClearGenericBinding(signalTrack);
        }

        currentBullet = null;
        currentEnemy = null;

        bulletTrack = null;
        cameraTrack = null;
        signalTrack = null;

        ExecutionPlaying = false;
    }

     void RestoreGameplay()
    {
        // Remove the execution targets.
        if (executionCamera != null)
        {
            executionCamera.Follow = null;
            executionCamera.LookAt = null;
            executionCamera.enabled = false;
        }

        // Stop Cinemachine from controlling the Main Camera.
        if (mainCameraBrain != null)
        {
            mainCameraBrain.enabled = false;
            
            mainCameraBrain.transform.SetPositionAndRotation(
                previousCameraPosition,
                previousCameraRotation
            );
        }

        // Return control to the normal camera-follow script.
        if (mainCameraFollowPlayer != null)
        {
            mainCameraFollowPlayer.enabled = true;
        }

        if (slowMotionActive)
        {
            Time.timeScale = previousTimeScale;
            Time.fixedDeltaTime =
                previousFixedDeltaTime;

            slowMotionActive = false;
        }

        SetPlayerControls(true);
    }

     void SetPlayerControls(bool controlsEnabled)
    {
        if (gameUI != null)
        {
            gameUI.SetActive(controlsEnabled);
        }
    }

     
    
}
