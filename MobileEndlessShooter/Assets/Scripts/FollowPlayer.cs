using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public float offset;
    [SerializeField] private Vector2 XYReset;
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

    public void ResetXYposition()
    { 
        transform.position = new Vector3(XYReset.x, XYReset.y, transform.position.z);
    }
}
