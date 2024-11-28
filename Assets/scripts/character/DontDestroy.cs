using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> dontDestroyObjects;

    private static DontDestroy instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            foreach (var obj in dontDestroyObjects)
            {
                Destroy(obj);
            }

            Destroy(gameObject);

            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (var obj in dontDestroyObjects)
        {
            if (!IsDuplicate(obj))
            {
                DontDestroyOnLoad(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    private bool IsDuplicate(GameObject obj)
    {
        GameObject[] existingObjects = FindObjectsOfType<GameObject>();

        foreach (var existingObj in existingObjects)
        {
            if (existingObj.name == obj.name && existingObj != obj)
            {
                return true;
            }
        }

        return false;
    }
}
