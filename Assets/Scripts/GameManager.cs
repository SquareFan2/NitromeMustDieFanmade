using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //静态实例，可在其他脚本访问

    [Header("按键")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode fireKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("整体数据")]
    [Tooltip("生命值")] public int hp = 50;
    [Tooltip("弹药数")] public int ammo = 0;
    [Tooltip("武器后坐力速度，平时为0")] public float subspeed = 0;
    [Tooltip("当前楼层")] public int currentFloor = 0;
    [Tooltip("楼层数组，共10个元素，储存关卡顺序")] public int[] floorArr;
    [Tooltip("通关状况，0代表通过0大关，1代表通过1大关，etc")] public int levelCleared = 0;
    [Tooltip("玩家在关卡开始时的位置")] public Vector3 playerStartPosition;
    [Tooltip("未储存分数")] public int unbankedScore = 0;
    [Tooltip("分数倍数")] public int multiplier = 1;
    [Tooltip("已储存分数")] public int bankedScore = 0;
    [Tooltip("关卡结束时更新，重来时用")] public int unbankedScoreTemp = 0;
    [Tooltip("当前武器编号")] public int currentWeapon = -1;

    [Header("关卡进程")]
    [Tooltip("转换场景前镜头的位置")] public Vector3 lastCamPos;
    [Tooltip("怪物全部击杀完毕flag")] public bool levelClear;
    [Tooltip("玩家进入电梯flag")] public bool enterElevator;
    [Tooltip("地图中的敌人总数")] public int existTotal;
    [Tooltip("待生成的敌人总数")] public int spawnTotal;

    [Header("UI")]
    [Tooltip("左上角UI楼层数")] public Text floorUI;
    [Tooltip("音乐是否静音")] public bool musicMute = false;
    [Tooltip("音效是否静音")] public bool soundMute = false;
    [Tooltip("是否暂停")] public bool gamePaused = false;

    private void Awake()
    {
        if(instance != null) //防止重复添加
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this); //切换场景时不会销毁
    }

    private void Start()
    {
        floorArr = new int[10];
    }

    //加载关卡场景
    public void LoadLevelStart() //运行关卡加载协程
    {
        StartCoroutine(LoadLevel());
    }

    public IEnumerator LoadLevel()
    {
        var index = floorArr[(currentFloor % 10) - 1] + 3;
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(index); //通过场景编号加载

        Debug.Log("Loading Scene " + index);
        foreach(int i in floorArr)
        {
            Debug.Log(floorArr[i]);
        }

        while (!sceneLoad.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator BackToMainMenu()
    {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(2); //通过场景编号加载

        while (!sceneLoad.isDone)
        {
            yield return null;
        }
    }

    public void HPUIChange(int c) //获取当前场景的Canvas挂载脚本，执行血量更改
    {
        var LUM = GameObject.Find("Canvas").GetComponent<LevelUIManager>();
        LUM.HPUIChange(c);
    }
}
