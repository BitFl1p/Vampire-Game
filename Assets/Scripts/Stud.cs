using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stud : MonoBehaviour
{
    public UIStud uiStud;
    public int value;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UIStud instance = Instantiate(uiStud, Vector3.zero, Quaternion.Euler(Vector3.zero), UI.instance.transform);
            instance.GetComponent<RectTransform>().localPosition = new Vector3(Camera.main.WorldToScreenPoint(transform.position).x, -5, Camera.main.WorldToScreenPoint(transform.position).y);
            Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
            instance.value = value;
            Destroy(gameObject);
        }
    }
}
