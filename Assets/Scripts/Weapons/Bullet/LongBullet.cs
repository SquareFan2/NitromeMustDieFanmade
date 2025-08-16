using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongBullet : MonoBehaviour
{
    public LayerMask playerMask; //获取玩家层
    public LayerMask groundMask; //获取地面层
    public GameObject pistolDestroy; //手枪时的子弹销毁动画
    public GameObject MGDestroy; //机关枪时的子弹销毁动画
    public ParticleSystem particle; //碎屑粒子
    public ParticleSystem rubble; //瓦砾粒子
    public ParticleSystem rubbleCounter; //相反方向瓦砾粒子
    public float damage = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 pos = collision.GetContact(0).point;
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Enemy")
        {
            var des = GameManager.instance.currentWeapon == 1 ? pistolDestroy : MGDestroy;
            Instantiate(des, pos, transform.rotation); //根据当前武器是手枪/机关枪，生成一个销毁动画对象
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Ground")
        {
            GenerateRubble(collision, pos);
            var v = collision.relativeVelocity;
            if (v.x > 0) //生成向右的碎屑粒子
            {
                particle.transform.eulerAngles = new Vector3(-15f, 90f, 0);
                Instantiate(particle, pos, particle.transform.rotation);
            }
            else //生成向左的碎屑粒子
            {
                particle.transform.eulerAngles = new Vector3(-165f, 90f, 0);
                Instantiate(particle, pos, particle.transform.rotation);
            }
        }
    }

    void GenerateRubble(Collision2D collision, Vector2 pos) //生成瓦砾粒子效果
    {
        Vector2 speed = collision.relativeVelocity; //获取子弹碰撞初速度
        float xr = 0;
        float yr = 0;
        float ang;
        //通过子弹速度微调发射粒子的位置，防止粒子卡在墙内，并定义粒子发射器的角度
        if (speed.x != 0) xr = speed.x > 0 ? 0.2f : -0.2f;
        if (speed.y != 0) yr = speed.y > 0 ? 0.2f : -0.2f;
        if (speed.y != 0)
        {
            ang = speed.y > 0 ? 270f : 90f;
        }
        else ang = speed.x > 0 ? 0 : 180f;
        Vector2 posr = new Vector2(pos.x + xr, pos.y + yr);
        rubble.transform.position = posr;
        rubble.transform.eulerAngles = new Vector3(ang, 90f, 0); //旋转角度类
        if (speed.x > 0) Instantiate(rubble, rubble.transform.position, rubble.transform.rotation);
        else Instantiate(rubbleCounter, rubble.transform.position, rubble.transform.rotation);
    }
}
