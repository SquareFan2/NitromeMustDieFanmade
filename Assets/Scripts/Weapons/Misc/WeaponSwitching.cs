using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponSwitching : MonoBehaviour
{
    //注：此脚本运行顺序略晚于默认顺序，以便在加载新关卡时，EnemySpawn_x_x脚本能够先设置好武器，此脚本再进行武器更新
    private GameManager GM;

    [Header("UI动画")]
    public Image animIcon; //切换武器时飞出的图标
    private Camera cam; //摄像头类，用于转换屏幕坐标与世界坐标
    private GameObject canvas; //当前场景的画布
    private Image animObject; //生成的动画图标
    private Vector2 animPos; //动画图标生成的坐标
    private double frame = 1.025; //用于计时，每个FixedUpdate加0.025
    private GameObject ammoIcon; //子物体中存放各种武器图标

    [Header("弹药计数")]
    private AmmoDictionary ammoDictionary; //武器弹药数字典
    private int maxAmmoNum; //当前武器的最大弹药数，从字典中获取
    private Text ammo; //弹药数UI文本
    private Text maxAmmo; //最大弹药数UI文本，不可见，仅用于记录当前武器的最大弹药数
    private int ammoNumTemp; //临时计数器，用于设置弹药数字的过渡动画
    private Animator ammoAnim; //弹药数UI动画控制器

    [Header("悬浮文本")]
    public GameObject chestText;
    private GameObject worldCanvas; //世界画布

    public Animator gunAnim; //枪动画控制器，防止弹药耗尽时强制切换导致发光材质未被换下

    private void Awake()
    {
        cam = Camera.main; //获取当前场景的主摄像头
        canvas = GameObject.Find("Canvas"); //获取当前场景的画布
        worldCanvas = GameObject.Find("WorldCanvas"); //获取当前场景的世界画布
        ammo = GameObject.Find("Ammo").GetComponent<Text>();
        ammoAnim = GameObject.Find("AmmoFrame").GetComponent<Animator>();
        maxAmmo = GameObject.Find("MaxAmmo").GetComponent<Text>();
        ammoIcon = GameObject.Find("AmmoIcon");
        ammoDictionary = gameObject.AddComponent<AmmoDictionary>(); //实例化武器弹药数字典
    }

    void Start()
    {
        GM = GameManager.instance;
        if (GM.currentWeapon < 0) //初始化时，默认武器为70弹药的手枪
        {
            GM.currentWeapon = 1;
            ammo.text = "070";
            maxAmmo.text = "70";
            GM.ammo = 70;
        }
        else //不为-1时则加载GM储存的武器
        {
            transform.GetChild(1).gameObject.SetActive(false); //将默认激活的Pistol设为不活动
            transform.GetChild(GM.currentWeapon).gameObject.SetActive(true); //将从GM获取的当前武器设为活动
        }
        ammoNumTemp = 0;
    }

    void FixedUpdate()
    {
        //切换武器动画
        if (frame < 1.025)
        {
            if (frame >= 1) //计时器等于1时将飞出动画删除并更改武器图标的UI
            {
                Destroy(animObject.gameObject);
                SwitchIcon(GM.currentWeapon);
            }
            else
            {
                animObject.rectTransform.anchoredPosition = animPos;
            }
            frame += 0.025;
        }

        //弹药数变化动画
        if(ammoNumTemp != GM.ammo)
        {
            if(ammoNumTemp < GM.ammo) //每个Fixed帧弹药数都只能变化1
            {
                ammo.text = string.Format("{0:D3}", ++ammoNumTemp); //将数字格式化，D代表十进制，D3代表3位十进制数；其他详见 https://www.cnblogs.com/net-sky/p/10250880.html
            }
            else
            {
                ammo.text = string.Format("{0:D3}", --ammoNumTemp);
            }
        }
    }

    void ammoReduce(int r) //减少弹药数量，由武器脚本触发
    {
        GM.ammo -= r;
        if(GM.ammo <= 0) //弹药空时自动切换
        {
            maxAmmo.text = "0";
            GM.ammo = 0;
            Switching(0);
            var textObj = Instantiate(chestText, transform.position + new Vector3(0, 0.5f, 0), chestText.transform.rotation, worldCanvas.transform);
            textObj.GetComponent<Text>().text = "OUT OF AMMO";
        }
    }

    public void Switching(int s) //切换武器，也会由其他脚本调用
    {
        transform.GetChild(GM.currentWeapon).gameObject.SetActive(false); //将当前武器设为不活动
        transform.GetChild(s).gameObject.SetActive(true); //将目标武器设为活动

        gunAnim.SetBool("ConstantFire", false); //将动画控制器的持续开火触发器设置一遍false，防止弹药耗尽时强制切换导致发光材质未被换下
        GM.subspeed = 0; //重置武器后坐力

        if (s == 0) //没有弹药时弹药数UI闪红光
        {
            ammoAnim.SetBool("Out", true);
        }
        else
        {
            ammoAnim.SetBool("Out", false);
        }
        GM.currentWeapon = s;

        //切换武器动画
        var icon = ammoIcon.transform.GetChild(s); //按下标获取对应武器子物体
        var image = icon.gameObject.GetComponent<Image>().sprite; //获取子物体image的精灵
        animIcon.sprite = image; //将精灵赋值给飞出的动画图标
        animPos = cam.WorldToScreenPoint(transform.position); //将开火点的世界坐标转换为屏幕的UI坐标
        animObject = Instantiate(animIcon, animPos, animIcon.transform.rotation);
        animObject.transform.SetParent(canvas.transform, true); //将生成的动画图标的父对象绑定为画布
        animPos = animObject.rectTransform.anchoredPosition; //将animPos转换为生成位置的rect坐标，方便DOTween移动时使用
        DOTween.To(() => animPos, x => animPos = x, new Vector2(80, 493), 0.8f); //DOTween动画,使animPos从生成位置变化至UI图标位置
        frame = 0; //刷新frame

        //更改弹药数
        maxAmmoNum = ammoDictionary.Value(s); //从字典获取当前武器的最大弹药数
        GM.ammo = maxAmmoNum;
    }

    void SwitchIcon(int s) //切换武器图标UI
    {
        //Debug.Log("SwitchIcon(" + s + ")");
        var icon = ammoIcon.transform.GetChild(s); //按下标获取对应武器子物体
        var image = icon.gameObject.GetComponent<Image>().sprite; //获取子物体image的精灵
        ammoIcon.GetComponent<Image>().sprite = image; //将精灵赋值给武器图标
    }
}
