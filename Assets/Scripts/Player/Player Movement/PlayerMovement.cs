using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
    
{
    [Header("Speed")]
    public float moveSpeed;
    public float slopeMoveSpeed;
    public float maxSpeed;

    [Header("Player Oreintation")]
    public Transform orient;

    //input
    float horizontalInput;
    float verticalInput;
    //input Vector
    Vector3 moveDir;

    public Rigidbody rb;

   


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    public float sphereRadius;
    public float maxSphereDistance;
    public static bool isGrounded;
   
    [Header("Drag Coeffecients")]
    public float groundDrag;
    public float airDrag;


    [Header("Jump Controls")]
    public float jumpForce;
    public float airMultiplier;
    public float jumpCd;
    bool exitingSlope;
    bool jump;
    //Used to see how long player is jumping for
    bool canJump;


    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Crouch Controls")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startingYScale;

    [Header("Slope Handling")]
    [SerializeField]float x;
    [SerializeField]float y;
    [SerializeField]float z;
    [SerializeField] float angle;
    public float maxSlopeAngle;
    RaycastHit slopeRaycast;


    [Header("Movement State")]
    public MovementState state;

    public enum MovementState{
        idle,
        walking,
        jumping,
        wallrunning,
        crouching,
        sliding,
        airdashing,
        grappling,
        inAir,
        

    }
    [Header("State Booleans")]
    public bool isSliding;
    
    public bool isWallrunning;

    



    void Start(){ 
        

        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true;
        canJump = true;

        startingYScale = transform.localScale.y;
        
        
    }

    void Update(){
        if(!IsOwner)
            return;


        PlayerInput();
        DragGroundedCheck();
        SpeedControl();
        StateHandler();
        // if(OnSlope() && !exitingSlope){
        //     SlopeDragLocalConverter();
        // }
        
        

    }


    void FixedUpdate() {
        

        MovePlayer();
        transform.rotation = orient.rotation;

        if(jump){
            Jump();
            jump = false;
        }

        
    }
    //CALLED IN UPDATE DIRECTLY
    void StateHandler(){

        //wallrunning
        if(isWallrunning){
            state = MovementState.wallrunning;

            
        }
        //jumping
        else if(Input.GetKey(jumpKey) && !canJump){
            state = MovementState.jumping;
        }
        //sliding
        else if(isSliding){
            state = MovementState.sliding;

        }
        //grappling
        else if(Grapple.isGrappling){
            state = MovementState.grappling;
        }
        //crouching
        else if(Input.GetKey(crouchKey)){
            state = MovementState.crouching;
            
        }
        //Air Dashing
        else if(Input.GetKeyDown(dashKey) && !isGrounded){
            state = MovementState.airdashing;
        }
        //walking
        else if(isGrounded && horizontalInput != 0 || verticalInput != 0){
            state = MovementState.walking;
        }
        else if(isGrounded){
            state = MovementState.idle;
        }
        
        //in the air
        else{
            state = MovementState.inAir;
        }


        
    }
 
    void PlayerInput(){
        //Grabbing Input Axis
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
    
        //Allows holding of space to continue jumping
        if(Input.GetKey(jumpKey) && canJump && isGrounded){
            //bool for jumping
            canJump = false;
            //bool to move jump physics to fixed update
            jump = true;
            
            Invoke(nameof(JumpReset), jumpCd);
        }

        
        //shrinks y scale
        //and adds force to push to ground
        if(Input.GetKeyDown(crouchKey)){
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        //resets y scale to stop crouch
        if(Input.GetKeyUp(crouchKey)){
            transform.localScale = new Vector3(transform.localScale.x, startingYScale, transform.localScale.z);
        }
    }
    
    void SpeedControl(){
        // maxSpeed is set to the log of our magnitude times 1/ mag with a scalar
        maxSpeed = Mathf.Log(rb.velocity.magnitude) * (1 / rb.velocity.magnitude) * 100;

        //taking square magnitude is faster
        if(rb.velocity.sqrMagnitude > maxSpeed * maxSpeed){
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }

    bool DragGroundedCheck(){
        
        isGrounded = Physics.SphereCast(transform.position, sphereRadius, -transform.up, out RaycastHit hit, maxSphereDistance, ground);
        
        if (isGrounded){
            rb.drag = groundDrag;
        }
        else if(OnSlope() && !exitingSlope){
            rb.drag = 0;
        }
        else{
            rb.drag = airDrag;
        }

        return isGrounded;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + -transform.up * maxSphereDistance, sphereRadius);
    }

    //CALLED IN UPDATE DIRECTLY

    //CALLED IN FIXED UPDATE DIRECTLY
    void MovePlayer(){
        
        moveDir = (orient.forward * verticalInput) + (orient.right * horizontalInput);

        
        //Slope Calculations
        if (OnSlope() && !exitingSlope){
            
            angle = (Vector3.Angle(Vector3.up, slopeRaycast.normal));
            // print(angle)
            
            
            rb.AddForce(GetSlopeMoveDirection(moveDir) * moveSpeed, ForceMode.Force);
            if(rb.velocity.y > 0){
                rb.AddForce(Vector3.down * 80, ForceMode.Force);
            }
            
        }
        //Slope Calculations


        //Normal Movement
        if(isGrounded){
            rb.AddForce(moveDir.normalized * moveSpeed , ForceMode.Force);
        }
        else if(!isGrounded){
            rb.AddForce(moveDir.normalized * (moveSpeed * airMultiplier) , ForceMode.Force);
        }//Normal Movement


        // rb.useGravity = !OnSlope();
    }
    //CALLED IN FIXED UPDATE DIRECTLY



   

    
    void Jump(){
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Force for jump
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    public void JumpReset(){
        exitingSlope = false;
        canJump = true;
        
    }


    //Slope Functions are refrenced in Sliding.cs

    public bool OnSlope(){
        //shoots a raycast down an takes in the normal of what it hits
        //if it is too great an angle (greater than maxslope value)
        //returns true if not just returns false

        if(Physics.Raycast(transform.position, Vector3.down, out slopeRaycast, playerHeight *  .5f + .3f)){
            Debug.DrawRay(transform.position,Vector3.down, Color.green);
            float angle = Vector3.Angle(Vector3.up, slopeRaycast.normal);
            return angle < maxSlopeAngle && angle != 0;
        }


        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction){
        //vector projected on plane set to direction of normal that was got from slope Raycast
        return Vector3.ProjectOnPlane(direction, slopeRaycast.normal).normalized;
    }

    void SlopeDragLocalConverter(){
        //god only knows :)
        
        //converts the drag from world to local space probably
        x = (-(.02f / 15) * angle) + .92666666f;
        z = (-(.02f / 15) * angle) + .92666666f;
        Vector3 vel = transform.InverseTransformDirection(rb.velocity);

            vel.x *= x;
            vel.y *= y;
            vel.z *= z;

            rb.velocity = transform.TransformDirection(vel);
    }
    
    


    

    
    

    


    

    



}
