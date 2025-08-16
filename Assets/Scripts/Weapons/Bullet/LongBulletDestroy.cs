using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongBulletDestroy : MonoBehaviour
{
    private void Start()
    {
        int i = Random.Range(0, 2); //随机生成1或0，如果是1则播放另一种动画效果
        if (i > 0)
        {
            gameObject.GetComponent<Animator>().SetBool("random", true);
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}