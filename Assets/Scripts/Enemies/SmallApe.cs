using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SmallApe : Enemy
{
    protected override void Start()
    {
        base.Start();
        speed = 0;
        hp = 100; //血量
        toPlayerDamage = 4; //对玩家伤害量
        cl = new Color(1, 0.925f, 0, 1); //死亡液体颜色
        dripCount = 6; //死亡时溅出的水滴数量（浮动线）
        coinCount = 5; //死亡时掉落金币数量
        offset = new Vector2(0.46875f, 0f); //地面判定线间距（半径）
        lenWall = 0.5f; //墙壁射线长度
        offsetWall = new Vector2(0, 0.5f); //墙壁检测射线离底边的距离
    }

    protected override void Movement()
    {
        pos = transform.position;

        rb.velocity = new Vector2(face * speed, rb.velocity.y); //赋值速度

        transform.localScale = new Vector2(-faceDir, 1); //调整面向

        //受伤判定
        if (getHurt > 0) //受伤时有4帧速度为0
        {
            speed = 0;
            DOTween.KillAll(); //终止所有Tween，否则将有缓存保持怪物的速度，无法实现重新缓慢加速
            getHurt--;
        }

        if (isGround && getHurt == 0) //施加速度
        {
            if (speed < 4.5f) //4.5为最大速度，当小于最大速度时怪物速度逐渐增加
            {
                DOTween.To(() => speed, x => speed = x, 4.5f, 0.5f);
            }
        }
    }

    protected override void GroundCheck()
    {
        base.GroundCheck();
        if (!leftCheckGround && !rightCheckGround)
        {
            DOTween.KillAll(); //终止所有Tween，否则将有缓存保持怪物的速度，无法实现重新缓慢加速
            DOTween.To(() => speed, x => speed = x, 0.5f, 0.3f);
        }
    }
}
