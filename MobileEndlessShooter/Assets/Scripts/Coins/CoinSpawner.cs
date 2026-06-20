using System;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]private GameObject CoinDropPrefab;
    private const string coinHash = "CoinPila";
    
    
    public void DropCoins(int scoreValue, Vector3 spawnPosition)
    {
        float yOffset = 3.5f;
        Vector3 dropPosition = new Vector3(spawnPosition.x, spawnPosition.y - yOffset, spawnPosition.z);
        GameObject coin =  GenericObjectPooler.Instance.GetFromPool(coinHash, CoinDropPrefab, dropPosition,  Quaternion.identity);
        if (coin.TryGetComponent(out Coins coinsScript)) coinsScript.Initialize(scoreValue);
    }
    
}
