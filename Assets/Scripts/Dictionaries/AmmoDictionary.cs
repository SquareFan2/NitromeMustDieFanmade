using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDictionary : MonoBehaviour
{
    //定义字典结构
    public Dictionary<int, int> ammoDictionary = new Dictionary<int, int>();
    void Awake() //将武器与最大弹药数存入字典中
    {
        ammoDictionary.Add(0, 0);
        ammoDictionary.Add(1, 70);
        ammoDictionary.Add(2, 150);
        ammoDictionary.Add(3, 25);
        /*foreach (KeyValuePair<string, int> pair in ammoDictionary)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }*/
    }

    //通过键值查找伤害值
    public int Value(int i)
    {
        return ammoDictionary[i];
    }
}
