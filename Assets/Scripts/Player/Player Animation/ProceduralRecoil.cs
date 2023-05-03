using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ProceduralRecoil : NetworkBehaviour
{

    public Vector3 recoilAmount = new Vector3(0f, 0f, 5f);
    public Vector3 recoilRotAmount = new Vector3(15f, 0f, 0f);
    [SerializeField] float time;
    [SerializeField] float rotTime;
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            PlayerAction.onPrimaryFire += Recoil;
        }
        
    }

    


    void Recoil(){
        //position
        Vector3 targetVec = transform.localPosition + recoilAmount;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetVec, time);

        //rotation
        Quaternion targetRot = transform.localRotation * Quaternion.Euler(recoilRotAmount);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, rotTime);
    }

    void OnDisable(){
        PlayerAction.onPrimaryFire -= Recoil;
    }
}
