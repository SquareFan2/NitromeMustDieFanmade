using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour //主菜单选关界面UI挂载脚本
{
    private int[] staticArr; //固定数组，按顺序排列楼层
    private int[] randomArr; //随机数组，存放随机打乱顺序后的楼层
    private int[] tempArr; //临时数组，存放除特殊关之外的楼层
    private GameManager GM;
    private AudioManager AM;

    private void Start()
    {
        staticArr = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        randomArr = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        tempArr = new int[7] { 0, 1, 3, 4, 6, 7, 8 };
        GM = GameManager.instance;
        AM = AudioManager.instance;
    }

    private void Shuffle()
    {
        for(int i = 0; i < 7; i++)
        {   //随机和数组的其中一位交换位置
            var r = Random.Range(0, 7);
            var temp = tempArr[i];
            tempArr[i] = tempArr[r];
            tempArr[r] = temp;
        }
        randomArr[0] = tempArr[0];
        randomArr[1] = tempArr[1];
        randomArr[3] = tempArr[2];
        randomArr[4] = tempArr[3];
        randomArr[6] = tempArr[4];
        randomArr[7] = tempArr[5];
        randomArr[8] = tempArr[6];
    }

    public void OnClick1()
    {
        GM.currentFloor = 1;
        if(GM.levelCleared < 1) //未通过第一大关，则固定顺序加载关卡
        {
            GM.floorArr = staticArr;
        }
        else //随机打乱数组并赋值给GameManager
        {
            Shuffle();
            GM.floorArr = randomArr;
        }
        GM.Invoke("LoadLevelStart", 0.2f);

        AM.StopLoop(); //停止循环并控制BGM淡出
        AM.cor = AM.StartCoroutine(AM.BGMout());
    }
}
