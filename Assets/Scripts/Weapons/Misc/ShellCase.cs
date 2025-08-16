using UnityEngine;

public class ShellCase : MonoBehaviour
{
    float Lifetime = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Lifetime >= 0)
        {
            Lifetime -= Time.deltaTime;
        }
        else Destroy(gameObject);
    }
}
