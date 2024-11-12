using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> dontDestroyObjects;

    void Awake()
    {
        foreach (var obj in dontDestroyObjects)
        {
            DontDestroyOnLoad(obj);
        }
    }
}
