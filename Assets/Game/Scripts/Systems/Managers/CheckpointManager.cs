using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    public JimController player;

    public static Dictionary<string, IObstacle> obstacleDictionary;

    public static List<IObstacle> savedObstacles;

    public static float playerHealth;
    public static Vector3 lastCheckPointPosition;

    private void Awake()
    {
        applicationClosing = false; 
        obstacleDictionary = new Dictionary<string, IObstacle>();
        savedObstacles = new List<IObstacle>();
    }

    private void Start()
    {
        //PlayerData data = SaveLoadSystem.LoadPlayerData();

        //if (data != null)
        //{
        //    LoadCheckpoint(data);
        //}
    }

    public void LoadCheckpoint(PlayerData data)
    {
        
        for(int i = 0; i < data.numberOfObstacles; i++)
        {
            // Go through each obstacle and check if it was triggered
            IObstacle currentObstacle = obstacleDictionary[data.obstaclesIDs[i]];

            savedObstacles.Add(currentObstacle);
            
            if (data.areObstaclesTriggered[i])
            {
                // If it was, return it to its activated state
                currentObstacle.UnresetObstacle();
            }
        }



        //Set the player to the position of the last checkpoint
        Vector3 lastCheckpointPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
        player.transform.position = lastCheckpointPosition;

    }

    public void SaveCheckpoint(Vector3 triggerPosition, float health)
    {
        lastCheckPointPosition = triggerPosition;
        playerHealth = health;

        SaveLoadSystem.SavePlayerData();
    }

}
