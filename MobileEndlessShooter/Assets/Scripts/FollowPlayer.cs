using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float offset;
   
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camPos =new Vector3(transform.position.x, transform.position.y , player.position.z - offset);
        transform.position = camPos;

    }
}
