using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerParentSetup : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerTransform;
    [SerializeField]
    private Transform cameraPosTransform;

    [SerializeField]
    public GameObject cameraHolderGO;

    [SerializeField] 
    public GameObject cameraGO;

    [SerializeField] 
    public GameObject weaponHolderGO;

    private PlayerWeaponSelector weaponSelector;
    
    [SerializeField] 
    public GameObject FPSCamGO;

    public WeaponSO weaponSO;

    private ulong ownerId;
    private NetworkObject networkObject;

    
    
   
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner) return;
        StartCoroutine(Wait());

    }

   
    [ServerRpc(RequireOwnership = false)]
    private void PlayerSetupStartServerRpc(ulong ownerId)
    {
        print("PlayerSetup" + IsOwner);
        
        weaponSelector = GetComponent<PlayerWeaponSelector>();
        playerTransform = this.gameObject;
        cameraPosTransform = transform.GetChild(2);
        //Parent Layer
        //Player
            //CameraHolder
                //Camera
                    //WeaponHolder
                    //FPSCamera
                        //ArmHolder
                    
        //CameraHolder            
        // ParentCameraHolderServerRpc(ownerId);
        cameraHolderGO = Instantiate(cameraHolderGO, cameraPosTransform.position, Quaternion.identity);
        cameraHolderGO.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        cameraHolderGO.transform.position = cameraPosTransform.position;
        cameraHolderGO.transform.SetParent(playerTransform.transform);
        
        //Camera
        // ParentCameraServerRpc(ownerId);
        cameraGO = Instantiate(cameraGO, cameraHolderGO.transform.position, Quaternion.identity);
        cameraGO.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        cameraGO.transform.SetParent(cameraHolderGO.transform);
        //WeaponHolder
        weaponHolderGO = Instantiate(weaponHolderGO, cameraGO.transform.position, Quaternion.identity);
        weaponHolderGO.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        weaponHolderGO.transform.SetParent(cameraGO.transform);

        
        // ParentWeaponHolderServerRpc(ownerId);
        //Weapon is next, handled in weaponSelector
        
        // ParentWeaponServerRpc(ownerId);
        weaponSelector.SetupWeapon();
        weaponSO.Spawn(weaponHolderGO.transform, this, ownerId);
        //FPSCamera
        
        // ParentFpsCameraServerRpc(ownerId);
        FPSCamGO = Instantiate(FPSCamGO, cameraGO.transform.position, Quaternion.identity);
        FPSCamGO.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        FPSCamGO.transform.position = cameraGO.transform.position;
        FPSCamGO.transform.SetParent(cameraGO.transform, true);
        //ArmHolder is already parented to FPSCamera
        
        // weaponHolderGO.transform.localPosition = new Vector3(.15f, -.1f, .3f);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(.1f);
        networkObject = GetComponent<NetworkObject>();
        ownerId = networkObject.OwnerClientId;
        
        PlayerSetupStartServerRpc(ownerId);
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // [ServerRpc(RequireOwnership = false)]
    // private void ParentCameraHolderServerRpc(ulong ownerId)
    // {
    //     
    // }
    // [ServerRpc(RequireOwnership = false)]
    // private void ParentCameraServerRpc(ulong ownerId)
    // {
    //     
    // }
    // [ServerRpc(RequireOwnership = false)]
    // private void ParentWeaponHolderServerRpc(ulong ownerId)
    // {
    //     
    // }
    // [ServerRpc(RequireOwnership = false)]
    // private void ParentWeaponServerRpc(ulong ownerId)
    // {
    //     
    // }
    // [ServerRpc(RequireOwnership = false)]
    // private void ParentFpsCameraServerRpc(ulong ownerId)
    // {
    //     
    // }


}
