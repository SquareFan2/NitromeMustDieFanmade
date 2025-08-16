using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour
{
    private GameObject control;
    private GameObject crates;
    private GameObject revive;
    private GameObject[] arr; //数组中存放三个帮助页面
    private int c = 0;

    private void Awake()
    { //获取三个子物体，即三个帮助页面
        control = transform.GetChild(0).gameObject;
        crates = transform.GetChild(1).gameObject;
        revive = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        arr = new GameObject[3] {control, crates, revive};
    }

    public void Prev() //向前切换
    {
        var temp = c;
        c--;
        if (c == -1) c = 2;
        arr[temp].SetActive(false);
        arr[c].SetActive(true);
    }
    public void Next() //向后切换
    {
        var temp = c;
        c++;
        if (c == 3) c = 0;
        arr[temp].SetActive(false);
        arr[c].SetActive(true);
    }
}
