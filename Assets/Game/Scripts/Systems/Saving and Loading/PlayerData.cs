using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float health;
    public float[] position;

    public PlayerData ()
    {
        health = CheckpointManager.Instance.playerHealth;

        position = new float[3];

        position[0] = CheckpointManager.Instance.lastCheckPointPosition.x;
        position[1] = CheckpointManager.Instance.lastCheckPointPosition.y;
        position[2] = CheckpointManager.Instance.lastCheckPointPosition.z;
    }
}
