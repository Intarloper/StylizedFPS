using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    void OnEnable(){
        rb = GetComponent<Rigidbody>();
        Destroy(this.gameObject, 10f);
    }
    

    private void OnCollisionEnter(Collision other) {
        rb.isKinematic = true;

        Destroy(this.gameObject, 3f);
    }
}
