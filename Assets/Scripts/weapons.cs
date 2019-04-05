using UnityEngine;
using UnityEngine.UI;   
using System;
using System.Collections.Generic;      
using Random = UnityEngine.Random;      


public class weapons : MonoBehaviour
{
  
    // Start is called before the first frame update
    public static int damage = 1;
    public static int range = 1;
    public static int attackDelay = 0;
    public static int weaponType = 0;
    public GameObject[] weapon;

     void Start()
    {
        initWeapon((int)(name[8] - '0'), (int)(name[10] - '0'));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void equipt(int t,int r){
        initWeapon(t,r);
    }


    public static void initWeapon(int t,int r){
        weaponType = t;
        range = r;
        damage = weaponType;
        attackDelay = weaponType - 1;    
    }


}
