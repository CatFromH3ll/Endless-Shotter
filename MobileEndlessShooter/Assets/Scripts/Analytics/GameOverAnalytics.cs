using UnityEngine;

public class GameOverAnalytics : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private Score scoreManager;
    [SerializeField] private ModelSelector boatSelector;
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private PlayerMovement playerJump;
    [SerializeField] private WeaponSpawner weaponSpawner;
    
    
    
    public void GameOverAnalyse()
    {
        AnalyticsManager.Instance.AnalyseRunOver(
            gameTimer.time, scoreManager.CoinCollected,
            PlayerPrefs.GetInt("SelectedBoatIndex",0),playerAbilities.AbilityUsed);
        
        AnalyticsManager.Instance.AnalyseActionsMade(
            playerAbilities.AbilityUsed,weaponSpawner.ShotsFired);
    }
}


