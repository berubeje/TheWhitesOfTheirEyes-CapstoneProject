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

        numberOfObstacles = CheckpointManager.savedObstacles.Count;

        obstaclesIDs = new string[numberOfObstacles];
        areObstaclesTriggered = new bool[numberOfObstacles];

        for(int i = 0; i < numberOfObstacles; i++)
        {
            obstaclesIDs[i] = CheckpointManager.savedObstacles[i].id;
            areObstaclesTriggered[i] = CheckpointManager.savedObstacles[i].isTriggered;
        }

        
    }
}
