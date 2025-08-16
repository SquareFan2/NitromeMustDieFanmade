using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollideDictionary : MonoBehaviour
{
    //定义字典结构
    public Dictionary<string, int> enemyCollideDictionay = new Dictionary<string, int>();
    void Awake() //将敌人名称与碰撞伤害值存入字典中
    {
        enemyCollideDictionay.Add("OrangeNose(Clone)", 4);
        foreach (KeyValuePair<string, int> pair in enemyCollideDictionay)
        {
            Debug.Log(pair.Key + " " + pair.Value);
        }
    }

    //通过键值查找伤害值
    public int Value(string s)
    {
        return enemyCollideDictionay[s];
    }
}
