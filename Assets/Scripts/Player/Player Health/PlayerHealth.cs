using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<int> playerHealth = new NetworkVariable<int>();
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHealth.Value = 10;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;
        
        if (collision.gameObject.GetComponent<Projectile>())
        {
            playerHealth.Value -= 10;

            if (playerHealth.Value <= 0)
            {
                DespawnPlayerServerRpc();
            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    void DespawnPlayerServerRpc()
    {
        transform.position = Vector3.zero;
        playerHealth.Value = 10;
        // NetworkObject[] networkObjects = GetComponentsInChildren<NetworkObject>();
        //
        // foreach (NetworkObject no in networkObjects)
        // {
        //     // print(no);
        //     NetworkObject.Despawn();
        //    
        //     
        // }
        // NetworkObject.Despawn();

    }
}
