using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    private Vector3 bulletDirection = Vector3.zero; 
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bulletDirection += transform.forward * (bulletSpeed * Time.deltaTime);
    }
    
}
