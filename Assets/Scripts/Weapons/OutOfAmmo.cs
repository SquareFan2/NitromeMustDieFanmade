using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfAmmo : MonoBehaviour
{
    public GameObject player; //玩家
    public GameObject gun; //枪
    public Animator gunAnim; //枪动画控制器
    public GameObject outBullet; //子弹
    public Transform firePoint; //子弹发射位置
    public float speed = 7.2f; //子弹飞行速度
    public Animator fireAnim; //开火逐帧动画控制器

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) //按下开火键时，执行开火函数
        {
            Shoot();
        }
    }

    void Shoot() //开火函数
    {
        AudioManager.instance.GunSound(AudioManager.instance.outOfAmmoFire); //播放音效
        //生成子弹
        GameObject obj = Instantiate(outBullet, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); //获取刚体
        rb.velocity = new Vector2(-player.transform.localScale.x * speed, 0); //赋予子弹速度

        gunAnim.SetTrigger("Fire");
        fireAnim.SetTrigger("Fire");
    }
}
