using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KeepCameraOnPlayer : NetworkBehaviour
{
    
    private Transform camPosition;
    

    void Start(){
        print(OwnerClientId);
        print(IsOwner);
        if(IsOwner){
            print("Cam Set");
            transform.position = camPosition.transform.position;
            
        }

        

        
    }

    
}
