using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawn : MonoBehaviour
{
    [Header("基本参数")]
    public GameObject spawnEffect;
    public int spawnNum;
    public int existNum;
    protected Tilemap tilemap;
    protected Vector2Int[] vector2Ints; //坐标数组，存放所有瓦片的坐标
    protected BoundsInt bounds; //道具生成地图的边界
    protected int n = 0; //可生成格子总数
    protected float timer = 0; //计时器
    protected GameManager GM;

    void Awake()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    void Start()
    {
        GM = GameManager.instance;
        GM.spawnTotal = spawnNum;
        GM.existTotal = existNum;

        bounds = tilemap.cellBounds; //获取边界
        vector2Ints = new Vector2Int[bounds.size.x * bounds.size.y]; //以格子总数为大小实例化数组
        for (int i = bounds.xMin; i <= bounds.xMax; i++) //遍历整个Tilemap，如果有砖块则将坐标计入数组
        {
            for (int j = bounds.yMin; j <= bounds.yMax; j++)
            {
                if (tilemap.GetTile(new Vector3Int(i, j, 0)))
                {
                    vector2Ints[n++] = new Vector2Int(i, j);
                }
            }
        }
        for (int i = 0; i < n; i++)
        {
            Debug.Log("vector2Ints[" + i + "] = " + vector2Ints[i]);
        }
    }

    void FixedUpdate() //生成程序每关手动编写
    {
        timer += Time.deltaTime; //每秒timer+1
        LevelSpawn();
    }

    protected virtual void LevelSpawn()
    {

    }

    protected void SpawnDown(GameObject enemy, int tileNum) //向下方生成敌人
    {
        var x = vector2Ints[tileNum].x;
        var y = vector2Ints[tileNum].y;
        var pos = tilemap.CellToWorld(new Vector3Int(x, y, 0)); //转换单元格为世界坐标
        GM.spawnTotal--;
        GM.existTotal++;
        spawnEffect.transform.eulerAngles = new Vector3(0, 0, 90); //使生成特效朝下
        Instantiate(spawnEffect, pos, spawnEffect.transform.rotation); //生成敌人生成特效
        Instantiate(enemy, pos, enemy.transform.rotation);
    }
}
