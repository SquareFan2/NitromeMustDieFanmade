using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerOption : MonoBehaviour //主菜单玩家设置UI界面挂载脚本
{
    private GameObject button;
    private Text keyText;
    private Text keyTextShadow;
    private Transform keyChild;

    private void Awake()
    {
        keyChild = transform.Find("Keys");
    }

    private void Start()
    {
        Initialization();
    }

    private void Initialization() //初始化按键名称
    {
        keyChild.GetChild(1).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.fireKey.ToString();
        keyChild.GetChild(1).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.fireKey.ToString();
        keyChild.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.jumpKey.ToString();
        keyChild.GetChild(2).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.jumpKey.ToString();
        keyChild.GetChild(3).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.upKey.ToString();
        keyChild.GetChild(3).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.upKey.ToString();
        keyChild.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.downKey.ToString();
        keyChild.GetChild(4).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.downKey.ToString();
        keyChild.GetChild(5).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.leftKey.ToString();
        keyChild.GetChild(5).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.leftKey.ToString();
        keyChild.GetChild(6).transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.rightKey.ToString();
        keyChild.GetChild(6).transform.GetChild(3).GetComponent<Text>().text = GameManager.instance.rightKey.ToString();
    }

    public void OnClick() //更改按键函数
    {
        button = EventSystem.current.currentSelectedGameObject;
        keyText = button.transform.Find("KeyText").GetComponent<Text>(); //获取显示文本
        keyTextShadow = button.transform.Find("KeyTextShadow").GetComponent<Text>();
        keyText.text = "Press a key";
        keyTextShadow.text = "Press a key";
    }

    private void OnGUI()
    {
        if (button)
        {
            Event e = Event.current; //获取当前的输入事件
            if (e != null)
            {
                if(e.isKey && e.keyCode != KeyCode.None)
                {
                    FindSame(e.keyCode);
                    switch (button.name)
                    {
                        case "Fire": GameManager.instance.fireKey = e.keyCode; break;
                        case "Jump": GameManager.instance.jumpKey = e.keyCode; break;
                        case "Up": GameManager.instance.upKey = e.keyCode; break;
                        case "Down": GameManager.instance.downKey = e.keyCode; break;
                        case "Left": GameManager.instance.leftKey = e.keyCode; break;
                        case "Right": GameManager.instance.rightKey = e.keyCode; break;
                        default: break;
                    }
                    keyText.text = e.keyCode.ToString();
                    keyTextShadow.text = e.keyCode.ToString();
                    EventSystem.current.SetSelectedGameObject(null); //取消按钮的选中
                    button = null;
                }
                else if (e.isMouse) //鼠标判定，在0~6之间循环判定
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if (Input.GetMouseButton(i))
                        {
                            FindSame(KeyCode.Mouse0 + i);
                            switch (button.name)
                            {
                                case "Fire": GameManager.instance.fireKey = KeyCode.Mouse0 + i; break;
                                case "Jump": GameManager.instance.jumpKey = KeyCode.Mouse0 + i; break;
                                case "Up": GameManager.instance.upKey = KeyCode.Mouse0 + i; break;
                                case "Down": GameManager.instance.downKey = KeyCode.Mouse0 + i; break;
                                case "Left": GameManager.instance.leftKey = KeyCode.Mouse0 + i; break;
                                case "Right": GameManager.instance.rightKey = KeyCode.Mouse0 + i; break;
                                default: break;
                            }
                            keyText.text = (KeyCode.Mouse0 + i).ToString();
                            keyTextShadow.text = (KeyCode.Mouse0 + i).ToString();
                            EventSystem.current.SetSelectedGameObject(null); //取消按钮的选中
                            button = null;
                        }
                    }
                }
            }
        }
    }

    private void FindSame(KeyCode k) //查找与将要设置的键位冲突的键位，并将冲突的键位设置为当前按键原来的键位
    {
        KeyCode sk = KeyCode.None;
        switch (button.name)
        {
            case "Fire": sk = GameManager.instance.fireKey; break;
            case "Jump": sk = GameManager.instance.jumpKey; break;
            case "Up": sk = GameManager.instance.upKey; break;
            case "Down": sk = GameManager.instance.downKey; break;
            case "Left": sk = GameManager.instance.leftKey; break;
            case "Right": sk = GameManager.instance.rightKey; break;
            default: break;
        }
        if (GameManager.instance.fireKey == k && button.name != "Fire")
        {
            GameManager.instance.fireKey = sk;
            keyChild.GetChild(1).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(1).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
        if (GameManager.instance.jumpKey == k && button.name != "Jump")
        {
            GameManager.instance.jumpKey = sk;
            keyChild.GetChild(2).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(2).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
        if (GameManager.instance.upKey == k && button.name != "Up")
        {
            GameManager.instance.upKey = sk;
            keyChild.GetChild(3).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(3).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
        if (GameManager.instance.downKey == k && button.name != "Down")
        {
            GameManager.instance.downKey = sk;
            keyChild.GetChild(4).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(4).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
        if (GameManager.instance.leftKey == k && button.name != "Left")
        {
            GameManager.instance.leftKey = sk;
            keyChild.GetChild(5).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(5).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
        if (GameManager.instance.rightKey == k && button.name != "Right")
        {
            GameManager.instance.rightKey = sk;
            keyChild.GetChild(6).transform.GetChild(2).GetComponent<Text>().text = sk.ToString();
            keyChild.GetChild(6).transform.GetChild(3).GetComponent<Text>().text = sk.ToString();
        }
    }

    public void ResetKeys() //重置按键为默认
    {
        GameManager.instance.fireKey = KeyCode.Mouse0;
        GameManager.instance.jumpKey = KeyCode.Space;
        GameManager.instance.upKey = KeyCode.W;
        GameManager.instance.downKey = KeyCode.S;
        GameManager.instance.leftKey = KeyCode.A;
        GameManager.instance.rightKey  = KeyCode.D;
        Initialization();
    }
}
