using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float waitTime = 0.2f;
    public LayerMask platform; //获取平台层
    public Rigidbody2D rb; //获取角色的刚体
    private KeyCode jumpKey;
    private KeyCode downKey;

    void Awake()
    {
        effector = GetComponent<PlatformEffector2D>();
    }


    private void Start()
    {
        jumpKey = GameManager.instance.jumpKey;
        downKey = GameManager.instance.downKey;
    }
    void FixedUpdate()
    {
        if (rb) //防止角色死亡后依然访问
        {
            Movement();
        }
    }

    void Movement()
    {
        if (rb.velocity.y > 0)
        {
            effector.rotationalOffset = 0;
        }
        else if (Input.GetKey(downKey) && !Input.GetKey(jumpKey)) //跳跃时保证平台可以穿过
        {
            if (waitTime <= 0)//等待0.2秒后才下落
            {
                effector.rotationalOffset = 180f;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
        else //下落开始后0.1秒内无法取消
        {
            if (waitTime <= 0 && waitTime >= -0.1f)
            {
                waitTime -= Time.deltaTime;
            }
            else
            {
                waitTime = 0.2f;
                effector.rotationalOffset = 0;
            }
        }
    }
}
