using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropStuds : MonoBehaviour
{
    public Stud stud;
    public int amount;
    private void OnDestroy()
    {
        for(; amount > 0; amount--)
        {
            Stud instance = Instantiate(stud, transform.position, Quaternion.Euler(Vector3.zero));
            instance.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.5f, 1.5f), 2f, Random.Range(-1.5f, 1.5f));
        }
    }
}
