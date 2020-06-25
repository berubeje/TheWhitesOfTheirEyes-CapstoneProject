using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------------------------
// file: PlayerData.cs
//
// author: Rishi Barnwal
// date: 06/21/2020
//
// summary: A serializable class to save and load player data
///-------------------------------------------------------------------------------------------------
///

[System.Serializable]
public class PlayerData
{
    public float health;
    public float[] position;

    public int numberOfObstacles;
    public string[] obstaclesIDs;
    public bool[] areObstaclesTriggered;

    public PlayerData ()
    {
        health = CheckpointManager.playerHealth;

        position = new float[3];

        position[0] = CheckpointManager.lastCheckPointPosition.x;
        position[1] = CheckpointManager.lastCheckPointPosition.y;
        position[2] = CheckpointManager.lastCheckPointPosition.z;

        numberOfObstacles = CheckpointManager.obstacleDictionary.Count;

        obstaclesIDs = new string[numberOfObstacles];
        areObstaclesTriggered = new bool[numberOfObstacles];
        
        int i = 0;
        
        foreach(KeyValuePair<string, IObstacle> item in CheckpointManager.obstacleDictionary)
        {
            obstaclesIDs[i] = item.Key;
            areObstaclesTriggered[i] = item.Value.isTriggered;
            i++;
        }

        
    }
}
