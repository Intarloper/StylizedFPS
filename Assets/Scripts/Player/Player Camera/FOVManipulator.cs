using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;



public class FOVManipulator : NetworkBehaviour
{
    
    [Header("Wall Run")]
    [SerializeField] private Quaternion targetLeft = Quaternion.Euler(0, 0 , -15);
    [SerializeField] private Quaternion targetRight = Quaternion.Euler(0, 0 , 15);
    [Header("Air Dash")]
    public float desiredAirDashFOV;
    public float airDashFOVScalar;

    //Starting FOV and Rotation
    private float intialFOV;
    private Quaternion initalRot;
    
    //Refrences
    private PlayerMovement playerMovement;
    private WallRun wr;
    [SerializeField]private Camera playerCam;
    [SerializeField]private GameObject playerCamGO;
    

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(WaitForFind());
    }

   

    // Update is called once per frame
    void Update()
    {   
        if(!IsOwner) return;
        AirDashFOV();
        WallRunCameraTilt();
        SlideCameraTilt();
        
    }

    

    void AirDashFOV(){
        if(AirDash.airDash){
            if(playerCam.fieldOfView < desiredAirDashFOV){
                playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, desiredAirDashFOV, Time.deltaTime * airDashFOVScalar);
            }
        }

        if(!AirDash.airDash){
            if(playerCam.fieldOfView > intialFOV){
                playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, intialFOV, Time.deltaTime * airDashFOVScalar);
            }
        }
    }

    void WallRunCameraTilt(){
        if(playerMovement.isWallrunning && wr.wallLeft){
            print("Entered Tilt");
            playerCamGO.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, targetLeft, Time.deltaTime * 10);
        }
        if(playerMovement.isWallrunning && wr.wallRight){
            playerCamGO.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, targetRight, Time.deltaTime * 10);
        }
        if(!playerMovement.isWallrunning && playerCam.transform.localRotation != initalRot){
            playerCamGO.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation,initalRot, Time.deltaTime * 10);
        }
    }

    void SlideCameraTilt(){
        float playerMagnitude = playerMovement.rb.velocity.magnitude;
        float rot;

        //sets rotation equal to both axes if horizontal is NOT 0 so that we can still have a rotation if we are only moving forward
        if(-Input.GetAxisRaw("Horizontal") != 0){
            rot = Input.GetAxisRaw("Vertical") * -Input.GetAxisRaw("Horizontal") * playerMagnitude;
        }
        else{
            rot = Input.GetAxisRaw("Vertical") * playerMagnitude;
        }

        
        
        Quaternion slideRot = Quaternion.Euler(0,0, rot); 

        if(playerMovement.isSliding){
 
            playerCam.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, slideRot, Time.deltaTime * 10);
        }
    }

    IEnumerator WaitForFind()
    {
        yield return new WaitForSeconds(.6f);
        playerMovement = GetComponent<PlayerMovement>();
        wr = GetComponent<WallRun>();



        playerCamGO = GetComponentInChildren<SyncFPSCam>().gameObject;

        playerCam = playerCamGO.GetComponent<Camera>();
        
        intialFOV = playerCam.fieldOfView;
        initalRot = playerCam.transform.localRotation;
        
    }




    


   
}
