using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    public Transform orient;
    public Transform player;
    public CameraShake camShake;
    Rigidbody rb;
    PlayerMovement playerMovement;

    [Header("Sliding Variables")]
    public float maxSlideTime;
    public float slideForce;
    float slideTimer;

    public float slideYScale;
    float startYScale;

    [Header("Input")]
    public KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    bool slideStarted;
    
    
    
    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = player.localScale.y;

        
        
    }

    // Update is called once per frame
    void Update()
    {
       
            
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        
    
        
        if(Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && PlayerMovement.isGrounded){
            StartSlide();
            
        }
        //set slide timer cause im a fucking moron
        
        


        if(Input.GetKeyUp(slideKey) && playerMovement.isSliding){
            StopSlide();
           
        }
        
    }

    void FixedUpdate() {

        if(playerMovement.isSliding){
            SlideHandler();
        }
    }


    void StartSlide(){
        
        playerMovement.isSliding = true;
        //decreases player scale
        player.localScale = new Vector3(player.localScale.x, slideYScale, player.localScale.z);
        //adds force down 
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;

        
        StartCoroutine(camShake.StartCameraShakeEffect());
        

    }
    

    void StopSlide(){
        playerMovement.isSliding = false;
        //resets scale
        player.localScale = new Vector3(player.localScale.x, startYScale, player.localScale.z);
        
    }

    

    void SlideHandler(){
        //CALLED IN FIXEDUPDATE


        Vector3 inputDir = orient.forward * verticalInput + orient.right * horizontalInput;

        //sliding no slope
        if(!playerMovement.OnSlope() || rb.velocity.y > -.1f){
            //adds slide force to whatever direction you are inputting
            rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
            
            slideTimer -= Time.deltaTime;
        }
        else{
            rb.AddForce(playerMovement.GetSlopeMoveDirection(inputDir) * slideForce, ForceMode.Force);
        }
        
        if(slideTimer <= 0){
            StopSlide();
        }
    }

   
}
