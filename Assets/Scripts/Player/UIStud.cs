using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStud : MonoBehaviour
{
    public float speed;
    public int value;

    void Update()
    {
        transform.localPosition += (UI.instance.stud.transform.localPosition - transform.localPosition).normalized * speed;

        bool checkValue = AlmostEqual(transform.position.x, UI.instance.stud.transform.position.x, .004f) && 
                          AlmostEqual(transform.position.y, UI.instance.stud.transform.position.y, .004f) && 
                          AlmostEqual(transform.position.z, UI.instance.stud.transform.position.z, .004f);
        if (checkValue) 
        {
            CharacterController3D.instance.studs += value;
            Destroy(gameObject); 
        }
    }
    public static bool AlmostEqual(float a, float b, float eps)
    {
        return Mathf.Abs(a - b) < eps;
    }
}
