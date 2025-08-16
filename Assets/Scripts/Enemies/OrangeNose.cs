using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeNose : Enemy
{
    protected override void Start()
    {
        base.Start();
        speed = 1.8f; //速度
        hp = 50; //血量
        toPlayerDamage = 2; //对玩家伤害量
        cl = new Color(0.952f, 0.4f, 0, 1); //死亡液体颜色
        dripCount = 5; //死亡时溅出的水滴数量（浮动线）
        coinCount = 3; //死亡时掉落金币数量
        offset = new Vector2(0.3125f, 0f); //地面判定线间距（半径）
        lenWall = 0.4f; //墙壁射线长度
        offsetWall = new Vector2(0, 0.5f); //墙壁检测射线离底边的距离
    }

    protected override void Movement()
    {
        base.Movement();

        if (isGround && getHurt == 0) //施加速度
        {
            rb.velocity = new Vector2(face * speed, rb.velocity.y);
        }

        if (face < faceDir) //转身时速度渐变
        {
            face += Time.deltaTime * 3.5f;
        }
        else if (face > faceDir)
        {
            face -= Time.deltaTime * 3.5f;
        }
    }

    protected override void GroundCheck()
    {
        base.GroundCheck();
        if (leftCheckGround || rightCheckGround)
        {
            if (!leftCheckGround) //当左或右射线没有判定时，证明行走至平台边界，调转方向
            {
                faceDir = 1f;
            }
            else if (!rightCheckGround)
            {
                faceDir = -1f;
            }
        }
    }
}
