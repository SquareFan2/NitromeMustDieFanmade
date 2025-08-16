using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtEffect : MonoBehaviour
{
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
