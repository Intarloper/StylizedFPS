using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;


public class PlayerCamera : NetworkBehaviour
{

    public float xSensitivity;
    public float ySensitivity;

    public Transform cameraOrientation;
    public Transform camHolder;
    

    float rotX;
    float rotY;
    
    private GameObject cam;
    private GameObject FpsCamera;

    private Camera cameraFPS;
    private Camera camera;
    public override void OnNetworkSpawn(){
        cam = this.gameObject;
        if(!IsOwner){
            cam.SetActive(false);
            
        }
        Invoke("Find", .6f);
        Invoke("CameraRefrences", .6f);
        Invoke("LayerFpsCamera", .6f);
        

        
    }

    // Start is called before the first frame update
    void Start()
    {
        

        

    }

    

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySensitivity;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -90f, 90f);

        camHolder.transform.rotation = Quaternion.Euler(rotX, rotY, 0);

        //playerOreint GameObject
        cameraOrientation.transform.rotation = Quaternion.Euler(0, rotY, 0);


        //Unlock Cursor from camera

        if(Input.GetKeyDown(KeyCode.Escape)){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LayerFpsCamera()
    {
        if(!IsOwner) return;
        
        FpsCamera = FindObjectOfType<SyncFPSCam>().gameObject;
        NetworkObject fpsCamNetworkObject = FpsCamera.GetComponent<NetworkObject>();
        cameraFPS = fpsCamNetworkObject.GetComponent<Camera>();

        var cameraData = camera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(cameraFPS);
    }

    void CameraRefrences()
    {
        if(!IsOwner) return;
        
        camHolder = transform.GetComponentInParent<Transform>();
        cameraOrientation = GameObject.FindObjectOfType<OrientFinder>().transform;
    }

    void Find()
    {
        if(!IsOwner) return;
        
        cam = this.gameObject;
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
}
