using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeNoseTest : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioManager AM; //音效管理器

    [Header("移动参数")]
    public float speed; //移动速度
    public float face; //移动方向，-1为向左，1为向右
    public float faceDir = -1f; //面向方向flag
    public int getHurt = 0; //受伤时速度为0的flag
    [Header("环境检测")]
    public bool isGround; //是否站在地面上
    public LayerMask ground; //获取地面层
    public LayerMask platform; //获取平台层
    public LayerMask stand; //获取站立层（平台层+地面层）
    public LayerMask playerMask; //获取玩家层
    [Header("敌人参数")]
    public string n; //受击的子弹名称
    private int hp = 50; //血量
    private BulletDictionary bulletDictionary; //子弹伤害字典
    [Header("发光效果")]
    public SpriteRenderer rend; //当前物体的渲染器
    public Material matLight; //材质，用于受伤发光效果
    public Sprite damageSprite; //受伤状态贴图
    public Material matDefault; //默认材质
    private Texture tex; //纹理，每帧都获取
    [Header("死亡效果")]
    public GameObject destroy1; //死亡效果1prefab
    public GameObject coinGenerator; //金币生成器
    private Color cl = new Color(0.952f, 0.4f, 0, 1); //死亡液体颜色
    private int dripCount = 5; //死亡时溅出的水滴数量（浮动线）
    private int coinCount = 3; //死亡时掉落金币数量
    private bool deathFlag = false; //死亡flag，防止重复执行死亡函数

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bulletDictionary = gameObject.AddComponent<BulletDictionary>(); //实例化子弹伤害字典
    }
    void Start()
    {
        AM = AudioManager.instance;
        tex = damageSprite.texture; //获取受伤贴图的纹理，用于受伤发光效果
        matLight.SetTexture("_TargetTex", tex); //将纹理赋值给材质
    }

    void FixedUpdate()
    {
        Movement();
    }
    void Movement()
    {
        Vector2 pos = transform.position;
        Vector2 offset = new Vector2(0.3125f, 0f);
        RaycastHit2D leftCheckGround = Physics2D.Raycast(pos - offset, Vector2.down, 0.2f, stand); //左射线
        RaycastHit2D rightCheckGround = Physics2D.Raycast(pos + offset, Vector2.down, 0.2f, stand); //右射线
        //Debug.DrawRay(pos - offset, Vector2.down, Color.red, 0.2f);
        //Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);
        if (leftCheckGround || rightCheckGround) //地面物理判定
        {
            isGround = true;
            anim.SetBool("isGround", true);
            if (!leftCheckGround) //当左或右射线没有判定时，证明行走至平台边界，调转方向
            {
                faceDir = 1f;
            }
            else if (!rightCheckGround)
            {
                faceDir = -1f;
            }
        }
        else
        {
            isGround = false;
            anim.SetBool("isGround", false);
        }

        if(face < faceDir) //转身时速度渐变
        {
            face += Time.deltaTime * 3.5f;
        }
        else if(face > faceDir)
        {
            face -= Time.deltaTime * 3.5f;
        }

        //墙壁判定
        Vector2 offsetWall = new Vector2(0, 0.5f); //Y轴适当上移
        RaycastHit2D leftCheckWall = Physics2D.Raycast(pos + offsetWall, Vector2.left, 0.4f, ground); //左射线
        RaycastHit2D rightCheckWall = Physics2D.Raycast(pos + offsetWall, Vector2.right, 0.4f, ground); //右射线
        if (leftCheckWall)
        {
            face = 1f;
            faceDir = 1f;
        }
        if (rightCheckWall)
        {
            face = -1f;
            faceDir = -1f;
        }

        if (getHurt > 0) //受伤时有4帧速度为0
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            getHurt--;
        }

        if (isGround && getHurt == 0) //施加速度
        {
            rb.velocity = new Vector2(face * speed, rb.velocity.y);
        }

        transform.localScale = new Vector2(-faceDir, 1); //调整面向
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            getHurt = 4; //受伤时有4帧速度为0
            n = collision.gameObject.name; //获取撞击的子弹的名称
            if(n != "ShotgunBullet(Clone)") AM.EnemySound(AM.getBullet); //除部分子弹外播放音效
            int damage = bulletDictionary.Value(n); //通过子弹名称获取子弹的伤害值
            anim.SetTrigger("Hurt");
            hp -= damage;
            if (hp <= 0)
            {
                var v = collision.relativeVelocity.x > 0 ? true : false; //判断攻击来自的方向（只有左右）
                Death(v);
            }
        }
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("GetDamage", 2); //给玩家造成伤害
            hp -= 5; //被玩家撞到时减少5点hp
            anim.SetTrigger("Hurt");
            if (hp <= 0)
            {
                var v = collision.relativeVelocity.x > 0 ? true : false; //判断攻击来自的方向（只有左右）
                Death(v);
            }
        }
    }

    void Death(bool v)
    {
        if (!deathFlag) //防止重复执行死亡函数
        {
            deathFlag = true;
        }
        else
        {
            return;
        }
        AM.EnemySound(AM.enemyDeath); //播放死亡音效
        var pos = new Vector2(transform.position.x, transform.position.y + 0.46875f);
        GameObject death = Instantiate(destroy1, pos, transform.rotation); //生成死亡效果
        death.SendMessage("GetColor", cl); //将当前敌人的颜色传给死亡效果的脚本
        death.SendMessage("GetCount", dripCount); //将当前敌人的水滴数量传给死亡效果的脚本
        death.SendMessage("GetV", v); //将攻击来自的方向传给死亡效果的脚本
        GameObject coin = Instantiate(coinGenerator, pos, transform.rotation); //生成金币生成器
        coin.SendMessage("CoinGenerate", coinCount); //将当前敌人的金币掉落数量传给金币生成器
        GameManager.instance.existTotal--; //减少剩余敌人数量
        Destroy(gameObject);
    }

    void LightOn()
    {
        rend.material = matLight; //将物体的材质设置为shader脚本渲染的发光材质

    }

    void LightOff()
    {
        rend.material = matDefault; //将物体的材质还原为Sprite Default
    }
}