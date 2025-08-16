using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawn_1_1 : EnemySpawn
{
    [Header("敌人预制体")]
    public GameObject orangeNose;

    protected override void LevelSpawn()
    {
        if (timer >= 7 && GM.spawnTotal == 1)
        {
            SpawnDown(orangeNose, 0);
        }
    }
}
