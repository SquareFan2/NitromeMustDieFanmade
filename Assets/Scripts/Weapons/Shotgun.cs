using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shotgun : MonoBehaviour
{
    private GameManager GM;

    public GameObject player; //玩家
    public GameObject gun; //枪
    public Animator gunAnim; //枪动画控制器
    public GameObject shotgunBullet; //子弹
    public Transform firePoint; //子弹发射位置
    public float speed = 13.5f; //子弹飞行速度
    public Animator fireAnim; //开火逐帧动画控制器
    public GameObject shellCase; //空弹壳物件
    private GameObject weaponSwitcher; //武器管理部件，用于减少弹药等
    public float timer = 0; //开火计时器，计时器>=0时无法开火

    void Start()
    {
        GM = GameManager.instance;
        weaponSwitcher = transform.parent.gameObject; //获取父对象，即武器管理部件
    }

    void Update()
    {
        if (timer >= 0) //计时器计时
        {
            timer -= Time.deltaTime;
            if(GM.subspeed != 0) //后坐力持续0.1s，且递减
            {
                GM.subspeed = ((timer - 1.1f) / 0.1f) * 5f;
                if(GM.subspeed <= 0)
                {
                    GM.subspeed = 0;
                }
            }
        }
        if (EventSystem.current.IsPointerOverGameObject()) return; //如果正在点击UI，则屏蔽掉攻击操作
        if (Input.GetKeyDown(GameManager.instance.fireKey)) //按下开火键时，执行开火函数并播放音效
        {
            if (timer < 0) //计时器未重置时无法开火
            {
                AudioManager.instance.GunSound(AudioManager.instance.ShotgunFire); //播放音效
                Shoot();
            }
        }
    }

    void Shoot() //开火函数
    {
        timer = 1.2f;
        GM.subspeed = 5f; //后坐力
        //生成子弹
        for (int i = 0; i < 10; i++) //一次发射10个子弹
        {
            GameObject obj = Instantiate(shotgunBullet, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); //获取刚体

            var ry = Random.Range(-3.617f, 4.914f); //子弹飞行方向随机在垂直方向上偏移-15~25度，y速度绝对值最大为tan15°(20°) * speed（此处按13.5计算）
            var rx = Random.Range(speed - 0.8f, speed + 0.8f); //子弹水平速度随机浮动±0.5f
            rb.velocity = new Vector2(-player.transform.localScale.x * rx, ry); //赋予子弹速度
        }

        gunAnim.SetTrigger("Fire");
        fireAnim.SetTrigger("Fire");

        weaponSwitcher.SendMessage("ammoReduce", 1);

        Shellcase();
    }

    void Shellcase() //生成空弹壳
    {
        var playerScale = player.transform.localScale.x; //玩家面向
        Vector2 shellPos = new Vector2(gun.transform.position.x + 0.1875f * -playerScale, gun.transform.position.y + 0.1875f); //弹壳生成位置
        GameObject newShell = Instantiate(shellCase, shellPos, transform.rotation);
        Rigidbody2D rbShell = newShell.GetComponent<Rigidbody2D>();
        var R = Random.Range(9, 12) * 0.1f; //弹壳飞行矢量力的浮动范围
        Vector2 shellDir = new Vector2(player.transform.localScale.x * 0.5f * R, 0.75f * R); //弹壳飞行矢量力
        rbShell.AddForce(shellDir, ForceMode2D.Impulse); //弹壳移动力
        //两侧施加相反力，使弹壳旋转
        rbShell.AddForceAtPosition(new Vector2(0, 1f), new Vector2(shellPos.x - 0.09f, shellPos.y));
        rbShell.AddForceAtPosition(new Vector2(0, -1f), new Vector2(shellPos.x + 0.09f, shellPos.y));
    }
}
