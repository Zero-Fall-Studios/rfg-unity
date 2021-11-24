using System.Collections.Generic;
using UnityEngine;

namespace RFG
{
  [AddComponentMenu("RFG/Object Pool/Object Pool")]
  public class ObjectPool : Singleton<ObjectPool>
  {
    [System.Serializable]
    public class Pool
    {
      public string tag;
      public GameObject prefab;
      public int size;
    }

    public class QueueObject
    {
      public GameObject gameObject;
      public IPooledObject pooledObject;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<QueueObject>> poolDictionary;

    private void Start()
    {
      poolDictionary = new Dictionary<string, Queue<QueueObject>>();

      foreach (Pool pool in pools)
      {
        // Created the pool
        Queue<QueueObject> objectPool = new Queue<QueueObject>();
        poolDictionary.Add(pool.tag, objectPool);

        // Create the gameObjects
        for (int i = 0; i < pool.size; i++)
        {
          GameObject obj = Instantiate(pool.prefab);
          obj.name = pool.tag;
          obj.SetActive(false);

          QueueObject queueObject = new QueueObject();
          queueObject.gameObject = obj;
          queueObject.gameObject.SetActive(false);
          queueObject.pooledObject = obj.GetComponent<IPooledObject>();
          poolDictionary[obj.name].Enqueue(queueObject);
        }
      }

    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, Transform parent = null, bool worldPositionStays = false, params object[] objects)
    {
      if (!poolDictionary.ContainsKey(tag))
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not exist");
        return null;
      }

      if (poolDictionary[tag].Count == 0)
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not contains any objects");
        return null;
      }

      QueueObject queueObject = poolDictionary[tag].Dequeue();
      GameObject objectToSpawn = queueObject.gameObject;
      objectToSpawn.SetActive(true);
      objectToSpawn.transform.position = position;
      objectToSpawn.transform.rotation = rotation;
      objectToSpawn.transform.SetParent(parent, worldPositionStays);

      IPooledObject pooledObj = queueObject.pooledObject;

      if (pooledObj != null)
      {
        pooledObj.OnObjectSpawn(objects);
      }

      poolDictionary[tag].Enqueue(queueObject);

      return objectToSpawn;
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation, params object[] objects)
    {
      if (!poolDictionary.ContainsKey(tag))
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not exist");
        return null;
      }

      if (poolDictionary[tag].Count == 0)
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not contains any objects");
        return null;
      }

      QueueObject queueObject = poolDictionary[tag].Dequeue();
      GameObject objectToSpawn = queueObject.gameObject;
      objectToSpawn.SetActive(true);
      objectToSpawn.transform.position = position;
      objectToSpawn.transform.rotation = rotation;

      IPooledObject pooledObj = queueObject.pooledObject;

      if (pooledObj != null)
      {
        pooledObj.OnObjectSpawn(objects);
      }

      poolDictionary[tag].Enqueue(queueObject);

      return objectToSpawn;
    }

    public void SpawnFromPool(string tag)
    {
      if (!poolDictionary.ContainsKey(tag))
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not exist");
        return;
      }

      if (poolDictionary[tag].Count == 0)
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not contains any objects");
        return;
      }

      QueueObject queueObject = poolDictionary[tag].Dequeue();
      GameObject objectToSpawn = queueObject.gameObject;
      objectToSpawn.SetActive(true);
      objectToSpawn.transform.position = transform.position;
      objectToSpawn.transform.rotation = transform.rotation;

      poolDictionary[tag].Enqueue(queueObject);

      return;
    }

    public void DeactivateAllByTag(string tag)
    {
      if (!poolDictionary.ContainsKey(tag))
      {
        LogExt.Warn<ObjectPool>($"Pool with tag {tag} does not exist");
        return;
      }
      foreach (QueueObject queueObject in poolDictionary[tag])
      {
        GameObject objectToSpawn = queueObject.gameObject;
        objectToSpawn.SetActive(false);
      }
    }
  }
}