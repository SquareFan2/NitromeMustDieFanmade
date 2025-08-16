using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDictionary : MonoBehaviour
{
    //定义字典结构
    public Dictionary<string, int> bulletDictionary = new Dictionary<string, int>();
    void Awake() //将子弹与伤害值存入字典中
    {
        bulletDictionary.Add("LongBullet(Clone)", 10);
        bulletDictionary.Add("OutBullet(Clone)", 5);
        bulletDictionary.Add("ShotgunBullet(Clone)", 10);
        /*foreach (KeyValuePair<string, int> pair in bulletDictionary)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }*/
    }

    //通过键值查找伤害值
    public int Value(string s)
    {
        return bulletDictionary[s];
    }
}
