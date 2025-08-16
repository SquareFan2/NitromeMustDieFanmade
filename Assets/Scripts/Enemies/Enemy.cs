using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    protected AudioManager AM; //音效管理器
    public AssetBundle ab; //material AB包

    [Header("移动参数")]
    public float speed; //移动速度，手动在inspector中指定
    public float face = -1f; //移动方向，-1为向左，1为向右
    public float faceDir = -1f; //面向方向flag
    public int getHurt = 0; //受伤时速度为0的flag
    protected Vector2 pos; //此对象当前位置
    [Header("环境检测")]
    public bool isGround; //是否站在地面上
    public Vector2 offset; //地面检测射线离中央的距离
    public Vector2 offsetWall; //墙壁检测射线离底边的距离
    public LayerMask ground; //获取地面层
    public LayerMask platform; //获取平台层
    public LayerMask stand; //获取站立层（平台层+地面层）
    public LayerMask playerMask; //获取玩家层
    protected RaycastHit2D leftCheckGround; //地面左射线
    protected RaycastHit2D rightCheckGround; //地面右射线
    protected float lenWall; //墙壁射线长度
    protected RaycastHit2D leftCheckWall; //墙壁左射线
    protected RaycastHit2D rightCheckWall; //墙壁右射线
    [Header("敌人参数")]
    public string n; //受击的子弹名称
    public int hp; //血量
    protected int toPlayerDamage; //对玩家造成的伤害
    protected BulletDictionary bulletDictionary; //子弹伤害字典
    [Header("发光效果")]
    public Sprite damageSprite; //受伤状态贴图
    protected Material matLight; //材质，用于受伤发光效果
    protected Material matDefault; //默认材质
    protected Texture tex; //纹理，每帧都获取
    protected SpriteRenderer rend; //当前物体的渲染器
    [Header("死亡效果")]
    protected GameObject destroy1; //死亡效果1prefab
    protected GameObject coinGenerator; //金币生成器
    protected Color cl; //死亡液体颜色
    protected int dripCount; //死亡时溅出的水滴数量（浮动线）
    protected int coinCount; //死亡时掉落金币数量
    protected bool deathFlag = false; //死亡flag，防止重复执行死亡函数

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        bulletDictionary = gameObject.AddComponent<BulletDictionary>(); //实例化子弹伤害字典
    }

    protected virtual void Start()
    {
        AM = AudioManager.instance;
        ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "enemy"); //加载AB包

        //层赋值
        ground = LayerMask.GetMask("Ground");
        platform = LayerMask.GetMask("Platform_Other");
        stand = LayerMask.GetMask("Ground", "Platform_Other");
        playerMask = LayerMask.GetMask("Player");

        //加载AB包资源
        matLight = (Material)ab.LoadAsset("LightMat");
        matDefault = ab.LoadAsset<Material>("Sprites_Custom");
        destroy1 = ab.LoadAsset<GameObject>("Enemy_Destroy_1");
        coinGenerator = ab.LoadAsset<GameObject>("CoinGenerator");
        AssetBundle.UnloadAllAssetBundles(false); //卸载AB包 如果变量填true则会把已加载出来的也卸载

        tex = damageSprite.texture; //获取受伤贴图的纹理，用于受伤发光效果
        matLight.SetTexture("_TargetTex", tex); //将纹理赋值给材质
    }

    void FixedUpdate()
    {
        Movement(); //基本移动
        GroundCheck(); //地面判定
        WallCheck(); //墙壁判定
    }
    protected virtual void Movement()
    {
        pos = transform.position;

        transform.localScale = new Vector2(-faceDir, 1); //调整面向

        //受伤判定
        if (getHurt > 0) //受伤时有4帧速度为0
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            getHurt--;
        }
    }

    protected virtual void GroundCheck() //地面判定
    {
        leftCheckGround = Physics2D.Raycast(pos - offset, Vector2.down, 0.2f, stand); //左射线
        rightCheckGround = Physics2D.Raycast(pos + offset, Vector2.down, 0.2f, stand); //右射线
        Debug.DrawRay(pos - offset, Vector2.down, Color.red, 0.01f);
        Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.01f);
        if (leftCheckGround || rightCheckGround) //地面物理判定
        {
            isGround = true;
            anim.SetBool("isGround", true);
            if(leftCheckGround && rightCheckGround)
            {
                if (leftCheckGround.collider.CompareTag("Pipe") && leftCheckGround.collider.CompareTag("Pipe")) //左右射线都触碰管道，代表怪物位于管道正上方，执行传送
                {
                    Debug.Log("Pipe");
                    PipeInteractionDown();
                }
            }
        }
        else
        {
            isGround = false;
            anim.SetBool("isGround", false);
        }
    }

    protected virtual void WallCheck() //墙壁判定
    {
        leftCheckWall = Physics2D.Raycast(pos + offsetWall, Vector2.left, lenWall, ground); //左射线
        rightCheckWall = Physics2D.Raycast(pos + offsetWall, Vector2.right, lenWall, ground); //右射线
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
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            getHurt = 4; //受伤时有4帧速度为0
            n = collision.gameObject.name; //获取撞击的子弹的名称
            if (n != "ShotgunBullet(Clone)") AM.EnemySound(AM.getBullet); //除部分子弹外播放音效
            int damage = bulletDictionary.Value(n); //通过子弹名称获取子弹的伤害值
            anim.SetTrigger("Hurt");
            hp -= damage;
            if (hp <= 0)
            {
                var v = collision.relativeVelocity.x > 0; //判断攻击来自的方向（只有左右）
                Death(v);
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.SendMessage("GetDamage", toPlayerDamage); //给玩家造成伤害
            hp -= 5; //被玩家撞到时减少5点hp
            anim.SetTrigger("Hurt");
            if (hp <= 0)
            {
                var v = collision.relativeVelocity.x > 0; //判断攻击来自的方向（只有左右）
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

    protected virtual void PipeInteractionDown() //向下传送进入管道
    {
        GameObject D = new GameObject(); //生成掉落时的贴图
        GameObject d = Instantiate(D, pos, transform.rotation);
        d.AddComponent<SpriteRenderer>();
        d.GetComponent<SpriteRenderer>().sprite = damageSprite;
        d.GetComponent<SpriteRenderer>().sortingLayerName = "Enemy";
        if(faceDir == 1) //根据面向调整贴图方向
        {
            d.transform.localScale = new Vector2(-1, 1);
        }
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
