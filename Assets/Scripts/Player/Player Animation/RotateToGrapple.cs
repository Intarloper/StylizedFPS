using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGrapple : MonoBehaviour
{
    public Grapple grapple;
    [SerializeField] Quaternion desiredRot;
    [SerializeField] float rotSpeed;

    private Vector3 clampedRotation;

    private void Start()
    {
        grapple = GetComponentInParent<Grapple>();
    }

    void Update()
    {
        Rotate();
    }

    void Rotate(){
        if(!Grapple.isGrappling){
            desiredRot = transform.parent.rotation;
        }
        else{
            desiredRot = Quaternion.LookRotation(grapple.grapplePoint - transform.position);
        }
        

        
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRot, Time.deltaTime * rotSpeed);
    }
}
