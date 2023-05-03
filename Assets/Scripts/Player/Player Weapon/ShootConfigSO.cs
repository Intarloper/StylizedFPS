using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Shoot Configuration", menuName = "Weapons/Shoot Config", order = 2)]
public class ShootConfigSO : ScriptableObject
{
    
    public float fireRate;
    public float secondaryFireCD;
    public float projectileSpeed;
    public float upwardProjectileSpeed;
    public GameObject projectile;
    
    

}
