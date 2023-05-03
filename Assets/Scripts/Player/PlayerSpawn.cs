using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : NetworkBehaviour
{
    public Vector3 spawnPosition;
    public override void OnNetworkSpawn()
    {
        transform.position = spawnPosition;
    }
}
