using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Weapon", order = 1)]
public class WeaponSO : ScriptableObject
{
    public WeaponType weaponType;
    
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRot;

    public ShootConfigSO shootConfigSO;

    private NetworkBehaviour activeNetworkBehaviour;
    private GameObject model;
    



    public void Spawn(Transform Parent, NetworkBehaviour activeNetworkBehaviour, ulong ownerId)
    {
        this.activeNetworkBehaviour = activeNetworkBehaviour;
        

        //Model Setup
        model = Instantiate(modelPrefab);
        model.GetComponent<NetworkObject>().SpawnWithOwnership(ownerId);
        
        
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRot);
        model.transform.SetParent(Parent, false);

        
            
        
        
    }
    


}
