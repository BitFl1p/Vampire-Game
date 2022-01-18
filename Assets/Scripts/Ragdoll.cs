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
        foreach (Rigidbody limb in limbs) limb.isKinematic = !ragdollEnabled;
        if (punchRagdoll)
        {
            chest.velocity = Vector3.up;
        }
    }
}
