using UnityEngine;
using UnityEngine.EventSystems;

public class BoatPreviewRotate : MonoBehaviour, IDragHandler
{
    
    
    [SerializeField] private Transform previewRoot;
    [SerializeField] private float zoomDeadZone = 10;
    [SerializeField] private Vector3 zoomAmount =  new (5.0f, 5.0f, 5.0f);
    [SerializeField] private Vector3 startingScale;
    [SerializeField] private float maxScale = 84.0f;
    [SerializeField] private float minScale = 50.0f;
    // How fast the boat rotates when dragging, Higher value = faster rotation.
    [SerializeField] private float rotationSpeed = 0.3f;
    
    // Stores the current up/down rotation.
    private float xRotation;

    // Stores the current left/right rotation.
    private float yRotation;


    private void Start()
    {
        startingScale = previewRoot.localScale;
    }
    private void Update()
    {
        
        if(Input.touchCount == 2)
        {
            Touch touch1 =  Input.GetTouch(0);
            Touch touch2 =  Input.GetTouch(1);
            Vector2 currentDist = touch1.position - touch2.position;
            Vector2 previousDist = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);
            float delta = currentDist.magnitude - previousDist.magnitude;
            
            //if two fingers are dragging away zoom in
            if (delta > zoomDeadZone)
            {
                ClampScale();
                startingScale +=  zoomAmount;
                previewRoot.localScale = startingScale;
            }
            //if two fingers are dragging in, zoom out
            else if (delta < -zoomDeadZone)
            {
                ClampScale();
                startingScale -= zoomAmount;
                previewRoot.localScale = startingScale;
            }
            
        }
    }

    public void ClampScale()
    {
        startingScale.x = Mathf.Clamp(startingScale.x, minScale, maxScale);
        startingScale.y = Mathf.Clamp(startingScale.y, minScale, maxScale);
        startingScale.z = Mathf.Clamp(startingScale.z, minScale, maxScale);
    }

    // This function is called automatically while the player drags, on the UI object that has this script.
    public void OnDrag(PointerEventData eventData)
    {
        // If the player uses 2 fingers, do not rotate.
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
