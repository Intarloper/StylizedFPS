using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SetWeaponLocalPosition : MonoBehaviour
{
    [SerializeField] private Vector3 weaponPosition;
    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = weaponPosition;
      
    }

   

    

}
