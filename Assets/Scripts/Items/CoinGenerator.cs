using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public GameObject coin;

    void CoinGenerate(int n) //n表示生成硬币数量
    {
        var j = 180 / (n+1);
        for(int k = 1; k <= n; k++)
        {
            GameObject o = Instantiate(coin, transform.position, coin.transform.rotation);
            Rigidbody2D rb = o.GetComponent<Rigidbody2D>();
            var x = Mathf.Cos(j * k * Mathf.Deg2Rad);
            var y = Mathf.Sin(j * k * Mathf.Deg2Rad);
            rb.AddForce(new Vector2(x * 10, y * 14), ForceMode2D.Impulse);
            //Debug.Log(k + ", " + j * k + ", " + new Vector2(Mathf.Cos(j * k * Mathf.Deg2Rad), Mathf.Sin(j * k * Mathf.Deg2Rad)));
        }
        Destroy(gameObject);
    }
}
