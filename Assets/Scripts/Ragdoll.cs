using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[ExecuteAlways]
public class Ragdoll : MonoBehaviour
{
    public bool ragdollEnabled, punchRagdoll;
    public Rigidbody chest;
    List<Rigidbody> limbs;
    private void Start()
    {
        limbs = GetComponentsInChildren<Rigidbody>().ToList();
        limbs.Remove(GetComponent<Rigidbody>());
    }
    private void Update()
    {
        foreach (Rigidbody limb in limbs) 
        {
            limb.drag = 5;
            limb.angularDrag = 5;
            limb.isKinematic = !ragdollEnabled;
            limb.GetComponent<Collider>().enabled = ragdollEnabled;
            transform.parent.GetComponent<Collider>().enabled = !ragdollEnabled;
            transform.parent.GetComponent<Animator>().enabled = !ragdollEnabled;
            transform.parent.GetComponent<Rigidbody>().isKinematic = ragdollEnabled;
        }
        if (punchRagdoll)
        {
            chest.velocity = Vector3.up * 1.2f;
        }
    }
}
