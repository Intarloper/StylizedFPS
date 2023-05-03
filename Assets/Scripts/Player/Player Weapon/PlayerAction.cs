using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[DisallowMultipleComponent]
public class PlayerAction : NetworkBehaviour
{

    private Transform cam;
    [SerializeField]private GameObject playerCamGO;
    [SerializeField]private Transform weaponProjSpawn;
    [SerializeField] private Vector3 projectileOffset;
    GameObject projectile;
    Rigidbody rb;

    public Vector3 forceToAdd;
    bool primaryFireTriggered;

    public NetworkVariable<bool> _primaryFireTriggered = new NetworkVariable<bool>();
    private bool _primaryTrigger;

    float fireRateTimer;
    float secondaryCooldownTimer;
    
    public delegate void OnPrimaryFire();
    public static event OnPrimaryFire onPrimaryFire;

    public delegate void OnSecondaryFire();
    public static event OnSecondaryFire onSecondaryfire;


    [SerializeField] private PlayerWeaponSelector weaponSelector;
    
    void Start() {
        if(!IsOwner) return;
        fireRateTimer = weaponSelector.ActiveWeapon.shootConfigSO.fireRate;
        secondaryCooldownTimer = weaponSelector.ActiveWeapon.shootConfigSO.secondaryFireCD;

        
    }

    public override void OnNetworkSpawn()
    {
        
        StartCoroutine(WaitForFind());
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        fireRateTimer += Time.deltaTime;
        secondaryCooldownTimer += Time.deltaTime;

        //event caller for primary fire
        if(Input.GetMouseButtonDown(0) && weaponSelector.ActiveWeapon != null){
            if(fireRateTimer > weaponSelector.ActiveWeapon.shootConfigSO.fireRate){
                
                    
                RequestProjectileSpawn();
                print(_primaryTrigger);
                _primaryTrigger = false;
                
                fireRateTimer = 0;
            }
            
        }
        //event caller for secondary fire
        if(Input.GetMouseButtonDown(1) && weaponSelector.ActiveWeapon != null){
            if(secondaryCooldownTimer > weaponSelector.ActiveWeapon.shootConfigSO.secondaryFireCD){
                if(onSecondaryfire != null){
                    onSecondaryfire();
                }
                secondaryCooldownTimer = 0;
            }
        }
    }

    void RequestProjectileSpawn()
    {
        print("Requested Projectile Spawn");
        Vector3 pos = cam.transform.position;
        Quaternion rot = cam.transform.rotation;
        SpawnProjectileServerRpc(weaponProjSpawn.position,rot);
    }
    
    
    //only works with host for now NEED TO FIX
    [ServerRpc (RequireOwnership = false)]
    void SpawnProjectileServerRpc(Vector3 pos, Quaternion rot)
    {
        _primaryTrigger = true;
        print("Got into spawn projectile RPC");
        //Instantiate and grab networkObject to sync
        GameObject projectile = Instantiate(weaponSelector.ActiveWeapon.shootConfigSO.projectile, pos, rot);
        NetworkObject projectileNetworkObject = projectile.GetComponent<NetworkObject>();
        projectileNetworkObject.Spawn();
        if (projectileNetworkObject.TryGetComponent(out Rigidbody rb))
        {
            //Calculate Force Serverside
            Vector3 force = rot * Vector3.forward * weaponSelector.ActiveWeapon.shootConfigSO.projectileSpeed
                            + transform.up * weaponSelector.ActiveWeapon.shootConfigSO.upwardProjectileSpeed;
            
            rb.AddForce(force,ForceMode.Impulse);
            
        }
        // rb = projectileTransform.GetComponent<Rigidbody>();
        
        
    }
    
    
    
    

    void OnDisable(){
        
    }
    
    
    IEnumerator WaitForFind()
    {
        yield return new WaitForSeconds(.6f);
        Find();

    }

    private void Find()
    {
        if(!IsOwner) return;
        
        playerCamGO = GetComponentInChildren<PlayerCamera>().gameObject;
        cam = playerCamGO.GetComponent<Transform>();

        weaponProjSpawn = GetComponentInChildren<SetWeaponLocalPosition>().transform;
    }
}
