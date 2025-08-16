using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPChest : MonoBehaviour
{
    public GameObject chestDisappear; //箱子消失动画
    private int pos; //当前箱子生成的格子下标
    private GameObject chestSpawn;
    private SpriteRenderer rend;
    private SpriteRenderer triggerRend;
    private float lifeTime; //存在时间

    [Header("悬浮文本")]
    public GameObject chestText;
    private GameObject worldCanvas; //画布
    private Camera cam; //摄像头类，用于转换屏幕坐标与世界坐标

    private void Awake()
    {
        chestSpawn = GameObject.Find("ChestSpawn");
        rend = GetComponent<SpriteRenderer>();
        triggerRend = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        worldCanvas = GameObject.Find("WorldCanvas"); //获取当前场景的世界画布
    }
    void Start()
    {

    }

    void FixedUpdate()
    {
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

    void GetCoord(int c) //从ItemSpawn脚本获取格子数组下标
    {
        pos = c;
    }

    void Destroy_(bool get) //摧毁前解除格子占用，get代表是否由玩家拾取
    {
        if (get)
        {
            var offset = new Vector3(0, 0.5f, 0); //向上偏移
            var textObj = Instantiate(chestText, transform.position + offset, chestText.transform.rotation, worldCanvas.transform);
            textObj.GetComponent<Text>().text = "HEALTH UP";
        }

        Instantiate(chestDisappear, transform.position, chestDisappear.transform.rotation);
        chestSpawn.SendMessage("HPChestDestroy", pos);
        Destroy(gameObject);
    }
}
