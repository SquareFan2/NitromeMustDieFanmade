using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour //暂停界面预制件挂载脚本
{
    private GameManager GM;
    private AudioManager AM;
    private Animator canvasAnim;

    private void Start()
    {
        GM = GameManager.instance;
        AM = AudioManager.instance;
        canvasAnim = transform.parent.gameObject.GetComponent<Animator>();
    }

    public void Continue()
    {
        Time.timeScale = 1;
        GM.gamePaused = false;
        Destroy(gameObject);
        AM.UISound(AM.buttonIn);
    }

    public void StartAgain()
    {
        Time.timeScale = 1;
        GM.gamePaused = false;
        GM.currentWeapon = -1; //重置武器情况
        canvasAnim.SetTrigger("Out");

        AM.StopLoop(); //停止循环并控制BGM淡出
        AM.cor = AM.StartCoroutine(AM.BGMout());

        Invoke("LoadLevel", 1.5f);
        AM.UISound(AM.buttonIn);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        GM.gamePaused = false;
        GM.currentWeapon = -1;
        GM.unbankedScore = 0;
        GM.multiplier = 1;
        GM.hp = 50;
        canvasAnim.SetTrigger("Out");

        AM.StopLoop(); //停止循环并控制BGM淡出
        AM.cor = AM.StartCoroutine(AM.BGMout());

        Invoke("BackToMainMenu", 0.2f);
        AM.UISound(AM.buttonOut);
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void BackToMainMenu()
    {
        GM.StartCoroutine(GM.BackToMainMenu());
    }
}
