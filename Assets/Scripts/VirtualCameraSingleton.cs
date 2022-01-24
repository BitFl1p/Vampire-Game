using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraSingleton : MonoBehaviour
{
    #region Singleton Shit
    public static VirtualCameraSingleton instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion
}
