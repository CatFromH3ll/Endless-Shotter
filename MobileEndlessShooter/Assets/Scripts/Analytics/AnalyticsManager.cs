using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public sealed class AnalyticsManager : MonoBehaviour
{
    // Singleton instance, allowing access through AnalyticsManager.Instance.
    public static AnalyticsManager Instance { get; private set; }

// True only after Unity Analytics has initialized successfully.
    public bool IsReady { get; private set; }


    private async void Awake()
    {
        // Prevents duplicate AnalyticsManager objects between scenes.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Stores this object as the singleton instance.
        Instance = this;

        // Keeps the AnalyticsManager alive when changing scenes.
        DontDestroyOnLoad(gameObject);

        try
        {
            // Initializes Unity Gaming Services.
            await UnityServices.InitializeAsync();

            // Starts collecting analytics data.
            AnalyticsService.Instance.StartDataCollection();

            // Analytics events can now be recorded.
            IsReady = true;

            Debug.Log("Analytics initialized. App version: " + Application.version);
        }
        catch (Exception exception)
        {
            // Analytics initialization failed, so events should not be recorded.
            IsReady = false;

            Debug.LogError("Analytics initialization failed: " + exception.Message);
        }
    }


// Records the player's results when a run ends.
    public void AnalyseRunOver(
        float survivalTime,
        int coinsCollected,
        int boatIndex,
        int abilityUsed)
    {
        // Prevents recording events before Analytics is ready.
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }

        // Creates the event and assigns its parameter values.
        RunOverEvent analyticsEvent = new()
        {
            CoinsCollected = coinsCollected,
            BoatIndex = boatIndex,
            SurvivalTime = survivalTime,
            AbilityUsed = abilityUsed
        };

        // Sends the event to Unity Analytics.
        AnalyticsService.Instance.RecordEvent(analyticsEvent);

        Debug.Log("Analytics event sent: run_Completed");
    }


// Records which daily reward the player claimed.
    public void AnalyseRewardClaimed(int rewardIndex)
    {
        // Prevents recording events before Analytics is ready.
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }

        // Creates the reward event and assigns the claimed reward index.
        RewardClaimedEvent analyticsEvent = new()
        {
            RewardIndex = rewardIndex
        };

        // Sends the event to Unity Analytics.
        AnalyticsService.Instance.RecordEvent(analyticsEvent);

        Debug.Log("Analytics event sent: reward_claimed");
    }


// Records how many jumps and shots the player made.
    public void AnalyseActionsMade(int jumpMade, int shotFired)
    {
        // Prevents recording events before Analytics is ready.
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }

        // Creates the action event and assigns the final counters.
        ActionsMadeEvent analyticsEvent = new()
        {
            JumpMade = jumpMade,
            ShotFired = shotFired
        };

        // Sends the event to Unity Analytics.
        AnalyticsService.Instance.RecordEvent(analyticsEvent);

        Debug.Log("Analytics event sent: action_made");
    }


// Custom Analytics event containing the player's final run information.
    public sealed class RunOverEvent : Unity.Services.Analytics.Event
    {
        // The name must match the event created in the Unity Analytics dashboard.
        public RunOverEvent() : base("run_over")
        {

        }

        // Adds the player's survival time to the event.
        public float SurvivalTime
        {
            set => SetParameter("survival_time", value);
        }

        // Adds the number of coins collected during the run.
        public int CoinsCollected
        {
            set => SetParameter("coins_collected", value);
        }

        // Adds the index of the boat used during the run.
        public int BoatIndex
        {
            set => SetParameter("boat_index", value);
        }

        // Adds the number of times the player's ability was used.
        public int AbilityUsed
        {
            set => SetParameter("ability_used", value);
        }
    }


// Custom Analytics event for claiming a daily reward.
    public sealed class RewardClaimedEvent : Unity.Services.Analytics.Event
    {
        // The name must match the event created in the Unity Analytics dashboard.
        public RewardClaimedEvent() : base("reward_claimed")
        {
        }

        // Adds the claimed reward's index to the event.
        public int RewardIndex
        {
            set => SetParameter("reward_index", value);
        }
    }


// Custom Analytics event containing the player's action counters.
    public sealed class ActionsMadeEvent : Unity.Services.Analytics.Event
    {
        // The name must match the event created in the Unity Analytics dashboard.
        public ActionsMadeEvent() : base("action_made")
        {
        }

        // Adds the number of jumps made during the run.
        public int JumpMade
        {
            set => SetParameter("jump_made", value);
        }

        // Adds the number of shots fired during the run.
        public int ShotFired
        {
            set => SetParameter("shot_fired", value);
        }
    }
}


