using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pistol : MonoBehaviour
{
    public GameObject player; //玩家
    public GameObject gun; //枪
    public Animator gunAnim; //枪动画控制器
    public GameObject longBullet; //子弹
    public Transform firePoint; //子弹发射位置
    public float speed = 18f; //子弹飞行速度
    public Animator anim3; //开火逐帧动画控制器
    public GameObject shellCase; //空弹壳物件
    private GameObject weaponSwitcher; //武器管理部件，用于减少弹药等
    //private int index = 0; //生成的子弹编号
    //public BulletArray<GameObject> bArray = new BulletArray<GameObject>(40); //实例化容器
    void Awake()
    {
        weaponSwitcher = transform.parent.gameObject; //获取父对象，即武器管理部件
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; //如果正在点击UI，则屏蔽掉攻击操作
        if (Input.GetKeyDown(GameManager.instance.fireKey)) //按下开火键时，执行开火函数并播放音效
        {
            AudioManager.instance.GunSound(AudioManager.instance.pistolFire); //播放音效
            Shoot();
        }
    }

    void Shoot() //开火函数
    {
        //生成子弹/*并加入容器队列*/
        GameObject obj = Instantiate(longBullet, firePoint.position, firePoint.rotation);
        //bArray.setItem(index, obj);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>(); //获取刚体
        rb.velocity = new Vector2(-player.transform.localScale.x * speed, 0); //赋予子弹速度

        gunAnim.SetTrigger("Fire");
        anim3.SetTrigger("Fire");

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

    public class BulletArray<T>
    {
        private T[] array;
        public BulletArray(int size)
        {
            array = new T[size + 1];
        }
        public T getItem(int index)
        {
            return array[index];
        }
        public void setItem(int index, T value)
        {
            array[index] = value;
        }
    }
}
