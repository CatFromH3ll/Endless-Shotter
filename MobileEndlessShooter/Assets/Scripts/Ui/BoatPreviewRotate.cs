using UnityEngine;
using UnityEngine.EventSystems;

public class BoatPreviewRotate : MonoBehaviour, IDragHandler
{
    
    
    [SerializeField] private Transform previewRoot;
    
    // How fast the boat rotates when dragging, Higher value = faster rotation.
    [SerializeField] private float rotationSpeed = 0.3f;
    
    // Stores the current up/down rotation.
    private float xRotation;

    // Stores the current left/right rotation.
    private float yRotation;

    // This function is called automatically while the player drags, on the UI object that has this script.
    public void OnDrag(PointerEventData eventData)
    {
        // If the player uses 2 fingers, do not rotate.
        // We save 2 fingers for pinch zoom later.
        if (Input.touchCount > 1)
        {
            return;
        }

        // eventData.delta tells us how much the finger/mouse moved, since the last frame.
        Vector2 dragDelta = eventData.delta;

        // Dragging left/right rotates the boat around the Y axis.
        yRotation -= dragDelta.x * rotationSpeed;

        // Dragging up/down tilts the boat around the X axis.
        xRotation += dragDelta.y * rotationSpeed;

        
        // Apply the final rotation to the preview parent. This rotates whichever boat is currently active.
        previewRoot.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
