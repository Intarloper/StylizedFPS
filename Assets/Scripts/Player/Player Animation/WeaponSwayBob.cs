using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;


public class WeaponSwayBob : NetworkBehaviour
{
    PlayerMovement pm;
    float mouseX;
    float mouseY;
    Vector2 walkInput;

    Quaternion swayTargetRot;
    [Header("Sway Controls")]
    [SerializeField] float smoothStep;
    [SerializeField] float swayAmount;


    [Header("Bob controls")]
    [SerializeField] float bobAmount;
    [SerializeField] float speedCurve;
    [SerializeField] float frequencyScalarSin;
    [SerializeField] float frequencyScalarCos;
    float sinCurve {get => Mathf.Sin(speedCurve / frequencyScalarSin);}
    float cosCurve {get => Mathf.Cos(speedCurve / frequencyScalarCos);}

    public Vector3 weaponTravelLimit = Vector3.one * .1f; //limits travel from ove input
    public Vector3 bobLimit = Vector3.one * .01f; // lomits bobbing over time

    Vector3 bobPosition;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEulerRotation;

    
    // Start is called before the first frame update
    void Start()
    {
        
        if(IsOwner){
            pm = GetComponentInParent<PlayerMovement>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        
        GetInput();

        Sway();
        BobOffset();

        if(PlayerMovement.isGrounded){ 
            BobRotation(); 
        }

        PositionRotationSet();

        
    }


    void GetInput(){
        mouseX = Input.GetAxisRaw("Mouse X") * swayAmount - 90f;
        mouseY = Input.GetAxisRaw("Mouse Y") * swayAmount;
        
        walkInput.x = Input.GetAxisRaw("Horizontal");
        walkInput.y = Input.GetAxisRaw("Vertical");
        walkInput = walkInput.normalized;
        
    }

    void Sway(){
        
        Quaternion rotX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotY = Quaternion.AngleAxis(mouseX, Vector3.up);


        swayTargetRot = rotX * rotY;

        
    }

    void BobOffset(){
        speedCurve += Time.deltaTime * (PlayerMovement.isGrounded ?  pm.rb.velocity.magnitude : 1f ) + 0.1f;
        
        if(pm.state != PlayerMovement.MovementState.idle){
            if(PlayerMovement.isGrounded || pm.isWallrunning){
                bobPosition.x = ((cosCurve * bobLimit.x) * (PlayerMovement.isGrounded ? 1:0)) - (walkInput.x * weaponTravelLimit.x);
                bobPosition.y = (sinCurve * bobLimit.y) - (pm.rb.velocity.y * weaponTravelLimit.y);
                bobPosition.z = -(walkInput.y * weaponTravelLimit.z);
            }
            else{
                //no sin or cos curve  multiplied while in air
                bobPosition.x = ((bobLimit.x) * (PlayerMovement.isGrounded ? 1:0)) - (walkInput.x * weaponTravelLimit.x);
                bobPosition.y = (bobLimit.y) - (pm.rb.velocity.y * weaponTravelLimit.y);
                bobPosition.z = -(walkInput.y * weaponTravelLimit.z);
            }

            
        }
        else{
            bobPosition = Vector3.zero;
        }
        // print(bobPosition.x);

    }

    void BobRotation(){
        bobEulerRotation.x = (walkInput != Vector2.zero ? multiplier.x * sinCurve : 0);
        bobEulerRotation.y = (walkInput != Vector2.zero ? multiplier.y * cosCurve : 0);
        bobEulerRotation.z = (walkInput != Vector2.zero ? multiplier.z * cosCurve * walkInput.x : 0);
    }

    void PositionRotationSet(){
        transform.localPosition = Vector3.Lerp(transform.localPosition, bobPosition, Time.deltaTime * smoothStep);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, swayTargetRot * Quaternion.Euler(bobEulerRotation), smoothStep * Time.deltaTime);
    }
}
