using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearTimer : MonoBehaviour
{
    public float maxCount, count;
    public bool started;
    public Light lightSource;
    private void Update()
    {
        if (started)
        {
            if (lightSource) lightSource.enabled = false;
            GetComponent<MeshRenderer>().material.SetFloat("Vector1_E12B8954", 1 - count / maxCount);
            GetComponent<MeshRenderer>().material.SetFloat("Vector1_B2B0A39B", (count / maxCount) * .4f);
            count -= Time.deltaTime;
            if (count <= 0) Destroy(gameObject);
        }
    }

}
