using System;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField]private GameObject CoinDropPrefab;
    private const string coinHash = "CoinPila";
    
    public void DropCoins(int scoreValue, Vector3 spawnPosition)
    {
        Vector3 dropPosition = spawnPosition;
        GameObject coin =  GenericObjectPooler.Instance.GetFromPool(coinHash, CoinDropPrefab, dropPosition,  Quaternion.identity);
        if (coin.TryGetComponent(out Coins coinsScript)) coinsScript.Initialize(scoreValue);
    }
    
}
