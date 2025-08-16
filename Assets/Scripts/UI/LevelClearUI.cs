using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelClearUI : MonoBehaviour //关卡通过时，左侧显示的结算UI挂载脚本
{
    private Animator anim; //控制UI淡出
    private Animator canvasAnim; //画布挂载的动画控制器，控制整个关卡淡出
    private AudioManager AM;

    [Header("分数板")]
    private Text UnbankedScore; //未储存分数
    private Text MultiplierScore; //倍率数字
    private Text TotalScore; //未储存分数 * 倍率
    private Text Mult; //选项中的“x 2”等数字
    private Text MultShadow;
    private Text BankedScore; //下方已储存分数

    [Header("选择完毕图片")]
    private GameObject Chosen;
    private Text ChosenText;
    private Text ChosenTextShadow;
    private GameObject GambleImage;
    private GameObject BankImage;

    [Header("准备提示图片")]
    private GameObject Ready;
    private Animator readyAnim; //准备按键挂载的动画控制器
    private Text ReadyText;
    private Text ReadyTextShadow;
    private GameManager GM;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        canvasAnim = GameObject.Find("Canvas").GetComponent<Animator>();

        var board = transform.Find("Board");
        var banked = transform.Find("BankedBoard");
        var up = transform.Find("Up");
        Chosen = transform.Find("Chosen").gameObject;
        Ready = transform.Find("Ready").gameObject;

        UnbankedScore = board.transform.Find("UnbankedScore").GetComponent<Text>();
        MultiplierScore = board.transform.Find("MultiplierScore").GetComponent<Text>();
        TotalScore = board.transform.Find("TotalScore").GetComponent<Text>();
        Mult = up.transform.Find("Mult").GetComponent<Text>();
        BankedScore = banked.transform.Find("BankedScore").GetComponent<Text>();
        MultShadow = up.transform.Find("MultShadow").GetComponent<Text>();

        ChosenText = Chosen.transform.Find("ChosenText").GetComponent<Text>();
        ChosenTextShadow = Chosen.transform.Find("ChosenTextShadow").GetComponent<Text>();
        GambleImage = Chosen.transform.Find("GambleImage").gameObject;
        BankImage = Chosen.transform.Find("BankImage").gameObject;

        readyAnim = Ready.GetComponent<Animator>();
        ReadyTextShadow = Ready.transform.GetChild(0).GetComponent<Text>();
        ReadyText = Ready.transform.GetChild(1).GetComponent<Text>();
    }

    void Start()
    {
        GM = GameManager.instance;
        AM = AudioManager.instance;
        UnbankedScore.text = GM.unbankedScore.ToString("D7");
        MultiplierScore.text = GM.multiplier.ToString("D2");
        TotalScore.text = (GM.unbankedScore * GM.multiplier).ToString("D7");
        Mult.text = "x " + (GM.multiplier + 1).ToString();
        BankedScore.text = GM.bankedScore.ToString("D7");
        MultShadow.text = Mult.text;
        StartCoroutine(UpDown());

        Invoke("BGM", 0.5f); //一段时间后播放BGM
    }

    void GetReady()
    {
        StopCoroutine(UpDown());
        StartCoroutine(WaitForReady());
    }

    IEnumerator UpDown()
    {
        while (true) //等待玩家按上/下键
        {
            if(Input.GetKey(GM.upKey)) //倍率+1并更新分数板
            {
                AM.TempSound(AM.clear[Random.Range(0,3)]); //播放音效

                GM.multiplier += 1;
                MultiplierScore.text = GM.multiplier.ToString("D2");
                TotalScore.text = (GM.unbankedScore * GM.multiplier).ToString("D7");
                Chosen.SetActive(true);
                GambleImage.SetActive(true);
                ChosenText.text = "Score Gambled!";
                ChosenTextShadow.text = ChosenText.text;
                Ready.SetActive(true);
                transform.Find("Up").gameObject.SetActive(false); //隐藏上下选择物体
                transform.Find("Or").gameObject.SetActive(false);
                transform.Find("Down").gameObject.SetActive(false);
                GetReady();
                yield break;
            }
            if (Input.GetKey(GM.downKey)) //将分数加至总分，倍率归1，并更新面板
            {
                AM.TempSound(AM.clear_6); //播放音效

                GM.bankedScore += GM.unbankedScore * GM.multiplier;
                GM.unbankedScore = 0;
                GM.multiplier = 1;
                UnbankedScore.text = "0000000";
                MultiplierScore.text = "01";
                TotalScore.text = "0000000";
                BankedScore.text = GM.bankedScore.ToString("D7");
                Chosen.SetActive(true);
                BankImage.SetActive(true);
                ChosenText.text = "Score Banked!";
                ChosenTextShadow.text = ChosenText.text;
                Ready.SetActive(true);
                transform.Find("Up").gameObject.SetActive(false); //隐藏上下选择物体
                transform.Find("Or").gameObject.SetActive(false);
                transform.Find("Down").gameObject.SetActive(false);
                GetReady();
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator WaitForReady()
    {
        while (true) //监测玩家按下开火键
        {
            if (Input.GetKey(GM.fireKey))
            {
                AM.TempSound(AM.buttonIn); //播放音效

                GM.unbankedScoreTemp = GM.unbankedScore; //分数更新
                readyAnim.SetBool("IsReady", true); //准备方框“√”
                ReadyText.text = "Player 1\nis ready!";
                ReadyTextShadow.text = ReadyText.text;
                anim.SetTrigger("Ready"); //UI淡出
                yield return new WaitForSeconds(2f); //UI消失后2秒，黑屏
                canvasAnim.SetTrigger("Out"); //黑屏动画
                GM.currentFloor += 1;
                GM.levelCleared += 1;

                AM.StopLoop(); //停止循环并控制BGM淡出
                AM.cor = AM.StartCoroutine(AM.BGMout());

                yield return new WaitForSeconds(1f); //黑屏1秒后加载下一关的场景
                GM.LoadLevelStart();
                yield break;
            }
            yield return null;
        }
    }

    private void BGM()
    {
        if (AM.cor != null) AM.StopCoroutine(AM.cor); //停止未完成的协程
        AM.musicSource.Stop(); //停止之前的BGM
        AM.MusicPlay(AM.musicE);
        AM.cor = AM.StartCoroutine(AM.LoopMusicE()); //开启循环
    }
}
