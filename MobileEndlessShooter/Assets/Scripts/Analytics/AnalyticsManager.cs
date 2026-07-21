using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public sealed class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }
    
    public bool IsReady { get; private set; }


    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            await UnityServices.InitializeAsync(); // Initializes Unity Gaming Services.
            
            AnalyticsService.Instance.StartDataCollection(); //Approves access is granted to record
            
            IsReady = true;
            Debug.Log("Analytics initialized. App version: " + Application.version);
        }
        catch (Exception exception)
        {
            IsReady = false;
            Debug.LogError("Analytics initialization failed: " + exception.Message);
        }
    }

    public void AnalyseRunOver(float survivalTime, int coinsCollected, int boatIndex,  int abilityUsed)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }
        RunOverEvent analyticsEvent = new()
        {
            CoinsCollected = coinsCollected,
            BoatIndex = boatIndex,
            SurvivalTime = survivalTime,
            AbilityUsed = abilityUsed
        };
        
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        Debug.Log("Analytics event sent: run_Completed");
    }

    public void AnalyseRewardClaimed(int rewardIndex)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }

        RewardClaimedEvent analyticsEvent = new()
        {
            RewardIndex = rewardIndex
        };
        
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        Debug.Log("Analytics event sent: reward_claimed");
    }

    public void AnalyseActionsMade(int jumpMade, int shotFired)
    {
        if (!IsReady)
        {
            Debug.LogWarning("Analytics is not ready");
            return;
        }

        ActionsMadeEvent analyticsEvent = new()
        {
            JumpMade = jumpMade,
            ShotFired = shotFired
        };
        
        AnalyticsService.Instance.RecordEvent(analyticsEvent);
        Debug.Log("Analytics event sent: action_made");
    }

    public sealed class RunOverEvent : Unity.Services.Analytics.Event
    {
        public RunOverEvent() : base("run_over")
        {
            
        }

        public float SurvivalTime
        {
            set => SetParameter("survival_time", value);
        }

        public int CoinsCollected
        {
            set => SetParameter("coins_collected", value);
        }

        public int BoatIndex
        {
            set => SetParameter("boat_index", value);
        }

        public int AbilityUsed
        {
            set => SetParameter("ability_used", value);
        }
    }

    public sealed class RewardClaimedEvent : Unity.Services.Analytics.Event
    {
        public RewardClaimedEvent() : base("reward_claimed")
        {
        }

        public int RewardIndex
        {
            set => SetParameter("reward_index", value);
        }
    }

    public sealed class ActionsMadeEvent : Unity.Services.Analytics.Event
    {
        public ActionsMadeEvent() : base("action_made")
        {
        }

        public int JumpMade
        {
            set => SetParameter("jump_made", value);
        }
        
        public int ShotFired
        {
            set => SetParameter("shot_fired", value);
        }
    }

    
}
