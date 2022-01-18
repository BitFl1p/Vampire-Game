using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speen : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.eulerAngles += Vector3.one;
    }
}
