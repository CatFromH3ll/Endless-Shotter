using UnityEngine;

[CreateAssetMenu(fileName = "NewHazardData", menuName = "EndlessShooter/Hazard Data")]
public class HazardData : ScriptableObject
{
    [Header("Visuals")]
    public GameObject HazardPrefab;
    
    
    
    [Header("Damage")] public int damage = 10;

    
}
