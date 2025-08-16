using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Cinemachine.CinemachineImpulseSource impulse; //镜头振动源
    private GameManager GM;
    private AudioManager AM;

    [Header("移动参数")]
    public float speed = 8f;
    public bool canCtrl = true; //角色是否可控
    private int faceDir = -1; //角色面向，-1为向左，1为向右
    private float horiAxis1 = 0; //向左水平输入值，用于平滑移动
    private float horiAxis2 = 0; //向右水平输入值，用于平滑移动
    private bool lastAxis = false; //上一个松开的水平移动按键，false为左，true为右
    [Header("跳跃参数")]
    public float jumpForce; //跳跃力
    private float downForce = 5.0f; //下降力，提前松开跳跃键时施加
    public float jumpHoldDuration = 0; //跳跃时间计时器
    public bool jumpStart = false; //判断是否人为起跳
    public float canJump = 0.1f;//提前跳跃计时器，在落地前的canJump秒内长按跳跃键可跳跃，优化手感
    [Header("环境检测")]
    public bool isGround; //是否站在地面上
    public LayerMask ground; //获取地面层
    public LayerMask platform; //获取平台层
    public LayerMask stand; //获取站立层（平台层+地面层）
    public LayerMask playerMask; //获取玩家层
    private float xVelocity;
    private float xVelocityRaw;
    public float rxVelocity;
    private float groundDistance = 0.2f; //地面距离
    [Header("粒子")]
    public ParticleSystem stopParticle; //刹车粒子发射器
    private float spDis = 0; //刹车粒子发射器偏移量
    private ParticleSystem newParticle;
    private float xScale; //用于监测面向是否发生变化，变化时立即删除刹车粒子
    public ParticleSystem walkParticle; //走路粒子发射器
    private float walkDis = 0; //监视走路距离，每走固定距离创建一个走路粒子发射器
    private float walkPos = 0; //每帧记录走路距离的变量
    [Header("受伤")]
    public GameObject hurtEffect; //受伤效果动画
    public SpriteRenderer playerRend; //玩家渲染器
    public SpriteRenderer gunRend; //枪渲染器
    private bool isInvincible; //判断是否处于无敌状态的flag
    private float timeSpentInvincible = 0; //无敌时间计数器
    private float hv; //受伤时击退的速度
    [Header("死亡")]
    public GameObject death; //死亡预制体
    public GameObject deathGun; //死亡时掉落的枪预制体
    [Header("血条")]
    private Animator healthbarAnim; //血条动画控制器，血量低时切换动画
    [Header("物品")]
    public GameObject coinCollected; //硬币收集动画
    private WeaponSwitching weaponSwitching; //切换武器脚本
    private Text scoreText; //分数UI
    private int scoreTemp = 0; //临时计数器，用于设置分数数字的过渡动画

    //按键设置
    private bool jumpHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); //获取动画
        impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        healthbarAnim = GameObject.Find("HealthBar").GetComponent<Animator>(); //获取血条动画控制器
        scoreText = GameObject.Find("Score").GetComponent<Text>(); //获取分数UI
        weaponSwitching = transform.Find("Firepoint").GetComponent<WeaponSwitching>(); //获取开火点的切换武器脚本
    }

    void Start()
    {
        GM = GameManager.instance;
        AM = AudioManager.instance;
    }

    void Update()
    {
        jumpHeld = Input.GetKey(GameManager.instance.jumpKey);
        PhysicsCheck();
        WalkParticle();
    }

    private void FixedUpdate()
    {
        HorizontalAxis();
        SwitchAnim();
        if (canCtrl) GroundMovement(); //只有可控flag为真时才执行移动函数
        FlipDirection();
        HurtFlash();
    }

    void PhysicsCheck() //地面判定
    {
        Vector2 pos = transform.position;
        Vector2 offset = new Vector2(0.25f, 0f);
        RaycastHit2D leftCheckGround = Physics2D.Raycast(pos - offset, Vector2.down, groundDistance, stand); //左射线
        RaycastHit2D rightCheckGround = Physics2D.Raycast(pos + offset, Vector2.down, groundDistance, stand); //右射线
        //Debug.DrawRay(pos - offset, Vector2.down, Color.red, 0.2f);
        //Debug.DrawRay(pos + offset, Vector2.down, Color.red, 0.2f);
        if(leftCheckGround || rightCheckGround) isGround = true;
        else isGround = false;
    }

    void GroundMovement()//角色移动
    {
        rxVelocity = rb.velocity.x;
        //xVelocity = Input.GetAxis("Horizontal");
        //xVelocityRaw = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xVelocity * speed - GM.subspeed * faceDir/*后坐力*/, rb.velocity.y);
        if (jumpHeld)
        {
            if(isGround && canJump > 0 && jumpStart == false){
                AM.PlayerSound(AM.jump);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpStart = true; //人为起跳信号
            }
            if(jumpStart){
                jumpHoldDuration += Time.deltaTime;
            }
            canJump -= Time.deltaTime;
        }
        else {
            jumpStart = false;
            canJump = 0.1f; //提前跳跃重新计时
            if(jumpHoldDuration > 0){
                if(jumpHoldDuration > 0.15f) jumpHoldDuration = 0; //按键超过0.15秒判定为完整跳跃
                else { //否则施加下降力，提前下落
                    if(jumpHoldDuration < 0.07f) downForce = 8.0f;
                    else downForce = 5.0f;
                    rb.AddForce(Vector2.down * downForce, ForceMode2D.Impulse);
                    jumpHoldDuration = 0;
                }
            }
        }
    }

    void HorizontalAxis() //模拟系统自带input的平滑效果（因系统自带输入系统不支持动态更改按键，因此使用手动方法）
    {
        if (Input.GetKey(GameManager.instance.leftKey))
        {
            lastAxis = false;
            if (horiAxis1 < 1)
            {
                horiAxis1 += Time.deltaTime * 5;
            }
            xVelocity = Mathf.Lerp(0, -1, horiAxis1); //平滑增加至最大值
            xVelocityRaw = -1;

            if (horiAxis2 > 0) //没有向右输入时，右水平输入值减少
            {
                horiAxis2 -= Time.deltaTime * 3;
            }
        }
        else if (Input.GetKey(GameManager.instance.rightKey))
        {
            lastAxis = true;
            if (horiAxis2 < 1)
            {
                horiAxis2 += Time.deltaTime * 5;
            }
            xVelocity = Mathf.Lerp(0, 1, horiAxis2); //平滑增加至最大值
            xVelocityRaw = 1;

            if (horiAxis1 > 0) //没有向左输入时，左水平输入值减少
            {
                horiAxis1 -= Time.deltaTime * 3;
            }
        }
        else //没有按键输入时，aixs值减少
        {
            if (horiAxis1 > 0)
            {
                horiAxis1 -= Time.deltaTime * 3;
            }
            if (horiAxis2 > 0)
            {
                horiAxis2 -= Time.deltaTime * 3;
            }
            if(lastAxis == true) //缓慢减速
            {
                xVelocity = Mathf.Lerp(0, 1, horiAxis2);
            }
            else
            {
                xVelocity = Mathf.Lerp(0, -1, horiAxis1);
            }
            xVelocityRaw = 0;
        }
    }

    void SwitchAnim()//切换动画
    {
        anim.SetBool("Idle", false);
        //跑步动画切换
        anim.SetFloat("Walk", Mathf.Abs(rxVelocity)); //跑步速度精准数值
        anim.SetFloat("Walkraw", Mathf.Abs(xVelocityRaw)); //左右按键情况，仅为0或1
        
        if(rb.velocity.y > 0.1){
            anim.SetBool("Jump", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Fall", false);
        }
        else if(rb.velocity.y < -0.1){
            anim.SetBool("Fall", true);
            anim.SetBool("Idle", false);
            anim.SetBool("Jump", false);
        }
        else if(isGround){ //如果碰撞体接触到地面层
            anim.SetBool("Fall", false);
            anim.SetBool("Idle", true);
            anim.SetBool("Jump", false);
        }
    }

    void FlipDirection() //角色转向
    {
        if (xVelocityRaw < 0) faceDir = -1;
        if (xVelocityRaw > 0) faceDir = 1;
        transform.localScale = new Vector2(-faceDir, 1);
        if(xScale != transform.localScale.x)
        {
            if (transform.Find("StopParticle(Clone)")) //如果转向时有刹车粒子，则将其删除
            {
                Destroy(transform.Find("StopParticle(Clone)").gameObject);

            }
        }
        xScale = transform.localScale.x;
    }

    public void StopParticle() //刹车粒子特效，由刹车动画触发
    {
        if (rb.velocity.x > 7f || rb.velocity.x < -7f) //速度较快时才触发刹车粒子特效
        {
            float ang;
            if (transform.localScale.x > 0)
            {
                spDis = -0.1875f;
                ang = -135f;
            }
            else
            {
                spDis = 0.1875f;
                ang = -45f;
            }
            //生成刹车粒子效果并以玩家为父对象
            newParticle = Instantiate(stopParticle, new Vector2(transform.position.x + spDis, transform.position.y), transform.rotation, transform);
            newParticle.transform.eulerAngles = new Vector3(ang, 90f, 0); //粒子发射器角度修正
        }
    }

    void WalkParticle() //走路粒子特效，由持续走路距离触发
    {
        if (rb.velocity.x > 7f || rb.velocity.x < -7f)
        {
            walkDis += walkPos - transform.position.x;
            walkPos = transform.position.x;
            if((walkDis > 4f || walkDis < -4f) && isGround) //行进距离超过4格且站在地面上时，生成走路粒子发射器
            {
                float ang;
                if (transform.localScale.x > 0)
                {
                    spDis = 0.2f;
                    ang = -30f;
                }
                else
                {
                    spDis = -0.2f;
                    ang = -150f;
                }
                //生成走路粒子效果并以玩家为父对象
                ParticleSystem newWalk = Instantiate(walkParticle, new Vector2(transform.position.x + spDis, transform.position.y), transform.rotation);
                newWalk.transform.eulerAngles = new Vector3(ang, 90f, 0); //粒子发射器角度修正
                walkDis = 0;
            }
        }
        else //速度减慢时清空已行进距离
        {
            walkDis = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) //撞到敌人受伤
    {
        if(collision.gameObject.tag == "Enemy")
        {
            AM.PlayerSound(AM.hurt1[Random.Range(0, 4)]);
            Physics2D.IgnoreLayerCollision(10, 14, true); //关闭敌人与玩家之间的碰撞
            impulse.GenerateImpulse(); //向Cinemachine镜头发送振动信号
            isInvincible = true; //受伤无敌flag开
            var v = collision.relativeVelocity;
            rb.AddForce(new Vector2(0, v.y), ForceMode2D.Impulse); //向上击退力
            var pos = collision.GetContact(0).point;
            hv = v.x > 0 ? 5f : -5f; //判断水平击退力方向
            Instantiate(hurtEffect, pos, transform.rotation);
        }
    }

    void HurtFlash() //受伤时闪烁效果
    {
        if (isInvincible)
        {
            timeSpentInvincible += Time.deltaTime;
            if(timeSpentInvincible < 0.2f) //0.2s的受伤击退
            {
                rb.velocity = new Vector2(hv, rb.velocity.y);
            }
            if (timeSpentInvincible < 2f) //总无敌闪烁时间为2s
            {
                //模的结果小于0.025时设置为隐藏，大于0.025时设置为显示，即为每0.025s进行隐藏与显示的交替
                float remainder = timeSpentInvincible % 0.05f; //与0.05进行模运算
                playerRend.enabled = remainder > 0.025f;
                gunRend.enabled = remainder > 0.025f;
            }
            else
            { //超过3s后停止闪烁并将计数器归零
                playerRend.enabled = true;
                gunRend.enabled = true;
                isInvincible = false;
                timeSpentInvincible = 0;
                Physics2D.IgnoreLayerCollision(10, 14, false); //重新开启碰撞
            }
        }
    }

    void GetDamage(int d) //从敌人脚本获取撞到敌人的伤害值（正值）
    {
        GM.hp -= d;
        if (GM.hp <= 0) //hp不能小于0，防止血块数组下标出现负数，并传递给HPUIChange函数正好将hp减为0的值
        {
            GM.HPUIChange(-(GM.hp + d));
            GM.hp = 0;
            Death(); //角色死亡
        }
        else if(GM.hp > 50) //hp不能超过50
        {
            var temp = -d + 50 - GM.hp;
            GM.hp = 50;
            GM.HPUIChange(temp);
        }
        else GM.HPUIChange(-d);

        if (GM.hp <= 10) //根据HP值判断是否改变血条动画
        {
            AM.player1DangerSource.Play(); //播放危险音效
            healthbarAnim.SetBool("Low", true);
        }
        else
        {
            AM.player1DangerSource.Stop(); //停止播放危险音效
            healthbarAnim.SetBool("Low", false);
        }
    }

    void Death() //角色死亡
    {
        AM.PlayerSound(AM.dead1[Random.Range(0, 2)]);
        var p = Instantiate(death, transform.position, transform.rotation); //生成玩家死亡预制体
        var g = Instantiate(deathGun, transform.position, transform.rotation); //生成枪死亡预制体
        p.transform.localScale = transform.localScale; //设置面向
        g.transform.localScale = transform.localScale;
        var rp = p.GetComponent<Rigidbody2D>();
        var rg = g.GetComponent<Rigidbody2D>();
        rp.AddForce(new Vector2(-faceDir * 10, 10), ForceMode2D.Impulse); //玩家死亡预制体飞出
        rg.AddForce(new Vector2(faceDir * 10, 10), ForceMode2D.Impulse); //枪死亡预制体飞出，并在两侧施加相反力使其旋转
        rg.AddForceAtPosition(new Vector2(0, 3f), new Vector2(transform.position.x - 0.1f, transform.position.y), ForceMode2D.Impulse);
        rg.AddForceAtPosition(new Vector2(0, -3f), new Vector2(transform.position.x + 0.1f, transform.position.y), ForceMode2D.Impulse);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) //收集物品
    {
        if(collision.gameObject.tag == "Coin") //收集硬币
        {
            AM.ItemSound(AM.coin);
            Instantiate(coinCollected, collision.gameObject.transform.position, coinCollected.transform.rotation);
            Destroy(collision.gameObject.transform.parent.gameObject); //由于碰撞的物体是硬币的子物体，因此需要删除子物体的父级
            GM.unbankedScore += 5;
            scoreText.text = string.Format("{0:0,000,000}", GM.unbankedScore); //更改分数并反映至UI
        }
        if (collision.gameObject.tag == "HPChest") //收集医药箱
        {
            AM.ItemSound(AM.hpChest);
            collision.gameObject.transform.parent.gameObject.SendMessage("Destroy_", true); //由于碰撞的物体是硬币的子物体，因此需要删除子物体的父级
            GetDamage(-10); //加10点HP
        }
        if (collision.gameObject.tag == "AmmoChest") //收集武器箱
        {
            AM.ItemSound(AM.ammoChest);
            var chest = collision.transform.parent.gameObject; //获取碰撞体的父物体（箱子）
            int ammoNum = chest.GetComponent<AmmoChest>().AmmoNum();
            chest.SendMessage("Destroy_", true); //由于碰撞的物体是硬币的子物体，因此需要删除子物体的父级
            weaponSwitching.Switching(ammoNum); //调用切换武器脚本
        }
    }

    private void ScoreChange() //分数变化函数，此函数放入FixedUpdate中，分数每个fix帧只能变化1
    {
        if(scoreTemp != GM.unbankedScore)
        {
            if(scoreTemp < GM.unbankedScore)
            {
                scoreText.text = string.Format("{0:0,000,000}", ++scoreTemp); //更改分数并反映至UI
            }
            else
            {
                scoreText.text = string.Format("{0:0,000,000}", --scoreTemp); //更改分数并反映至UI
            }
        }
    }

    void CanControll(bool flag) //从其他脚本获取是否能够控制角色
    {
        canCtrl = flag;
    }

    public void MoveStop() //停止角色，用于进入电梯时
    {
        rb.velocity = new Vector2(0, 0);
    }
}
