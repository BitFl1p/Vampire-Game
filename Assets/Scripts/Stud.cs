using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stud : MonoBehaviour
{
    public UIStud uiStud;
    public int value;
    public float collectTimer;
    private void Update()
    {
        collectTimer -= Time.deltaTime;
        if (collectTimer < 0)
        {
            if (Vector3.Distance(transform.position, new Vector3(CharacterController3D.instance.transform.position.x, transform.position.y, CharacterController3D.instance.transform.position.z)) < 0.6)
            {
                UIStud instance = Instantiate(uiStud, Vector3.zero, Quaternion.Euler(Vector3.zero), UI.instance.transform);
                RectTransform rect = instance.GetComponent<RectTransform>();
                Vector2 screenPoint = (Vector2)Camera.main.WorldToScreenPoint(transform.position) - new Vector2(Screen.width / 2, Screen.height / 2);
                /*if(RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, Camera.main, out Vector2 screenPos))
                {
                    rect.localPosition = screenPos;
                }*/
                rect.localPosition = screenPoint;

                //Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
                //rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0);
                instance.value = value;
                Destroy(gameObject);
            }
        }
    }
    
}
