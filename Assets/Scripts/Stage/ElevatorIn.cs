using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorIn : MonoBehaviour
{
    private AudioManager AM;

    void Start()
    {
        AM = AudioManager.instance;
    }

    public void OpenSound()
    {
        AM.TempSound(AM.elevatorOpen);
    }

    public void ArriveSound()
    {
        AM.TempSound(AM.elevatorArrive);
    }

    public void CloseSound()
    {
        AM.TempSound(AM.elevatorClose);
    }
}
