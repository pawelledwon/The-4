using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public string levelName;
    public float[] checkPointPosition;

    public PlayerData(NetworkPlayer player)
    {
        levelName = SceneManager.GetActiveScene().name;

        checkPointPosition = new float[3];

        checkPointPosition[0] = player.checkPoint.x;
        checkPointPosition[1] = player.checkPoint.y;
        checkPointPosition[2] = player.checkPoint.z;
    }
}
