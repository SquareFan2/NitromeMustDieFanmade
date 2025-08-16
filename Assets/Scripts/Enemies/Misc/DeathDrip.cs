using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathDrip : MonoBehaviour
{
    public Sprite drip1;
    public Sprite drip2;
    public Sprite drip3;
    public Sprite drip4;
    public Sprite drip5;
    public Sprite drip6;
    public Sprite drip7;
    public Sprite drip8;
    public Sprite drip9;
    public Sprite drip10;
    public Sprite drip11;
    public Sprite drip12;
    public Sprite drip13;
    public SpriteRenderer rend; //精灵渲染器
    private Rigidbody2D rb;
    private Color cl = new Color(1, 1, 1, 1); //死亡颜色
    private int r; //粒子外观随机数
    public ParticleSystem subdirp; //水滴接触墙面时的溅射效果粒子
    public GameObject splash;
    private bool v; //致死子弹来自左/右方向，true表示右，false表示左

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();

        r = Random.Range(1, 14); //通过设置精灵渲染器的精灵随机变化水滴的贴图
        rend.sprite = drip1;
        rend.color = cl;
        switch (r)
        {
            case 1: rend.sprite = drip1; break;
            case 2: rend.sprite = drip2; break;
            case 3: rend.sprite = drip3; break;
            case 4: rend.sprite = drip4; break;
            case 5: rend.sprite = drip5; break;
            case 6: rend.sprite = drip6; break;
            case 7: rend.sprite = drip7; break;
            case 8: rend.sprite = drip8; break;
            case 9: rend.sprite = drip9; break;
            case 10: rend.sprite = drip10; break;
            case 11: rend.sprite = drip11; break;
            case 12: rend.sprite = drip12; break;
            case 13: rend.sprite = drip13; break;
            default: break;
        }
        //设置向上飞的力
        float vx = v == true ? 0.3f : -0.3f; //向左/右偏移
        float rfx = Random.Range(-0.3f + vx, 0.3f + vx); //随机左右水平力和随机垂直向上力
        float rfy = Random.Range(0.8f, 1.2f);
        Vector2 f = new Vector2(rfx, rfy);
        rb.AddForce(f, ForceMode2D.Impulse);
    }

    void GetColor(Color cc) //接收敌人脚本传来的颜色参数
    {
        cl = cc;
    }
    void GetV(bool vv) //接收敌人脚本传来的速度左右方向情况
    {
        v = vv;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var pos = collision.GetContact(0).point;
        var v = collision.relativeVelocity; //获取碰撞时的相对线性速度
        var vx = Mathf.Abs(v.x); //速度绝对值
        var vy = Mathf.Abs(v.y);
        if (r > 5) //如果随机到的贴图够大，则生成次级水滴的粒子特效
        {
            float angP = 0; //粒子旋转角度，0为向右
            if(v.y > 0)
            {
                if(vy > vx) //向上
                {
                    angP = -90f;
                }
                else if(v.x < 0) //向左
                {
                    angP = 180f;
                }
                //否则向右，默认为0
            }
            else
            {
                if(vy > vx) //向下
                {
                    angP = 90f;
                }
                if (v.x < 0) //向左
                {
                    angP = 180f;
                }
                //否则向右，默认为0
            }
            subdirp.transform.eulerAngles = new Vector3(angP, 90f, 0); //设置粒子旋转角度
            var main = subdirp.main; //设置粒子颜色
            main.startColor = new ParticleSystem.MinMaxGradient(cl);
            Instantiate(subdirp, pos, subdirp.transform.rotation);
        }
        float ang = (180 / Mathf.PI * Mathf.Atan2(v.y, v.x)) - 90; //泼溅精灵的角度
        splash.GetComponent<SpriteRenderer>().color = cl; //设置泼溅精灵颜色
        splash.transform.eulerAngles = new Vector3(0, 0, ang); //设置泼溅精灵角度
        splash.transform.localScale = new Vector3(0.15f + (r / 100f), 0.15f + (r / 100f), 1); //设置泼溅精灵的大小
        //对泼溅生成的位置进行随机微调
        float posr = Random.Range(0, 0.3f);
        if (v.y > 0)
        {
            if (vy > vx) //向上
            {
                pos.y += posr;
            }
            else if (v.x < 0) //向左
            {
                pos.x -= posr;
            }
            else //否则向右
            {
                pos.x += posr;
            }
        }
        else
        {
            if (vy > vx) //向下
            {
                pos.y -= posr;
            }
            if (v.x < 0) //向左
            {
                pos.x -= posr;
            }
            else //否则向右
            {
                pos.x += posr;
            }
        }
        Instantiate(splash, pos, splash.transform.rotation);
        Destroy(gameObject);
    }
}
