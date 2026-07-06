using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private PlayerData[] playerDataOptions;
    [SerializeField] private WeaponSpawner weaponSpawner;
    [SerializeField] private GameObject[] boatModels;
    [SerializeField] private PlayerAbilities playerAbilities;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedBoatIndex", 0);
        
        if (selectedIndex < 0 || selectedIndex >= playerDataOptions.Length)
        {
            selectedIndex = 0;
        }

        PlayerData selectedData = playerDataOptions[selectedIndex];

        ApplyPlayerData(selectedData);
        
        
    }
    
    //THIS APPLIES THE MODEL'S STATS INTO THE PLAYER'S FIELDS
    public void ApplyPlayerData(PlayerData data)
    {
        playerHealth.SetHealthData(data.maxHealth);
        
        playerAbilities.SetShieldDuration(data.shieldDuration);
        
        weaponSpawner.SetWeaponData(data.weaponData);
        
        
        
        
        
    }
    
}
