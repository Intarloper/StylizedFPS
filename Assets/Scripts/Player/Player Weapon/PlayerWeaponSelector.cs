using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerWeaponSelector : NetworkBehaviour
{
    [SerializeField] private WeaponType _weapon;
    [SerializeField] private GameObject weaponHolderGO;
    [SerializeField] private List<WeaponSO> weapons;
    
    public WeaponSO ActiveWeapon;

    

    public void SetupWeapon()
    {
        if(!IsOwner) return;
        
        print("Spawning Weapon");
        WeaponSO weaponSO = weapons.Find(weapon => weapon.weaponType == _weapon);

        if(weaponSO == null){
            print("WeaponSO null");
        }
        ActiveWeapon = weaponSO;
        
        //due to netcode restrictions, we must instantiate a weaponholder prefab with a network object attached to it
        //then parent that to the player, as NetwrokObjects cant be parented to other NetworkObjects at runtime.
        //We instantiate it at the position of a temporary weaponHolder empty transform, then afterwards parent the crossbow
        //to the weaponHolder prefab
        
        
        
    }

    

    
}
