using System;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private int coinValue;
    private GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (player != null && (player.transform.position.z) > (transform.position.z + 20f))// check if the player passed the Coin
        {
            RecycleCoin();
        }
    }
    
    public void Initialize(int coinValue)
    {
        this.coinValue = coinValue;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Add Score based on coin value
            RecycleCoin();
        }
    }
    
    private void RecycleCoin()
    {
        gameObject.SetActive(false);
    }
}
