using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CheckOwnership : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitToChange());

    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeOwnerServerRpc()
    {
        if (!IsOwner)
        {
            NetworkObject networkObject= GetComponent<NetworkObject>();
            ulong clientId = networkObject.OwnerClientId;
            networkObject.ChangeOwnership(clientId);
            print("Owner?" + IsOwner);
        }
    }

    IEnumerator WaitToChange()
    {
        yield return new WaitForSeconds(.2f);
        ChangeOwnerServerRpc();
    }
}
