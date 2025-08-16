using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private SpriteRenderer rend;
    public SpriteRenderer playerRend;
    public Material lightMat;
    public Material playerLightMat;
    public Material defaultMat;
    void Start()
    {
        rend = this.GetComponent<SpriteRenderer>();
    }
    void LightOn()
    {
        rend.material = lightMat;
        playerRend.material = playerLightMat;
    }
    void LightOff()
    {
        rend.material = defaultMat;
        playerRend.material = defaultMat;
    }
}
