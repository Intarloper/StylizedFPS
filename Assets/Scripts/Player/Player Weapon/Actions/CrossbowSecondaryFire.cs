using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowSecondaryFire : MonoBehaviour
{
    void OnEnable(){
        PlayerAction.onSecondaryfire += Ability;
    }

    void Ability(){
        print("Secondary Fire Log Message");
    }

    void OnDisable(){
        PlayerAction.onSecondaryfire -= Ability;
    }
}
