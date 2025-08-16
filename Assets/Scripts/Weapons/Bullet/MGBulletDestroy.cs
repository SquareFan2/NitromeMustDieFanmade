using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGBulletDestroy : MonoBehaviour
{
    private void Start()
    {
        int i = Random.Range(0, 5); //随机生成1~4，对应四种销毁效果W
        switch (i)
        {
            case 1:break;
            case 2: gameObject.GetComponent<Animator>().SetTrigger("Random_2"); break;
            case 3: gameObject.GetComponent<Animator>().SetTrigger("Random_3"); break;
            case 4: gameObject.GetComponent<Animator>().SetTrigger("Random_4"); break;
            default:break;
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}