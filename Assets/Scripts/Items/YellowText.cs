using UnityEngine;

public class YellowText : MonoBehaviour
{
    private double frame = 0; //用于计时，每个FixedUpdate加0.025
    private double animV = 5; //运动的速度，根据加速度与距离公式推算出v = 2s，s固定为2.5
    private Vector3 spos; //物体初始位置
    private Vector3 offset;

    void Start()
    {
        spos = transform.position;
        offset = new Vector3(0, 2.5f, 0);
    }

    void FixedUpdate()
    {
        if (frame < 1.025)
        {
            double a = animV * frame * (1 - frame / 2); //根据加速度与距离公式计算出当前帧移动的距离
            a /= 2.5; //移动距离除以总距离，算出插值
            gameObject.transform.position = Vector2.Lerp(spos, spos + offset, (float)a);
        }
        if(frame >= 1.25)
        {
            Destroy(gameObject);
        }
        frame += 0.025;
    }
}
