using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGrappleArmAnim : MonoBehaviour
{
    public GameObject _object;

    // Start is called before the first frame update
    void Start()
    {
        
        _object.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Grapple.isGrappling){
            _object.SetActive(true);
        }
        else{
            _object.SetActive(false);
        }
    }
}
