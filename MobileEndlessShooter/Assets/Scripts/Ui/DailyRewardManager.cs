using System;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class DailyRewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dailyRewardPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private TextMeshProUGUI dailyRewardText; 
    
    [Header("Pre-placed Boat Models")]
    [Tooltip("Drag the 3D boat models that are ALREADY placed in your UI scene here!")]
    [SerializeField] private GameObject[] rewardBoatModelsInScene; 
    
    [Header("Reward Settings")]
    public int totalUnlockableBoats = 3; 

    private const string ChannelId = "daily_reward_channel";
    private const string LastClaimTimeKey = "LastClaimTime";
    private const string ConsecutiveDaysKey = "DailyRewardDayCount";
    private const string UnlockedBoatsKey = "UnlockedBoats"; 

    private void Start()
    {
        #if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Daily Reward",
            Importance = Importance.High,
            Description = "Reminds the player to claim their daily ship"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        #endif

        CheckRewardEligibility();
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Keyboard.current != null)
        {
            // R Key: Wipes everything and resets back to Day 1 (0 boats unlocked)
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            
                HideAllRewardBoats();
                CheckRewardEligibility();
            
                Debug.Log("PlayerPrefs wiped for testing!");
            }

            // N Key: Simulates skipping ahead to the next day for testing
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                if (dailyRewardPanel.activeSelf)
                {
                    int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1) + 1;
                    PlayerPrefs.SetInt(ConsecutiveDaysKey, currentDay);
                    PlayerPrefs.Save();
                }

                HideAllRewardBoats();
                
                int dayToShow = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);

                if (dayToShow <= totalUnlockableBoats)
                {
                    if (dailyRewardText != null)
                    {
                        dailyRewardText.text = $"Day {dayToShow} Reward!\nNew Boat Unlocked!";
                    }
                    // Day 1 = Index 0, Day 2 = Index 1, Day 3 = Index 2
                    ShowRewardBoat(dayToShow - 1);
                }
                else
                {
                    if (dailyRewardText != null)
                    {
                        dailyRewardText.text = $"Day {dayToShow} Reward!\nBonus Reward Unlocked!";
                    }
                }
                
                startPanel.SetActive(false);
                dailyRewardPanel.SetActive(true);

                Debug.Log($"Displaying Day {dayToShow} reward panel.");
            }
        }
        #endif
    }

    private void CheckRewardEligibility()
    {
        bool shouldShowReward = false;

        #if UNITY_ANDROID
        var intent = AndroidNotificationCenter.GetLastNotificationIntent();
        if (intent != null) shouldShowReward = true;
        #endif

        if (PlayerPrefs.HasKey(LastClaimTimeKey))
        {
            string lastClaimStr = PlayerPrefs.GetString(LastClaimTimeKey);
            DateTime lastClaimTime = DateTime.Parse(lastClaimStr);
            
            if ((DateTime.Now - lastClaimTime).TotalHours >= 24)
            {
                shouldShowReward = true;
            }
        }
        else
        {
            shouldShowReward = true; 
        }

        if (shouldShowReward)
        {
            int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);
            
            HideAllRewardBoats();

            if (currentDay <= totalUnlockableBoats)
            {
                if (dailyRewardText != null)
                {
                    dailyRewardText.text = $"Day {currentDay} Reward!\nNew Boat Unlocked!";
                }
                // Directly link day number to boat index (Day 1 -> Index 0)
                ShowRewardBoat(currentDay - 1);
            }
            else
            {
                if (dailyRewardText != null)
                {
                    dailyRewardText.text = $"Day {currentDay} Reward!\nBonus Reward Unlocked!";
                }
            }
            
            startPanel.SetActive(false);
            dailyRewardPanel.SetActive(true);
        }
    }
    
    private void ShowRewardBoat(int index)
    {
        if (rewardBoatModelsInScene == null) return;

        for (int i = 0; i < rewardBoatModelsInScene.Length; i++)
        {
            if (rewardBoatModelsInScene[i] != null)
            {
                rewardBoatModelsInScene[i].SetActive(i == index);
            }
        }
    }

    private void HideAllRewardBoats()
    {
        if (rewardBoatModelsInScene == null) return;
        for (int i = 0; i < rewardBoatModelsInScene.Length; i++)
        {
            if (rewardBoatModelsInScene[i] != null)
            {
                rewardBoatModelsInScene[i].SetActive(false);
            }
        }
    }

    public void ClaimReward()
    {
        int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);
        
        // Unlock boats up to the current day count, capped at your maximum limit
        int unlockedBoats = Mathf.Min(currentDay, totalUnlockableBoats);
        PlayerPrefs.SetInt(UnlockedBoatsKey, unlockedBoats);
        Debug.Log($"Success! Unlocked up to boat index {unlockedBoats - 1}!");

        // Increment the day counter for tomorrow
        currentDay++;
        PlayerPrefs.SetInt(ConsecutiveDaysKey, currentDay);

        PlayerPrefs.SetString(LastClaimTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();

        HideAllRewardBoats();
        dailyRewardPanel.SetActive(false);
        startPanel.SetActive(true);

        ScheduleNextRewardNotification();
    }

    private void ScheduleNextRewardNotification()
    {
        #if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();

        var notification = new AndroidNotification
        {
            Title = "New Ship Available!",
            Text = "Your next daily reward boat is ready to be unlocked.",
            FireTime = System.DateTime.Now.AddHours(24) 
        };

        AndroidNotificationCenter.SendNotification(notification, ChannelId);
        #endif
    }
}

