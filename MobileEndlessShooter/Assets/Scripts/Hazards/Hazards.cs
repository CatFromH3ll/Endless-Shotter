using Unity.VisualScripting;
using UnityEngine;

public class Hazards : MonoBehaviour
{
    HazardData data;
    public float damage { get; private set; }
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
        
    }

    // Update is called once per frame
    public void Initialize(HazardData data)
    {
        this.data = data;   
        damage =+ data.damage * UIControler.selectedDifficultyModifier;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Bullet")
        {
            gameObject.SetActive(false);
            AudioManager.instance.PlayerProjectileHitSound();
        }
    }
}
