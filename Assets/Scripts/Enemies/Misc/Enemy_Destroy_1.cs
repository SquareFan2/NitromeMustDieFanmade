using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Destroy_1 : MonoBehaviour
{
    //public Material deathMat; //死亡效果材质
    public SpriteRenderer rend; //渲染器，用于改变颜色
    public GameObject deathDrip; //水滴
    private Color cl = new Color (1,1,1,1); //死亡颜色
    private int ct; //生成水滴数量
    private bool v; //致死子弹来自左/右方向，true表示右，false表示左

    void Start()
    {
        while (ct > 0) //生成复数个水滴
        {
            float r = Random.Range(-0.1f, 0.1f);
            var pos = new Vector2(transform.position.x + r, transform.position.y);
            GameObject drip = Instantiate(deathDrip, pos, transform.rotation); //生成死亡效果
            drip.SendMessage("GetColor", cl); //将当前敌人的颜色传给水滴的脚本
            drip.SendMessage("GetV", v); //将攻击来自的方向传给水滴的脚本
            ct--;
        }
    }

    void GetColor(Color cc) //接收敌人脚本传来的颜色参数
    {
        cl = cc;
    }

    void GetCount(int c) //接收敌人脚本传来的水滴数量参数，并随机化
    {
        ct = Random.Range(c, c+3);
    }
    void GetV(bool vv) //接收敌人脚本传来的速度左右方向情况
    {
        v = vv;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    void ChangeColor()//保证第一帧不被染色
    {
        rend.color = cl;
    }
}
