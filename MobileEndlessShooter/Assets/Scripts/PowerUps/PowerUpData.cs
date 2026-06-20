using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpData", menuName = "Scriptable Objects/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string powerUpName;
    public GameObject powerUpPrefab;
}
