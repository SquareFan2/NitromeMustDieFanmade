using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAppear : MonoBehaviour
{
    public GameObject hpChest;
    public GameObject ammoChest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Generate(bool b) //true生成医药箱，false生成武器箱
    {
        if (b)
        {
            GenerateHP();
        }
        else
        {
            GenerateAmmo();
        }
    }

    void GenerateHP()
    {
        Instantiate(hpChest, transform.position, hpChest.transform.rotation);
    }

    void GenerateAmmo()
    {
        //Instantiate(ammoChest, transform.position, ammoChest.transform.rotation);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