/*
public class DailyRewardManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dailyRewardPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private TextMeshProUGUI dailyRewardText; 
    
    [Header("Pre-placed Boat Models")]
    [Tooltip("Drag the 3D boat models that are ALREADY placed in your UI scene here!")]
    [SerializeField] private GameObject[] rewardBoatModelsInScene; 
    
    [Header("Reward Settings")]
    public int totalUnlockableBoats = 3; 

    private const string ChannelId = "daily_reward_channel";
    private const string LastClaimTimeKey = "LastClaimTime";
    private const string ConsecutiveDaysKey = "DailyRewardDayCount";
    private const string UnlockedBoatsKey = "UnlockedBoats"; 

    private void Start()
    {
        #if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = ChannelId,
            Name = "Daily Reward",
            Importance = Importance.High,
            Description = "Reminds the player to claim their daily ship"
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        #endif

        CheckRewardEligibility();
    }

   private void Update()
{
    #if UNITY_EDITOR
    if (Keyboard.current != null)
    {
        // R Key: Wipes everything and resets back to Day 1
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        
            HideAllRewardBoats();
            CheckRewardEligibility();
        
            Debug.Log("PlayerPrefs wiped for testing!");
        }

        // N Key: Simulates the next reward / advances day without double-skipping
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            // If the panel is ALREADY open, pressing N advances to the subsequent day
            if (dailyRewardPanel.activeSelf)
            {
                int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1) + 1;
                int unlockedBoats = PlayerPrefs.GetInt(UnlockedBoatsKey, 1) + 1;

                PlayerPrefs.SetInt(ConsecutiveDaysKey, currentDay);
                // Cap unlocked boats at your maximum limit so it never exceeds available models
                PlayerPrefs.SetInt(UnlockedBoatsKey, Mathf.Min(unlockedBoats, totalUnlockableBoats));
                PlayerPrefs.Save();
            }

            // Clean slate for the boat visuals
            HideAllRewardBoats();
            
            int dayToShow = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);
            int boatToShow = PlayerPrefs.GetInt(UnlockedBoatsKey, 1);

            // Check if we are within the unlockable boat range (Days 1 to 3)
            if (dayToShow <= totalUnlockableBoats)
            {
                if (dailyRewardText != null)
                {
                    dailyRewardText.text = $"Day {dayToShow} Reward!\nNew Boat Unlocked!";
                }
                // Show the corresponding boat model (index starts at 0, so subtract 1)
                ShowRewardBoat(boatToShow - 1);
            }
            else // Past Day 3: No more boats to unlock!
            {
                if (dailyRewardText != null)
                {
                    dailyRewardText.text = $"Day {dayToShow} Reward!\nBonus Reward Unlocked!";
                }
                // HideAllRewardBoats() was already called above, so no boat will show here!
            }
            
            startPanel.SetActive(false);
            dailyRewardPanel.SetActive(true);

            Debug.Log($"Displaying Day {dayToShow} reward panel.");
        }
    }
    #endif
}

    private void CheckRewardEligibility()
    {
        bool shouldShowReward = false;

        #if UNITY_ANDROID
        var intent = AndroidNotificationCenter.GetLastNotificationIntent();
        if (intent != null) shouldShowReward = true;
        #endif

        if (PlayerPrefs.HasKey(LastClaimTimeKey))
        {
            string lastClaimStr = PlayerPrefs.GetString(LastClaimTimeKey);
            DateTime lastClaimTime = DateTime.Parse(lastClaimStr);
            
            if ((DateTime.Now - lastClaimTime).TotalHours >= 24)
            {
                shouldShowReward = true;
            }
        }
        else
        {
            shouldShowReward = true; 
        }

        if (shouldShowReward)
        {
            int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);
            int unlockedBoatsCount = PlayerPrefs.GetInt(UnlockedBoatsKey, 1);
            
            if (dailyRewardText != null)
            {
                dailyRewardText.text = $"Day {currentDay} Reward!\nNew Boat Unlocked!";
            }
            
            // Call the simplified UI toggle method
            ShowRewardBoat(unlockedBoatsCount - 1);
            
            startPanel.SetActive(false);
            dailyRewardPanel.SetActive(true);
        }
    }
    
    private void ShowRewardBoat(int index)
    {
        // Loop through the pre-placed models exactly like BoatSelector.cs
        for (int i = 0; i < rewardBoatModelsInScene.Length; i++)
        {
            if (rewardBoatModelsInScene[i] != null)
            {
                // Turn ON the boat that matches the current unlock index, and turn OFF the rest
                rewardBoatModelsInScene[i].SetActive(i == index);
            }
        }
    }

    public void ClaimReward()
    {
        int currentDay = PlayerPrefs.GetInt(ConsecutiveDaysKey, 1);
        int unlockedBoats = PlayerPrefs.GetInt(UnlockedBoatsKey, 1); 

        if (unlockedBoats < totalUnlockableBoats)
        {
            unlockedBoats++;
            PlayerPrefs.SetInt(UnlockedBoatsKey, unlockedBoats);
            Debug.Log($"Success! Boat index {unlockedBoats - 1} is now unlocked!");
        }
        else
        {
            Debug.Log("All boats already unlocked! Giving fallback reward...");
        }

        currentDay++;
        PlayerPrefs.SetInt(ConsecutiveDaysKey, currentDay);

        PlayerPrefs.SetString(LastClaimTimeKey, DateTime.Now.ToString());
        PlayerPrefs.Save();
        
        HideAllRewardBoats();
        
        dailyRewardPanel.SetActive(false);
        startPanel.SetActive(true);

        ScheduleNextRewardNotification();
    }
    
    private void HideAllRewardBoats()
    {
        if (rewardBoatModelsInScene == null) return;
        for (int i = 0; i < rewardBoatModelsInScene.Length; i++)
        {
            if (rewardBoatModelsInScene[i] != null)
            {
                rewardBoatModelsInScene[i].SetActive(false);
            }
        }
    }

    private void ScheduleNextRewardNotification()
    {
        #if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllNotifications();

        var notification = new AndroidNotification
        {
            Title = "New Ship Available!",
            Text = "Your next daily reward boat is ready to be unlocked.",
            FireTime = System.DateTime.Now.AddHours(24) 
        };

        AndroidNotificationCenter.SendNotification(notification, ChannelId);
        #endif
    }
}
*/