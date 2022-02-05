using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class FacePlayer : MonoBehaviour
{
    public RectTransform healthCanvas;
    public Health health;
    public Vector3 offset;
    void Update()
    {
        transform.position = health.transform.position + offset;
        transform.LookAt(Camera.main.transform.position);
        if (health.health >= health.maxHealth) healthCanvas.gameObject.SetActive(false);
        else healthCanvas.gameObject.SetActive(true);
    }
}
