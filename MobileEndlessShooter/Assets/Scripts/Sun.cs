using UnityEngine;

public class Sun : MonoBehaviour
{
    public float DaySpeed = 4f;
    public float NightSpeed = 12f;

    [SerializeField] private float nightStartY = -1.4f;
    [SerializeField] private float dayStartY = 1f;

    public bool isNight;
    public GameObject playerLights;

    private ExplosionLights explosionLights;

    private void Start()
    {
        explosionLights = FindFirstObjectByType<ExplosionLights>();

        //isNight = transform.position.y < nightStartY;
        playerLights.SetActive(isNight);
    }

    private void Update()
    {
        // Sunset: turn the lights on.
        if (!isNight && transform.position.y < nightStartY)
        {
            isNight = true;
            playerLights.SetActive(true);
            AudioManager.instance.TogglePlayerHeadlights();
        }
        // Sunrise: keep the lights on until the sun reaches dayStartY
        else if (isNight && transform.position.y > dayStartY)
        {
            isNight = false;
            playerLights.SetActive(false);
            AudioManager.instance.TogglePlayerHeadlights();
        }
        
        // Use the faster speed at night and the slower speed during the day
        float speed = isNight ? NightSpeed : DaySpeed;
        
        // Rotate the sun around the center of the world
        transform.RotateAround(
            Vector3.zero,
            Vector3.right,
            speed * Time.deltaTime
        );

        // Keep the sun facing the center
        transform.LookAt(Vector3.zero);
        explosionLights.SetNightStrength(speed);
    }
}
