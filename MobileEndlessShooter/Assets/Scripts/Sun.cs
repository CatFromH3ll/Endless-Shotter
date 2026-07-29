using UnityEngine;

public class Sun : MonoBehaviour
{
    public float DaySpeed = 4f;
    public float NightSpeed = 15f;
    public GameObject playerLights;

    private ExplosionLights explosionLights;
// Update is called once per frame
    void Update()
    {
        float speed;

        if (transform.position.y >= -1.4)
        {
            speed = DaySpeed;
            playerLights.SetActive(false);
        }

        else
        {
            speed = NightSpeed;
            playerLights.SetActive(true);
        }

        transform.RotateAround(Vector3.zero, Vector3.right, speed * Time.deltaTime);
        transform.LookAt(Vector3.zero);
        explosionLights.SetNightStrength(speed);
    }

}
