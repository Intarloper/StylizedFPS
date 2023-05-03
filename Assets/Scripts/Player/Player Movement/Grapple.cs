using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    LineRenderer lr;
    public Vector3 grapplePoint;
    SpringJoint joint;
    
    public Transform grappleTransform, cam , player;
    public LayerMask grappleableWall, grappleableFloor;
    public float maxDistance;

    

    [Header("Joint Config")]
    public float spring;
    public float damper;
    public float massScale;

    [Header("State")]
    public static bool isGrappling;
    
    
    

    public KeyCode grappleKey = KeyCode.Q;

    // Start is called before the first frame update
    void Start()
    {

       lr = GetComponent<LineRenderer>();
       cam = GetComponentInParent<Camera>().transform;
       grappleTransform = GetComponentInParent<Grapple>().transform;
       player = GetComponentInParent<PlayerMovement>().transform;
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(grappleKey)){
            StartGrapple();
        }
        else if(Input.GetKeyUp(grappleKey)){
            StopGrapple();
        }

        
    }
    void LateUpdate(){
        DrawRope();
    }


    void StartGrapple(){
        
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappleableWall) 
        && player.gameObject.GetComponent<SpringJoint>() == null){

            if(player.GetComponent<SpringJoint>()!= null){
                foreach(SpringJoint spring in player){
                    Destroy(player.GetComponent<SpringJoint>());
                }
            }
            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceToGrapplePoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceToGrapplePoint * .25f;
            joint.minDistance = distanceToGrapplePoint * .5f;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

            
        }
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, grappleableFloor) 
        && player.gameObject.GetComponent<SpringJoint>() == null){
            
            if(player.GetComponent<SpringJoint>()!= null){
                foreach(SpringJoint spring in player){
                    Destroy(player.GetComponent<SpringJoint>());
                }
            }

            isGrappling = true;
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceToGrapplePoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceToGrapplePoint * .25f;
            joint.minDistance = 0;

            joint.spring = spring;
            joint.damper = damper;
            joint.massScale = massScale;

        }
        
    }

    void StopGrapple(){
        isGrappling = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    void DrawRope(){
        Vector3[] points = {grappleTransform.position, grapplePoint};
        if(!joint) return;
        // lr.SetPosition(0, grappleTransform.position);
        // lr.SetPosition(1, grapplePoint);
        lr.positionCount = points.Length;
        lr.SetPositions(points);
    }


    
}
