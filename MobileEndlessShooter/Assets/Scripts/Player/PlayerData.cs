using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public GameObject boatModel;
    public WeaponData weaponData;
    public string boatName;
    public int boatIndex;
    public float maxHealth = 100f;
    public float shieldDuration = 10f;
}
