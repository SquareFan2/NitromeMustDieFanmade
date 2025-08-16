using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIManager : MonoBehaviour //挂载在Canvas上的脚本
{
    private GameManager GM;
    private AudioManager AM;
    [Header("血条")]
    public GameObject hPiece; //血条小块
    public GameObject[] pieceArray = new GameObject[50]; //血条小块数组，用于加减血量
    private Animator healthbarAnim; //血条动画控制器，血量低时切换动画
    public Sprite music;
    public Sprite musicYellow;
    public Sprite musicMute;
    public Sprite musicMuteYellow;
    public Sprite sound;
    public Sprite soundYellow;
    public Sprite soundMute;
    public Sprite soundMuteYellow;
    private Button MusicButton;
    private Button SoundButton;
    [Header("关卡内UI")]
    private Text scoreText; //分数UI
    private int scoreTemp = 0; //临时计数器，用于设置分数数字的过渡动画
    [Header("暂停菜单")]
    public GameObject pauseMenu;

    private void Awake()
    {
        healthbarAnim = transform.Find("HealthBar").GetComponent<Animator>(); //获取血条动画控制器
        scoreText = transform.Find("Score").GetComponent<Text>(); //获取分数UI
        MusicButton = transform.Find("LevelOptions").transform.GetChild(1).gameObject.GetComponent<Button>(); //获取控制音效和音乐的按钮
        SoundButton = transform.Find("LevelOptions").transform.GetChild(2).gameObject.GetComponent<Button>();
    }

    void Start()
    {
        GM = GameManager.instance;
        AM = AudioManager.instance;
        HPUIGenerate(); //生成血条
        ChangeFloorUI(); //更改楼层UI
        ButtonInit();
    }

    private void FixedUpdate()
    {
        ScoreChange();
    }

    void HPUIGenerate() //根据当前生命生成血条
    {
        for (int i = 0; i < GM.hp; i++)
        {
            GameObject piece = Instantiate(hPiece, new Vector2(-98 + i * 4, 451), hPiece.transform.rotation);
            piece.transform.SetParent(transform, false); //将生成的血条的父对象绑定为HP文本
            var count = transform.childCount; //画布的子物体总数
            piece.transform.SetSiblingIndex(count - 2); //将层级顺序设置为倒数第二个，防止遮挡住黑幕
            pieceArray[i] = piece; //将生成的血条加入数组
        }
    }

    public void HPUIChange(int c) //根据血量变化更改UI
    {
        if (c < 0) //扣血
        {
            for (int i = GM.hp; i < GM.hp - c; i++) //将扣掉部分的血块UI删除
            {
                Destroy(pieceArray[i]);
            }
        }
        else //加血
        {
            for (int i = GM.hp - c; i < GM.hp; i++) //重新生成增加的血量
            {
                GameObject piece = Instantiate(hPiece, new Vector2(-98 + i * 4, 451), hPiece.transform.rotation);
                piece.transform.SetParent(transform, false); //将生成的血条的父对象绑定为HP文本
                var count = transform.childCount; //画布的子物体总数
                piece.transform.SetSiblingIndex(count - 2); //将层级顺序设置为倒数第二个，防止遮挡住黑幕
                pieceArray[i] = piece; //将生成的血条加入数组
            }
        }

        if (GM.hp <= 10) //根据HP值判断是否改变血条动画
        {
            healthbarAnim.SetBool("Low", true);
        }
        else
        {
            healthbarAnim.SetBool("Low", false);
        }
    }

    private void ScoreChange() //分数变化函数，此函数放入FixedUpdate中，分数每个fix帧只能变化1
    {
        if (scoreTemp != GM.unbankedScore)
        {
            if (scoreTemp < GM.unbankedScore)
            {
                scoreText.text = string.Format("{0:0,000,000}", ++scoreTemp); //更改分数并反映至UI
            }
            else
            {
                scoreText.text = string.Format("{0:0,000,000}", --scoreTemp); //更改分数并反映至UI
            }
        }
    }

    public void ChangeFloorUI()
    {
        //获取子物体LevelProgress的子物体FloorText的文本
        var FloorText = transform.Find("LevelProgress").transform.Find("FloorText").GetComponent<Text>();
        FloorText.text = GM.currentFloor.ToString("D3"); //更改楼层数
    }

    public void GamePause() //切换暂停
    {
        if(GM.gamePaused == false)
        {
            Time.timeScale = 0;
            var p = Instantiate(pauseMenu, transform);
            var count = transform.childCount; //画布的子物体总数
            p.transform.SetSiblingIndex(count - 2); //将层级顺序设置为倒数第二个，防止遮挡住黑幕
            GM.gamePaused = true;
        }
        else
        {
            Time.timeScale = 1;
            Destroy(transform.Find("PauseMenu(Clone)").gameObject);
            GM.gamePaused = false;
        }
    }

    public void toggleSoundMute() //音效开关切换
    {
        if (!GM.soundMute)
        {
            GM.soundMute = true;
            SoundButton.image.sprite = soundMute;
            SpriteState spriteState = new SpriteState(); //通过SpriteState类更改悬浮和按下的效果
            spriteState.highlightedSprite = soundMuteYellow;
            spriteState.pressedSprite = soundMuteYellow;
            SoundButton.spriteState = spriteState;
        }
        else
        {
            GM.soundMute = false;
            SoundButton.image.sprite = sound;
            SpriteState spriteState = new SpriteState(); //通过SpriteState类更改悬浮和按下的效果
            spriteState.highlightedSprite = soundYellow;
            spriteState.pressedSprite = soundYellow;
            SoundButton.spriteState = spriteState;
        }
    }

    public void toggleMusicMute() //音乐开关切换
    {
        if (!GM.musicMute)
        {
            GM.musicMute = true;
            AM.musicSource.volume = 0; //将音乐Source音量改为0
            MusicButton.image.sprite = musicMute;
            SpriteState spriteState = new SpriteState(); //通过SpriteState类更改悬浮和按下的效果
            spriteState.highlightedSprite = musicMuteYellow;
            spriteState.pressedSprite = musicMuteYellow;
            MusicButton.spriteState = spriteState;
        }
        else
        {
            GM.musicMute = false;
            AM.musicSource.volume = 0.6f; //将音乐Source音量改为初始值
            MusicButton.image.sprite = music;
            SpriteState spriteState = new SpriteState(); //通过SpriteState类更改悬浮和按下的效果
            spriteState.highlightedSprite = musicYellow;
            spriteState.pressedSprite = musicYellow;
            MusicButton.spriteState = spriteState;
        }
    }

    private void ButtonInit() //根据当前情况对音效/音乐按钮初始化
    {
        if (GM.soundMute)
        {
            SoundButton.image.sprite = soundMute;
            SpriteState spriteState = new SpriteState();
            spriteState.highlightedSprite = soundMuteYellow;
            spriteState.pressedSprite = soundMuteYellow;
            SoundButton.spriteState = spriteState;
        }
        if (GM.musicMute)
        {
            MusicButton.image.sprite = musicMute;
            SpriteState spriteState = new SpriteState();
            spriteState.highlightedSprite = musicMuteYellow;
            spriteState.pressedSprite = musicMuteYellow;
            MusicButton.spriteState = spriteState;
        }
    }
}
