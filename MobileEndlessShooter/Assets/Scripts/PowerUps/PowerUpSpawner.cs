using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private PowerUpData[] powerUpsData;
    [SerializeField] private GameObject player;
    [SerializeField] private float distanceFromPlayer = 60.0f;
    [SerializeField] private float timeToSpawn = 5.0f;
    [SerializeField] private float timeInterval = 5.0f;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update()
    {
        PowerUpTimer();
        
    }
    
    private void SpawnPowerUp()
    {
        if (powerUpsData.Length == 0)
        {
            return;
        }

        float yOffset = 2.5f;
        float randomX = Random.Range(-7.0f, 7.0f);

        Vector3 spawnPosition = new Vector3(transform.position.x + randomX, yOffset, player.transform.position.z + distanceFromPlayer);

        int randomIndex = Random.Range(0, powerUpsData.Length);

        PowerUpData chosenPowerUp = powerUpsData[randomIndex];

        GenericObjectPooler.Instance.GetFromPool(
            chosenPowerUp.powerUpName,
            chosenPowerUp.powerUpPrefab,
            spawnPosition,
            chosenPowerUp.powerUpPrefab.transform.rotation
        );
        
    }
    
    

    private void PowerUpTimer()
    {
       timeToSpawn -= Time.deltaTime;
       
        if (timeToSpawn <= 0)
        {
            SpawnPowerUp();
            timeToSpawn =  timeInterval;
            
        }
    }
    
    
}
