using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; //静态实例，可在其他脚本访问

    public GameManager GM;
    public Coroutine cor;

    private float musicVolume = 0.6f; //BGM音量数值

    [Header("玩家音效")]
    public AudioClip jump;
    public AudioClip hurt1_1;
    public AudioClip hurt1_2;
    public AudioClip hurt1_3;
    public AudioClip hurt1_4;
    public AudioClip[] hurt1;
    public AudioClip dead1_1;
    public AudioClip dead1_2;
    public AudioClip[] dead1;
    public AudioClip danger1;

    [Header("敌人音效")]
    public AudioClip getBullet;
    public AudioClip enemyDeath;

    [Header("武器音效")]
    public AudioClip pistolFire;
    public AudioClip machineGunFire;
    public AudioClip outOfAmmoFire;
    public AudioClip ShotgunFire;

    [Header("物品音效")]
    public AudioClip coin;
    public AudioClip ammoChest;
    public AudioClip hpChest;

    [Header("场景音效")]
    public AudioClip elevatorOpen;
    public AudioClip elevatorArrive;
    public AudioClip elevatorClose;

    [Header("UI音效")]
    public AudioClip buttonIn;
    public AudioClip buttonOut;
    public AudioClip clear_1;
    public AudioClip clear_2;
    public AudioClip clear_3;
    public AudioClip clear_4;
    public AudioClip clear_5;
    public AudioClip clear_6;
    public AudioClip[] clear;

    [Header("音乐")]
    public AudioClip music1;
    public AudioClip music2;
    public AudioClip musicC;
    public AudioClip musicB;
    public AudioClip musicE;

    [Header("音源")]
    public AudioSource tempSource;
    public AudioSource playerSource;
    public AudioSource player1DangerSource;
    public AudioSource enemySource;
    public AudioSource gunSource;
    public AudioSource itemSource;
    public AudioSource UISource;
    public AudioSource musicSource;

    public int a = 0;

    private void Awake()
    {
        if (instance != null) //防止重复添加
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(this); //切换场景时不会销毁

        tempSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        player1DangerSource = gameObject.AddComponent<AudioSource>();
        enemySource = gameObject.AddComponent<AudioSource>();
        gunSource = gameObject.AddComponent<AudioSource>();
        itemSource = gameObject.AddComponent<AudioSource>();
        UISource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        GM = GameManager.instance;
        hurt1 = new AudioClip[4] { hurt1_1, hurt1_2, hurt1_3, hurt1_4 };
        dead1 = new AudioClip[2] { dead1_1, dead1_2 };
        clear = new AudioClip[3] { clear_1, clear_2, clear_3 };

        instance.player1DangerSource.clip = danger1;

        musicSource.volume = musicVolume; //降低BGM音量
        //musicSource.loop = true; //音乐Source循环播放
    }

    public void TempSound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.tempSource.clip = clip;
            instance.tempSource.PlayOneShot(clip);
        }
    }

    public void PlayerSound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.playerSource.clip = clip;
            instance.playerSource.PlayOneShot(clip);
        }
    }
    public void EnemySound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.enemySource.clip = clip;
            instance.enemySource.PlayOneShot(clip);
        }
    }
    public void GunSound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.gunSource.clip = clip;
            instance.gunSource.PlayOneShot(clip);
        }
    }

    public void ItemSound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.itemSource.clip = clip;
            instance.itemSource.PlayOneShot(clip);
        }
    }

    public void UISound(AudioClip clip)
    {
        if (!GM.soundMute)
        {
            instance.UISource.clip = clip;
            instance.UISource.PlayOneShot(clip);
        }
    }

    public void MusicPlay(AudioClip clip)
    {
        instance.musicSource.volume = musicVolume; //防止淡出未加载完导致音量异常
        instance.musicSource.clip = clip;
        instance.musicSource.PlayOneShot(clip);
    }

    public IEnumerator LoopMusicB() //Boss音乐循环
    {
        while (true)
        {
            yield return new WaitForSeconds(59.2f);
            musicSource.PlayOneShot(musicB);
        }
    }

    public IEnumerator LoopMusic1C() //挑战&第一关卡音乐循环
    {
        while (true)
        {
            yield return new WaitForSeconds(87.4f);
            musicSource.PlayOneShot(musicSource.clip);
        }
    }

    public IEnumerator LoopMusic2() //第二关卡音乐循环
    {
        while (true)
        {
            yield return new WaitForSeconds(93);
            musicSource.PlayOneShot(music2);
        }
    }

    public IEnumerator LoopMusicE() //电梯音乐循环
    {
        while (true)
        {
            yield return new WaitForSeconds(39.4f);
            musicSource.PlayOneShot(musicE);
        }
    }

    public void StopLoop() //停止循环
    {
        StopCoroutine(cor);
    }

    public IEnumerator BGMout() //BGM淡出
    {
        for (float i = musicVolume; i >= 0; i -= 0.1f)
        {
            musicSource.volume = i;
            yield return new WaitForSeconds(0.2f);
        }
        musicSource.Stop(); //停止播放BGM
        musicSource.volume = musicVolume; //恢复音量
        StopLoop(); //停止循环
        yield break;
    }

    public IEnumerator LevelBGMout() //关卡结束后BGM淡出（比普通淡出略慢）
    {
        for (float i = musicVolume; i >= 0; i -= 0.025f)
        {
            musicSource.volume = i;
            yield return new WaitForSeconds(0.13f);
        }
        musicSource.Stop(); //停止播放BGM
        musicSource.volume = musicVolume; //恢复音量
        StopLoop(); //停止循环
        yield break;
    }
}
