using UnityEngine;

public class ExplosionLights : MonoBehaviour
{
    [SerializeField] private Light explosionLight;

    [Header("Light Settings")]
    [SerializeField] private float startingIntensity = 6f;
    [SerializeField] private float lightDuration = 0.25f;

    private float timer;

    private void OnEnable()
    {
        timer = 0f;
        explosionLight.intensity = startingIntensity;
        explosionLight.enabled = true;
    }

    private void Update()
    {
        timer += Time.fixedUnscaledDeltaTime;

        // Converts the timer into a value between 0 and 1.
        float progress = timer / lightDuration;

        // Quickly fades the light from full brightness to zero.
        explosionLight.intensity =
            Mathf.Lerp(startingIntensity, 0f, progress);

        if (progress >= 1f)
        {
            explosionLight.enabled = false;
        }
    }
    
    public void SetNightStrength(float nightAmount)
    {
        // nightAmount: 0 during daytime, 1 during full night.
        startingIntensity = Mathf.Lerp(4f, 8f, nightAmount);
    }
}
