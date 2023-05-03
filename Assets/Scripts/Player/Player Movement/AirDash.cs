using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AirDash : NetworkBehaviour
{
    public Transform orient;
    public Camera playerCam;
    public GameObject playerCamGO;
    
    
    Rigidbody rb;
    
    public KeyCode airDashKey;

    float horizontalInput;
    float verticalInput;
    float dashTime;

    [Header("Dash")]
    public float dashIntensity;
    public float maxDashTime;


    [Header("Tilt")]
    public float tiltAmountX;
    public float tiltAmountZ;


    public static bool airDash;
    bool canAirDash;



    
    // Start is called before the first frame update
    void Start()
    {
        

        
        
    }

    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitForFind());
    }

    // Update is called once per frame
    void Update()
    {
       

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(airDashKey) && !PlayerMovement.isGrounded && canAirDash){
            airDash = true;
            dashTime = maxDashTime;
            canAirDash = false;
        }
        if(PlayerMovement.isGrounded){
            airDash = false;
            canAirDash = true;
        }

        if(airDash){
            Tilt();
        }
        
    }

    void FixedUpdate(){
        
            
        if(airDash){
            AirDashHandler();
        }
    }
    
    void AirDashHandler(){
        //Input Vector
        Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;
        //Add Force in direction of input Vector
        rb.AddForce(inputDir * dashIntensity, ForceMode.Impulse);
        dashTime -= Time.deltaTime;

        if(dashTime <= 0){
            airDash = false;
        }
    }
    
    void Tilt(){
        // Grab Axis' to make rotation happen based on direction
        float rotZ = -Input.GetAxis("Horizontal") * tiltAmountZ;
        float rotX = Input.GetAxis("Vertical") * tiltAmountX;

        Quaternion target = Quaternion.Euler(rotX,0,rotZ);

        playerCamGO.transform.localRotation = Quaternion.Lerp(playerCam.transform.localRotation, target, Time.deltaTime * 10);
        


    }

    IEnumerator WaitForFind()
    {
        yield return new WaitForSeconds(.6f);
        Find();
        
    }

    private void Find()
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody>();
        canAirDash = true;
        playerCamGO = FindObjectOfType<PlayerCamera>().gameObject;
        playerCam = playerCamGO.GetComponent<Camera>();
    }
    
}
