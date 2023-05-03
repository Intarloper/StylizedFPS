using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    // [SerializeField] private Button server;
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    [SerializeField] private Button online;
    private bool connected;

    private void Awake() {
        // server.onClick.AddListener(() => {
        //     NetworkManager.Singleton.StartServer();
        // });
        host.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            connected = true;
        });
        client.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            connected = true;
        });
    }

    private void Update()
    {
        if (MatchmakeQuick.isLobbyConnectedUI == true)
        {
            host.gameObject.SetActive(false);
            client.gameObject.SetActive(false);
            online.gameObject.SetActive(false);
        }

        if (connected)
        {
            host.gameObject.SetActive(false);
            client.gameObject.SetActive(false);
            online.gameObject.SetActive(false);
        }
    }
}
