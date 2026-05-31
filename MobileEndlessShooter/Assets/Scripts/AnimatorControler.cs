using UnityEngine;

public class AnimatorEventControler : MonoBehaviour 
{
    void ResicleEnemy()
    {
        GetComponentInParent<EnemyController>().RecycleEnemy();
    }
}
