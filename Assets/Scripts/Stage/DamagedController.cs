using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DamagedController : MonoBehaviour
{
    [Header("Tilemap")]
    private Tilemap damaged;
    private Tilemap wall;
    [Header("瓦片")]
    public TileBase tile1;
    public TileBase tile2;
    public TileBase tile3;
    public TileBase tile4;
    public TileBase tile5;
    public TileBase tile6;
    public TileBase tile7;
    public TileBase tile8;
    public TileBase tile9;
    public TileBase tile1_1;
    public TileBase tile2_1;
    public TileBase tile3_1;
    public TileBase tile4_1;
    public TileBase tile5_1;
    public TileBase tile6_1;
    public TileBase tile7_1;
    public TileBase tile8_1;
    public TileBase tile9_1;
    public TileBase tile1_2;
    public TileBase tile2_2;
    public TileBase tile3_2;
    public TileBase tile4_2;
    public TileBase tile5_2;
    public TileBase tile6_2;
    public TileBase tile7_2;
    public TileBase tile8_2;
    public TileBase tile9_2;
    public TileBase tile1_3;
    public TileBase tile2_3;
    public TileBase tile3_3;
    public TileBase tile4_3;
    public TileBase tile5_3;
    public TileBase tile6_3;
    public TileBase tile7_3;
    public TileBase tile8_3;
    public TileBase tile9_3;
    [Header("粒子")]
    public ParticleSystem rubble;
    public ParticleSystem rubbleCounter;
    public GameObject firePoint; //当前场景的开火点预制体
    [Header("变量")]
    private int[,] hp; //二维数组存储每个Wall的伤害值，默认为0
    private BulletDictionary bulletDictionary; //子弹伤害字典
    public int x;
    public int y;
    public int d;
    public string n;
    //public Vector2 speed;

    void Start()
    {
        bulletDictionary = gameObject.AddComponent<BulletDictionary>(); //实例化子弹伤害字典
        hp = new int[1000,1000];
        firePoint = GameObject.Find("Firepoint"); //获取当前场景的开火点预制体
        damaged = GameObject.Find("Damaged").GetComponent<Tilemap>();
        wall = gameObject.GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet" && collision.gameObject.layer != 18)
        {
            //speed = collision.relativeVelocity; //获取子弹撞击墙面的速度方向，确保损坏贴图生成在墙砖上
            Vector2 pos = collision.GetContact(0).point; //获取子弹的碰撞点
            //在碰撞点的上下左右0.2格的距离各创建一个点，确保所有与子弹接触的砖块都受到损伤
            Vector2 pos1 = new Vector2(pos.x-0.2f, pos.y); //左
            Vector2 pos2 = new Vector2(pos.x+0.2f, pos.y); //右
            Vector2 pos3 = new Vector2(pos.x, pos.y - 0.2f); //下
            Vector2 pos4 = new Vector2(pos.x, pos.y + 0.2f); //上
            //将4个点分别转换为Tilemap单元格
            Vector3Int posc1 = damaged.WorldToCell(pos1);
            Vector3Int posc2 = damaged.WorldToCell(pos2);
            Vector3Int posc3 = damaged.WorldToCell(pos3);
            Vector3Int posc4 = damaged.WorldToCell(pos4);
            Vector3Int[] poscarrtmp = { posc1, posc2, posc3, posc4 }; //将4个单元格存进数组
            List<Vector3Int> poscarr =new List<Vector3Int>(); //去掉重复的单元格
            foreach (Vector3Int v in poscarrtmp)
            {
                if (!poscarr.Contains(v))
                    poscarr.Add(v);
            }
            foreach (Vector3Int posi in poscarr)
            {
                if (wall.GetTile(posi)) //判断当前单元格上是否存在Wall层的瓦片
                {
                    x = posi.x + 500;
                    y = posi.y + 500;
                    if (hp[x, y] == 0) //当前砖块hp为0，则需要初始化
                    {
                        int rand = Random.Range(1, 13);
                        hp[x, y] += rand * 100; //hp的百位数为随机到的数值
                    }
                    d = hp[x, y] % 100; //d表示此砖块实际受到的伤害
                    if (d < 70) //如果砖块损坏值高于70，则无需再对墙壁损伤度进行更改
                    {
                        int r = hp[x, y] / 100; //r表示此砖块随机到的花纹样式
                        n = collision.gameObject.name; //获取撞击的子弹的名称
                        int damage = bulletDictionary.Value(n); //通过子弹名称获取子弹的伤害值
                        hp[x, y] += damage; //累加伤害值
                        d = hp[x, y] % 100; //重新计算伤害值

                        if (d >= 70) //损坏阶段3
                        {
                            switch (r)
                            {
                                case 1: damaged.SetTile(posi, tile3); break;
                                case 2: damaged.SetTile(posi, tile6); break;
                                case 3: damaged.SetTile(posi, tile9); break;
                                case 4: damaged.SetTile(posi, tile3_1); break;
                                case 5: damaged.SetTile(posi, tile6_1); break;
                                case 6: damaged.SetTile(posi, tile9_1); break;
                                case 7: damaged.SetTile(posi, tile3_2); break;
                                case 8: damaged.SetTile(posi, tile6_2); break;
                                case 9: damaged.SetTile(posi, tile9_2); break;
                                case 10: damaged.SetTile(posi, tile3_3); break;
                                case 11: damaged.SetTile(posi, tile6_3); break;
                                case 12: damaged.SetTile(posi, tile9_3); break;
                                default: break;
                            }
                        }
                        else if (d >= 50) //损坏阶段2
                        {
                            switch (r)
                            {
                                case 1: damaged.SetTile(posi, tile2); break;
                                case 2: damaged.SetTile(posi, tile5); break;
                                case 3: damaged.SetTile(posi, tile8); break;
                                case 4: damaged.SetTile(posi, tile2_1); break;
                                case 5: damaged.SetTile(posi, tile5_1); break;
                                case 6: damaged.SetTile(posi, tile8_1); break;
                                case 7: damaged.SetTile(posi, tile2_2); break;
                                case 8: damaged.SetTile(posi, tile5_2); break;
                                case 9: damaged.SetTile(posi, tile8_2); break;
                                case 10: damaged.SetTile(posi, tile2_3); break;
                                case 11: damaged.SetTile(posi, tile5_3); break;
                                case 12: damaged.SetTile(posi, tile8_3); break;
                                default: break;
                            }
                        }
                        else if (d >= 30) //损坏阶段1
                        {
                            switch (r)
                            {
                                case 1: damaged.SetTile(posi, tile1); break;
                                case 2: damaged.SetTile(posi, tile4); break;
                                case 3: damaged.SetTile(posi, tile7); break;
                                case 4: damaged.SetTile(posi, tile1_1); break;
                                case 5: damaged.SetTile(posi, tile4_1); break;
                                case 6: damaged.SetTile(posi, tile7_1); break;
                                case 7: damaged.SetTile(posi, tile1_2); break;
                                case 8: damaged.SetTile(posi, tile4_2); break;
                                case 9: damaged.SetTile(posi, tile7_2); break;
                                case 10: damaged.SetTile(posi, tile1_3); break;
                                case 11: damaged.SetTile(posi, tile4_3); break;
                                case 12: damaged.SetTile(posi, tile7_3); break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }
    }
}
