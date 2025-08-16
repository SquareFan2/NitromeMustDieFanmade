//控制主菜单的脚本
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private AudioManager AM;
    private GameManager GM;

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

    private void Awake()
    {
        MusicButton = transform.GetChild(6).gameObject.GetComponent<Button>(); //获取控制音效和音乐的按钮
        SoundButton = transform.GetChild(7).gameObject.GetComponent<Button>();
    }

    void Start()
    {
        AM = AudioManager.instance;
        GM = GameManager.instance;
        ButtonInit();

        if(AM.cor != null) AM.StopCoroutine(AM.cor); //停止未完成的协程
        AM.musicSource.Stop(); //停止之前的BGM
        AM.MusicPlay(AM.musicB); //播放BGM
        AM.cor = AM.StartCoroutine(AM.LoopMusicB()); //BGM循环
    }

    public void ButtonInSound()
    {
        AM.UISound(AM.buttonIn);
    }

    public void ButtonOutSound()
    {
        AM.UISound(AM.buttonOut);
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
