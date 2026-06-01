using System;
using System.Collections.Generic;
using UnityEngine;

public class CycleWorld : MonoBehaviour
{
    [SerializeField] private float startPosition;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject world;
    [SerializeField] private Collider resetWorldCollider;


    private void Start()
    {
        startPosition = player.transform.position.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float zOffset = player.transform.position.z - startPosition;

            world.transform.position -= new Vector3(0f, 0f, zOffset);

            Vector3 playerPos = player.transform.position;
            playerPos.z = startPosition;
            player.transform.position = playerPos;
        }
        
    }
}
