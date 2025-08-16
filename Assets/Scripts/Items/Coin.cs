using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float lifeTime; //硬币存在时间
    private SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lifeTime += Time.deltaTime;
        if(lifeTime > 8f) //8s后开始闪烁，模的结果小于0.025时设置为隐藏，大于0.025时设置为显示，即为每0.025s进行隐藏与显示的交替
        {
            float remainder = lifeTime % 0.05f; //与0.05进行模运算
            rend.enabled = remainder > 0.025f;
        }
        if(lifeTime > 10f) //10s后自动销毁
        {
            Destroy(gameObject);
        }
    }
}
