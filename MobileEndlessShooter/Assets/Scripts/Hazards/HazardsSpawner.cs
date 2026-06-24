using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Lumin;
using Random = System.Random;

public class HazardsSpawner : MonoBehaviour
{
    [SerializeField] private HazardData[] hazardData;
    public GameObject player;
    private const string obsticalHash = "obstical";
    Random random = new Random();
    private Vector3 leftPoint;
    private Vector3 rightPoint;
    private float waitTime;
    private int startRnd = 4;
    private int endRnd = 9;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(SpawnRoutine());
    }

    private void Update()
    {
        float leftMargin = 0.45f;
        float rightMargin = 0.55f;
            
        leftPoint = Camera.main.ViewportToWorldPoint
            (new Vector3(leftMargin,0.5f, transform.position.z - Camera.main.transform.position.z));
            
        rightPoint = Camera.main.ViewportToWorldPoint
            (new Vector3(rightMargin ,0.5f , transform.position.z - Camera.main.transform.position.z));
        
        
         
        
    }
    
    private IEnumerator SpawnRoutine()
    {
        waitTime = random.Next(startRnd, endRnd);
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            SpawnObstical();
        }
    }
    
    public void SpawnObstical()
    {
        Vector3 spawnPosition =  player.transform.position;
        spawnPosition.z += 200;
        spawnPosition.x = random.Next((int)leftPoint.x ,(int)rightPoint.x);;
        HazardData chosenHazard = hazardData[random.Next(0 ,hazardData.Length)];
        
        GameObject hazard = GenericObjectPooler.Instance.GetFromPool(
            obsticalHash,
            chosenHazard.HazardPrefab,
            spawnPosition,
            Quaternion.identity);
        if (hazard.TryGetComponent<Hazards>(out var obstacleScript))
        {
            obstacleScript.Initialize(chosenHazard);
        }
        
    }
}
