using System.Collections.Generic;
using UnityEngine;

public class GenericObjectPooler : MonoBehaviour
{
    public static GenericObjectPooler Instance { get; private set; }

    // the dictionary tracks the different pools and by assingnig an ID knows what to poll out of the object pooler
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        //checks if there is another instance of the pooler
        if (Instance == null)
        {
            //if there isn't then this is the new instance
            Instance = this;
        }
        else
        {
            //if there is, destroy this pooler because there is no need for another 
            Destroy(gameObject);
        }
    }

    /*this method will check if there is a pool for a specific object
     and if there isn't create a pool for it */
    public void SetupPool(string poolKey, GameObject prefab, int initialSize)
    {
        if (poolDictionary.ContainsKey(poolKey)) return; //check if there is a pool for the object

        Queue<GameObject> newObjectQueue = new Queue<GameObject>(); // if there isn't creat a new one

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false); // Keep them hidden
            newObjectQueue.Enqueue(obj);
        }

        poolDictionary.Add(poolKey, newObjectQueue);// adds the pool for the dictionary
    }

    
    /* will get an object from the pool based on the poolkey and if the pool runs empy add another object to it */
    public GameObject GetFromPool(string poolKey, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        //will check if the pool exist if it doesn't create a new small one
        if (!poolDictionary.ContainsKey(poolKey))
        {
            SetupPool(poolKey, prefab, 5);
        }

        GameObject objectToSpawn; 

        // Check if we have a hidden object available
        if (poolDictionary[poolKey].Count > 0)
        {
            objectToSpawn = poolDictionary[poolKey].Dequeue(); // if there is then use it
        }
        else
        {
            //if there isn't create a new object to increase the pool size
            objectToSpawn = Instantiate(prefab);
        }

        //unhid it in the desired position
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // return it to the pool list so it can be reused again
        poolDictionary[poolKey].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}