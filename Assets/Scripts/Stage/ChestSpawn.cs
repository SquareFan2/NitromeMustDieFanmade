using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ChestSpawn : MonoBehaviour
{
    private GameManager GM;

    public GameObject healthChest; //医疗箱预制件
    public GameObject ammoChest; //武器箱预制件
    public int[] ammoRange; //存储当前关卡所有可能出现的武器类型，在编辑器中更改

    private Tilemap tilemap;
    private Vector3Int[] vector3Ints; //坐标数组，存放所有瓦片的坐标，z为访问情况，1为可以访问，0为被占用
    private BoundsInt bounds; //道具生成地图的边界
    private int n; //可生成格子总数

    private Text ammoText; //弹药数UI文本
    private Text maxAmmoText; //最大弹药数UI文本
    private int ammo; //监测弹药数
    private int maxAmmo; //监测最大弹药数

    private float timeHP = 0; //医药箱经过时间
    private float timeAmmo = 0; //武器箱经过时间
    private Vector3 offset = new Vector3(0.5f, 0.484375f, 0); //生成箱子时的偏移量（因CelltoWorld的坐标为格子的左下角）
    private bool hpChestAppear = false; //标记医药箱是否已生成
    private bool ammoChestAppear = false; //标记武器箱是否已生成
    private bool canSpawn = true; //标记当前是否可以生成箱子，当关卡完成后由EnemySpawn脚本设为false

    void Awake()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        ammoText = GameObject.Find("Ammo").GetComponent<Text>();
        maxAmmoText = GameObject.Find("MaxAmmo").GetComponent<Text>();
    }

    void Start()
    {
        GM = GameManager.instance;

        bounds = tilemap.cellBounds; //获取边界
        vector3Ints = new Vector3Int[bounds.size.x * bounds.size.y]; //以格子总数为大小实例化数组
        //Debug.Log("xMin = " + bounds.xMin + ", yMin = " + bounds.yMin);
        //Debug.Log("xMax = " + bounds.xMax + ", yMax = " + bounds.yMax);
        n = 0;
        for(int i = bounds.xMin; i <= bounds.xMax; i++) //遍历整个Tilemap，如果有砖块则将坐标计入数组
        {
            for (int j = bounds.yMin; j <= bounds.yMax; j++)
            {
                if(tilemap.GetTile(new Vector3Int(i, j, 0)))
                {
                    vector3Ints[n++] = new Vector3Int(i, j, 1);
                }
            }
        }
    }

    void FixedUpdate()
    {
        ammo = int.Parse(ammoText.text);
        maxAmmo = int.Parse(maxAmmoText.text);

        //医药箱生成逻辑
        if(GM.hp < 50) //生命值不满时才判定
        {
            timeHP += Time.deltaTime;
            if(timeHP >= 7f) //每7s判定一次
            {
                timeHP = 0;
                int r = Random.Range(1, 51); //hp越少，生成概率越高
                if (r > GM.hp && !hpChestAppear && canSpawn)
                {
                    hpChestAppear = true; //标记医药箱已生成
                    HealthSpawn();
                }
            }
        }
        else
        {
            timeHP = 0;
        }

        //武器箱生成逻辑
        timeAmmo += Time.deltaTime;
        if (timeAmmo >= 7f) //每7s判定一次
        {
            timeAmmo = 0;
            int sr = Random.Range(1, 4); //第一次判定，固定1/3概率生成箱子
            if (sr == 1 && !ammoChestAppear && canSpawn)
            {
                ammoChestAppear = true; //标记武器箱已生成
                AmmoSpawn();
            }
            int r = Random.Range(1, maxAmmo + 1); //第二次判定，根据当前弹药数生成箱子
            if (r > Mathf.Max(ammo, maxAmmo / 2) && !ammoChestAppear && canSpawn) //弹药数越少，生成概率越高，但最高不超过1/2
            {
                ammoChestAppear = true; //标记武器箱已生成
                AmmoSpawn();
            }
        }
    }

    void HealthSpawn()//生成医药箱
    {
        int tr;
        do
        {
            tr = Random.Range(0, n); //随机抽选一个格子
        } while (vector3Ints[tr].z == 0); //如果不可访问则重新抽取
        vector3Ints[tr].z = 0; //标记占用
        var x = vector3Ints[tr].x;
        var y = vector3Ints[tr].y;
        var pos = tilemap.CellToWorld(new Vector3Int(x, y, 0)); //转换单元格为世界坐标
        GameObject o = Instantiate(healthChest, pos + offset, healthChest.transform.rotation);
        o.SendMessage("GetCoord", tr); //将格子下标传给生成的箱子
    }

    void AmmoSpawn()//生成武器箱
    {
        int tr;
        do
        {
            tr = Random.Range(0, n); //随机抽选一个格子
        } while (vector3Ints[tr].z == 0); //如果不可访问则重新抽取
        vector3Ints[tr].z = 0; //标记占用
        var x = vector3Ints[tr].x;
        var y = vector3Ints[tr].y;
        var pos = tilemap.CellToWorld(new Vector3Int(x, y, 0)); //转换单元格为世界坐标

        int ar;
        ar = Random.Range(0, ammoRange.Length); //随机抽选武器生成范围数组中的一个武器
        ar = ammoRange[ar];
        GameObject o = Instantiate(ammoChest, pos + offset, healthChest.transform.rotation);
        o.SendMessage("GetCoord", tr); //将格子下标传给生成的箱子
        o.SendMessage("GetAmmonum", ar); //将武器编号传给生成的箱子
    }

    void HPChestDestroy(int c) //医药箱消失后，将格子解除占用，将HP箱子计时器归零，并标记医药箱未生成
    {
        timeHP = 0;
        vector3Ints[c].z = 1;
        hpChestAppear = false;
    }

    void AmmoChestDestroy(int c) //武器箱消失后，将格子解除占用，将武器箱子计时器归零，并标记武器箱未生成
    {
        timeAmmo = 0;
        vector3Ints[c].z = 1;
        ammoChestAppear = false;
    }

    public void ChangeFlag(bool flag) //更改当前是否可以生成箱子，由EnemySpawn脚本执行
    {
        canSpawn = flag;
    }
}
