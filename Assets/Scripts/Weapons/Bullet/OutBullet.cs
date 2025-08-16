using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutBullet : MonoBehaviour
{
    public GameObject destroy; //子弹销毁动画
    public ParticleSystem particle; //碎屑粒子
    private SpriteRenderer rend;
    private bool start; //触地后开始销毁
    private float lifeTime; //存在时间

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(start) //触地后开始自动销毁
        {
            lifeTime += Time.deltaTime;
            if (lifeTime > 2f) //2s后开始闪烁，模的结果小于0.025时设置为隐藏，大于0.025时设置为显示，即为每0.025s进行隐藏与显示的交替
            {
                float remainder = lifeTime % 0.05f; //与0.05进行模运算
                rend.enabled = remainder > 0.025f;
            }
            if (lifeTime > 3f) //3s后自动销毁
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 pos = collision.GetContact(0).point;
        if (collision.gameObject.tag == "Enemy")
        {
            Instantiate(destroy, pos, transform.rotation); //生成一个销毁动画对象
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Ground")
        {
            start = true;
        }
    }
}
