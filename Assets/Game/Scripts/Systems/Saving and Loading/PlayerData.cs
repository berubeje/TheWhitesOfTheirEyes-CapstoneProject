using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float health;
    public float[] position;

    public int numberOfObstacles;
    public string[] obstaclesIDs;
    public bool[] areObstaclesTriggered;

    public string[] checkPointIDs;
    public bool[] areCheckpointsUsed;


    public PlayerData ()
    {
        health = CheckpointManager.Instance.playerHealth;

        position = new float[3];

        position[0] = CheckpointManager.Instance.lastCheckPointPosition.x;
        position[1] = CheckpointManager.Instance.lastCheckPointPosition.y;
        position[2] = CheckpointManager.Instance.lastCheckPointPosition.z;

        numberOfObstacles = CheckpointManager.Instance.savedObstacles.Count;

        obstaclesIDs = new string[numberOfObstacles];
        areObstaclesTriggered = new bool[numberOfObstacles];

        for(int i = 0; i < numberOfObstacles; i++)
        {
            obstaclesIDs[i] = CheckpointManager.Instance.savedObstacles[i].id;
            areObstaclesTriggered[i] = CheckpointManager.Instance.savedObstacles[i].isTriggered;
        }

        
    }
}
