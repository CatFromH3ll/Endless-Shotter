using UnityEngine;

public class FollowProjectile : MonoBehaviour
{
    public Transform projectile;
    public Vector3 offset;
    
   
    void Start()
    {
        
    }

    public void UpdateProjectile(GameObject projectile)
    {
        this.projectile = projectile.transform;
    }

    private void InisialaizeOffset()
    {
        Vector3 camPos =new Vector3(transform.position.x + offset.x, transform.position.y + offset.y , projectile.position.z + offset.z);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos =new Vector3(transform.position.x, transform.position.y , projectile.position.z);
        transform.position = camPos;

    }
    public void EndOfExecution()
    {
        Debug.Log("deactivated");
        gameObject.SetActive(false);
    }
}