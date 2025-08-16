using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MachineGun : MonoBehaviour
{
    private GameManager GM;

    public GameObject player; //玩家
    public GameObject gun; //枪
    public Animator gunAnim; //枪动画控制器
    public GameObject longBullet; //子弹
    public Transform firePoint; //子弹发射位置
    public float speed = 13.5f; //子弹飞行速度
    public Animator fireAnim; //开火逐帧动画控制器
    public GameObject shellCase; //空弹壳物件
    private GameObject weaponSwitcher; //武器管理部件，用于减少弹药等

    private Coroutine coroutine; //开启的协程

    void Start()
    {
        GM = GameManager.instance;
        weaponSwitcher = transform.parent.gameObject; //获取父对象，即武器管理部件
        gunAnim.SetBool("ConstantFire", false); //防止动画触发器卡在true
        fireAnim.SetBool("Fire", false);
    }

    void Update()
    {
        if (Input.GetKeyDown(GM.fireKey)) //按下开火键时，开启协程持续开火，激活动画并设置后坐力
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //如果正在点击UI，则屏蔽掉攻击操作
            gunAnim.SetBool("ConstantFire", true);
            fireAnim.SetBool("Fire",true);
            coroutine = StartCoroutine(Cor());
            GM.subspeed = 1.9f;
        }
        if (Input.GetKeyUp(GM.fireKey)) //松开开火键时，停止开火
        {
            gunAnim.SetBool("ConstantFire", false);
            fireAnim.SetBool("Fire", false);
            if(coroutine != null) StopCoroutine(coroutine);
            GM.subspeed = 0;
        }
    }

    void Shoot() //开火函数
    {
        AudioManager.instance.GunSound(AudioManager.instance.machineGunFire); //播放音效
        //生成子弹
        GameObject obj = Instantiate(longBullet, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); //获取刚体

        var r = Random.Range(-0.236f, 1.419f); //子弹飞行方向随机在垂直方向上偏移-1~6度，y速度绝对值最大为tan1°(6°) * speed（此处按13.5计算）
        rb.velocity = new Vector2(-player.transform.localScale.x * speed, r); //赋予子弹速度

        weaponSwitcher.SendMessage("ammoReduce", 1);

        Shellcase();
    }

    IEnumerator Cor()
    {
        while (true)
        {
            Shoot();

            yield return new WaitForSeconds(0.12f);
        }
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
