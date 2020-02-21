using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineBottom : MonoBehaviour
{
    public virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Vector3 vel = new Vector3(0, 0, 0);

            foreach (Rigidbody a in col.transform.root.gameObject.GetComponentsInChildren<Rigidbody>())
            {
                a.velocity = vel;
            }

        }
    }
}
