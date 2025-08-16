//此脚本用于管理各种关卡间通用的整体事件
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawn_Gen : MonoBehaviour
{
    [Header("基本参数")]
    private ChestSpawn chestSpawn; //箱子生成器脚本，用于关卡结束时标记禁止生成箱子
    private GameManager GM;
    private AudioManager AM;
    private GameObject canvas; //当前场景的画布

    [Header("UI")]
    public GameObject enemiesText; //敌人剩余数量UI

    [Header("关卡进程")]
    public GameObject goSign; //电梯go标志
    public GameObject elevatorScene; //电梯场景
    public GameObject scoreBoard; //计分板
    private Vector3 goSignOffset = new Vector3(0.5f, 2.25f, 0); //电梯go标志相对于玩家初始位置的偏移量
    private Animator elevatorAnim; //底层电梯动画控制器（关卡结束时开门）
    private Animator elevatorInAnim; //顶层电梯动画控制器（进入电梯后关门）
    private Transform playerTrans; //监控玩家位置
    private Animator playerAnim; //玩家动画控制器

    void Awake()
    {
        enemiesText = GameObject.Find("EnemiesText");
        chestSpawn = GameObject.Find("ChestSpawn").GetComponent<ChestSpawn>();
        elevatorAnim = GameObject.Find("Elevator").GetComponent<Animator>();
        elevatorInAnim = GameObject.Find("ElevatorIn").GetComponent<Animator>();
        playerTrans = GameObject.Find("Player1").transform;
        playerAnim = GameObject.Find("Player1").GetComponent<Animator>();
        canvas = GameObject.Find("Canvas"); //获取当前场景的画布
    }

    void Start()
    {
        GM = GameManager.instance;
        AM = AudioManager.instance;
        GM.playerStartPosition = playerTrans.position; //记录玩家开始位置
        GM.levelClear = false;
        GM.enterElevator = false;
        GM.unbankedScore = GM.unbankedScoreTemp; //分数设置为上一关结束时的分数

        StartCoroutine(BGM()); //延时播放背景音乐
    }

    void FixedUpdate()
    {
        enemiesText.GetComponent<Text>().text = string.Format("{0:D3}", GM.spawnTotal + GM.existTotal);
        var flag = GM.levelClear;
        if (GM.spawnTotal + GM.existTotal == 0 && !flag) //剩余敌人数量为0，关卡结束
        {
            AM.StopLoop(); //停止循环并控制关卡BGM淡出
            AM.StartCoroutine(AM.LevelBGMout());
            GM.levelClear = true;
            chestSpawn.ChangeFlag(flag); //禁止箱子生成器生成
            elevatorAnim.SetBool("Clear", true); //电梯开门
            AM.TempSound(AM.elevatorOpen);
            Instantiate(goSign, GM.playerStartPosition + goSignOffset, goSign.transform.rotation); //生成go标志
        }
        if (flag && !GM.enterElevator) //如果关卡已结束且玩家未进入电梯，实时监控玩家位置
        {
            PlayerPos();
        }
    }

    void PlayerPos()
    {
        var xd = playerTrans.position.x - GM.playerStartPosition.x; //计算出玩家位置与初始位置的插值，以模糊判断
        var yd = playerTrans.position.y - GM.playerStartPosition.y;
        xd = Mathf.Abs(xd);
        yd = Mathf.Abs(yd);
        if (xd <= 0.1 && yd <= 0.1) //玩家进入电梯
        {
            GM.enterElevator = true; //设置GM的flag
            playerAnim.SetBool("Clear", true); //玩家停止行动，保持idle
            elevatorInAnim.SetBool("Clear", true); //顶层电梯关闭
            Destroy(GameObject.Find("GoSignF(Clone)")); //删除go标志
            Debug.Log(Camera.main.WorldToScreenPoint(GM.playerStartPosition));
            Invoke("LoadElevator", 1.3f); //待电梯关闭动画结束后添加电梯场景
        }
    }

    void LoadElevator() //控制GM加载电梯场景
    {
        Instantiate(elevatorScene, new Vector3(0, 0, 0), elevatorScene.transform.rotation);
        Invoke("LoadBoard", 1);
    }

    void LoadBoard() //生成计分板
    {
        Instantiate(scoreBoard, canvas.transform);
    }

    IEnumerator BGM() //一段时间后播放背景音乐
    {
        yield return new WaitForSeconds(1.8f);

        //根据不同楼层播放不同的BGM
        var m = (GM.currentFloor) % 10;
        if (m == 1 || m == 5 || m == 7 || m == 9)
        {
            AM.MusicPlay(AM.music1);
            AM.cor = AM.StartCoroutine(AM.LoopMusic1C()); //开启循环
        }
        else if(m == 2|| m == 4 || m == 8)
        {
            AM.MusicPlay(AM.music2);
            AM.cor = AM.StartCoroutine(AM.LoopMusic2()); //开启循环
        }
        else if (m == 3 || m == 6)
        {
            AM.MusicPlay(AM.musicC);
            AM.cor = AM.StartCoroutine(AM.LoopMusic1C()); //开启循环
        }
        else if (m == 0)
        {
            AM.MusicPlay(AM.musicB);
            AM.cor = AM.StartCoroutine(AM.LoopMusicB()); //开启循环
        }

        yield break;
    }
}