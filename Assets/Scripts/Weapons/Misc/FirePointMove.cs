using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePointMove : MonoBehaviour
{
    public Transform player; //获取墙壁碰撞体
    public Transform temp; //临时点位置，与开火点默认位置相同，用于在恒定位置发射射线，因为开火点位置可能会改变
    public LayerMask ground;
    public bool touch;
    private Vector2 touchPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsCheck();
        //产生碰撞时，开火点修正为碰撞交点,其他时间为默认点
        if (touch == true) transform.position = touchPoint; 
        else transform.localPosition = temp.localPosition;
    }

    void PhysicsCheck()
    {
        //从开火点向玩家延伸一条射线，与墙壁产生碰撞时，将开火点移动至交点，以防止子弹射进墙内
        var pos = new Vector3(player.localScale.x, 0, 0);
        var offset = new Vector2(player.localScale.x * 0.4f, 0); //略微偏移，留出容错
        RaycastHit2D hit = Physics2D.Raycast(temp.position, pos, 1f, ground);
        //Debug.DrawRay(temp.position, pos, Color.red, 0.1f);
        if (hit)
        {
            touch = true;
            touchPoint = hit.point + offset;
        }
        else
        {
            touch = false;
        }
    }
}
