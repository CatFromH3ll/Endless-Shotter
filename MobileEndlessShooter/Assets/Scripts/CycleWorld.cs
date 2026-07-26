using System;
using System.Collections.Generic;
using UnityEngine;

public class CycleWorld : MonoBehaviour
{
    [SerializeField] private float startPosition;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject world;
    [SerializeField] private Renderer oceanRenderer;

    private Material oceanMaterial;
    private Vector2 oceanTextureOffset;


    private void Start()
    {
        // Save the player's original Z position.
        startPosition = player.transform.position.z;

        if (oceanRenderer != null)
        {
            // Create a runtime instance of the ocean material.
            oceanMaterial = oceanRenderer.material;

            // Save the ocean texture's starting offset.
            oceanTextureOffset = oceanMaterial.mainTextureOffset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only cycle the world when the player enters the trigger.
        if (!other.CompareTag("Player"))
            return;

        // Calculate how far the player travelled from the start position.
        float zOffset =
            player.transform.position.z - startPosition;

        Vector3 resetOffset =
            new Vector3(0f, 0f, zOffset);

        // Move the world and player backward by the same distance.
        world.transform.position -= resetOffset;
        player.transform.position -= resetOffset;

        if (oceanMaterial != null)
        {
            // Get the ocean plane's real world-space length.
            float oceanLengthZ =
                oceanRenderer.bounds.size.z;

            // Get how many times the texture repeats vertically.
            float textureTilingY =
                oceanMaterial.mainTextureScale.y;

            // Convert the travelled world distance into a texture offset.
            float offsetAmount =
                (zOffset / oceanLengthZ) * textureTilingY;

            // Offset the ocean texture to hide the player's teleport.
            oceanTextureOffset.y -= offsetAmount;

            // Apply the new offset to the material.
            oceanMaterial.mainTextureOffset =
                oceanTextureOffset;
            }
        }
}

