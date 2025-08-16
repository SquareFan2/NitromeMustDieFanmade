using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SceneElevator : MonoBehaviour //电梯场景预制件挂载脚本
{
    private Vector3 originPoint = new Vector3(-0.5f, -5f, 0); //电梯位于正中时玩家的位置

    void Start()
    {
        var offset = originPoint - GameManager.instance.playerStartPosition; //用玩家生成位置和正中时的玩家位置计算出位置偏差
        transform.position -= offset; //将电梯初始位置设置为与当前关卡重叠的位置
        Invoke("AnimStart", 1); //渐变动画完成后开始位置变化
    }

    private void AnimStart() //电梯移动（迫真）
    {
        transform.DOMove(new Vector3(0, 0, 0), 2);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
