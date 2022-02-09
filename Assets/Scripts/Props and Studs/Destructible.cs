using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Destructible : MonoBehaviour
{
    private bool destroyed;
    public bool killed;
    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            if (value == destroyed) return;
            destroyed = value;
            if (destroyed == true)
            {
                GetComponent<Collider>().enabled = false;
                for (; studCount > 0; studCount--)
                {
                    Stud instance = Instantiate(stud, transform.position, Quaternion.Euler(Vector3.zero));
                    instance.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-1.5f, 1.5f), 4f, Random.Range(-1.5f, 1.5f));
                }
                foreach (Rigidbody limb in limbs)
                {
                    limb.velocity = new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));
                    limb.drag = 1;
                    limb.angularDrag = 1;
                    limb.isKinematic = false;
                    limb.GetComponent<Collider>().enabled = true;
                    limb.GetComponent<DisappearTimer>().started = true;
                }
            }
        }
    }
    List<Rigidbody> limbs;
    public int studCount;
    public Stud stud;
    private void Start()
    {
        limbs = GetComponentsInChildren<Rigidbody>().ToList();
        limbs.Remove(GetComponent<Rigidbody>());
    }
    private void Update()
    {
        Destroyed = killed;
        if (transform.childCount <= 0) Destroy(gameObject);
    }
}
