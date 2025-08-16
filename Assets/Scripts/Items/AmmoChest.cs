using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoChest : MonoBehaviour
{
    public GameObject chestDisappear; //箱子消失动画
    private GameObject trigger; //子物体trigger
    private int pos; //当前箱子生成的格子下标
    private int ammoNum; //当前箱子生成的武器编号
    private GameObject chestSpawn;
    private SpriteRenderer rend;
    private SpriteRenderer triggerRend;
    private float lifeTime; //存在时间

    [Header("武器图标")]
    private GameObject ammoIcon; //子物体中存放各种武器图标
    private bool changeFlag = true; //标记图标是否已更改

    [Header("悬浮文本")]
    public GameObject chestText;
    private WeaponNameDictionary weaponNameDictionary; //武器名称字典
    private GameObject worldCanvas; //世界画布

    private void Awake()
    {
        chestSpawn = GameObject.Find("ChestSpawn");
        rend = GetComponent<SpriteRenderer>();
        ammoIcon = GameObject.Find("AmmoIcon");
        worldCanvas = GameObject.Find("WorldCanvas"); //获取当前场景的世界画布
        trigger = transform.GetChild(0).gameObject;
        triggerRend = trigger.gameObject.GetComponent<SpriteRenderer>();
        weaponNameDictionary = gameObject.AddComponent<WeaponNameDictionary>(); //实例化武器名称字典
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
        if(ammoNum != 0 && changeFlag) //获取到武器编号后更改箱子的图标和子物体trigger的武器编号
        {
            ChangeIcon();
            changeFlag = false;
        }

        lifeTime += Time.deltaTime;
        if (lifeTime > 10f) //10s后开始闪烁，模的结果小于0.025时设置为隐藏，大于0.025时设置为显示，即为每0.025s进行隐藏与显示的交替
        {
            float remainder = lifeTime % 0.05f; //与0.05进行模运算
            rend.enabled = remainder > 0.025f;
            triggerRend.enabled = remainder > 0.025f;
        }
        if (lifeTime > 15f) //15s后自动销毁
        {
            Destroy_(false);
        }
    }

    void GetCoord(int c) //从chestSpawn脚本获取格子数组下标
    {
        pos = c;
    }

    void GetAmmonum(int n) //从chestSpawn脚本获取生成哪个武器的箱子
    {
        ammoNum = n;
    }

    void ChangeIcon()
    {
        var icon = ammoIcon.transform.GetChild(ammoNum); //按下标获取对应武器子物体
        var image = icon.gameObject.GetComponent<Image>().sprite; //获取子物体image的精灵
        triggerRend.sprite = image; //将武器箱的图标更改为获取到的图标
    }

    public int AmmoNum() //其他脚本调用，返回当前箱子的武器编号
    {
        return ammoNum;
    }

    void Destroy_(bool get) //摧毁前解除格子占用，get代表是否由玩家拾取
    {
        if (get)
        {
            var offset = new Vector3(0, 0.5f, 0); //向上偏移
            var textObj = Instantiate(chestText, transform.position + offset, chestText.transform.rotation, worldCanvas.transform);
            textObj.GetComponent<Text>().text = weaponNameDictionary.Value(ammoNum); //从武器字典获取当前武器名称
        }
        Instantiate(chestDisappear, transform.position, chestDisappear.transform.rotation);
        chestSpawn.SendMessage("AmmoChestDestroy", pos);
        Destroy(gameObject);
    }
}
