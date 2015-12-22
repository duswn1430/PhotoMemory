using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPlooer : MonoBehaviour
{
    public static ObjectPlooer _Instance;

    public GameObject pooledBox;

    public int pooledAmount = 10;

    public bool willGrow = true;

    List<GameObject> pooledBoxs;

    void Awake()
    {
        _Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        pooledBoxs = new List<GameObject>();
        for (int i = 0; i < pooledAmount; ++i)
        {
            GameObject obj = (GameObject)Instantiate(pooledBox);
            obj.SetActive(false);
            pooledBoxs.Add(obj);
        }
    }

    public GameObject GetPooledBox()
    {
        for (int i = 0; i < pooledBoxs.Count; ++i)
        {
            if (!pooledBoxs[i].activeInHierarchy)
            {
                return pooledBoxs[i];
            }
        }

        if (willGrow)
        {
            GameObject obj = (GameObject)Instantiate(pooledBox);
            pooledBoxs.Add(obj);
            return obj;
        }

        return null;
    }
}
