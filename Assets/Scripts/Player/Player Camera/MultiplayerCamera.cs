using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiplayerCamera : NetworkBehaviour
{
    GameObject cameraHolder;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner){
            cameraHolder.SetActive(false);
        }
    }

    void Start(){
        cameraHolder = this.gameObject;
    }
}
