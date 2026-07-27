using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BoatPreviewRotate : MonoBehaviour, IDragHandler
{
    
    
    [SerializeField] private Transform previewRoot;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.05f;
    [SerializeField] private float zoomDeadZone = 0.5f;
    [SerializeField] private float maxScale = 84f;
    [SerializeField] private float minScale = 50f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.3f;

    // Current uniform scale of the preview model.
    private float currentScale;

    // Distance between the two fingers during the previous frame.
    private float previousPinchDistance = -1f;

    // Current preview rotation values.
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        // Required before using Enhanced Touch input.
        EnhancedTouchSupport.Enable();
    }

    private void Start()
    {
        // Store the model's starting scale.
        currentScale = previewRoot.localScale.x;
    }

    private void Update()
    {
        var touches = Touch.activeTouches;

        // Pinch zoom requires two fingers.
        if (touches.Count < 2)
        {
            // Reset so the next pinch starts cleanly.
            previousPinchDistance = -1f;
            return;
        }

        Vector2 firstFingerPosition = touches[0].screenPosition;
        Vector2 secondFingerPosition = touches[1].screenPosition;

        // Find the current distance between the two fingers.
        float currentPinchDistance = Vector2.Distance(
            firstFingerPosition,
            secondFingerPosition);

        // On the first pinch frame, only save the distance.
        if (previousPinchDistance < 0f)
        {
            previousPinchDistance = currentPinchDistance;
            return;
        }

        // Positive means fingers moved apart.
        // Negative means fingers moved closer.
        float pinchDelta =
            currentPinchDistance - previousPinchDistance;

        previousPinchDistance = currentPinchDistance;

        // Ignore very small finger movement.
        if (Mathf.Abs(pinchDelta) < zoomDeadZone)
            return;

        // Change and clamp the model scale.
        currentScale += pinchDelta * zoomSpeed;
        currentScale = Mathf.Clamp(
            currentScale,
            minScale,
            maxScale);

        // Apply equal scale to every axis.
        previewRoot.localScale = Vector3.one * currentScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Do not rotate while the player is pinch zooming.
        if (Touch.activeTouches.Count > 1)
            return;

        Vector2 dragDelta = eventData.delta;

        // Horizontal drag rotates left and right.
        yRotation -= dragDelta.x * rotationSpeed;

        // Vertical drag tilts up and down.
        xRotation += dragDelta.y * rotationSpeed;

        previewRoot.rotation =
            Quaternion.Euler(xRotation, yRotation, 0f);
    }

    private void OnDestroy()
    {
        // Clean up Enhanced Touch when this object is destroyed.
        EnhancedTouchSupport.Disable();
    }
}
