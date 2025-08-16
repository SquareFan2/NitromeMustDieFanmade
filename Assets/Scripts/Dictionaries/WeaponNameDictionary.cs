using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponNameDictionary : MonoBehaviour
{
    //定义字典结构
    public Dictionary<int, string> weaponNameDictionary = new Dictionary<int, string>();
    void Awake() //将武器与名称数存入字典中
    {
        weaponNameDictionary.Add(0, "OUT OF AMMO");
        weaponNameDictionary.Add(1, "PISTOL");
        weaponNameDictionary.Add(2, "MACHINE GUN");
        weaponNameDictionary.Add(3, "SHOTGUN");
        /*foreach (KeyValuePair<string, int> pair in ammoDictionary)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }*/
    }

    //通过键值查找伤害值
    public string Value(int i)
    {
        return weaponNameDictionary[i];
    }
}
