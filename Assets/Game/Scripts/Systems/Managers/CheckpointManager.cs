using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    public JimController player;

    public Dictionary<string, IObstacle> obstacleDictionary;
    public IObstacle[] obstacles;
    public List<IObstacle> savedObstacles;
    public float playerHealth;
    public Vector3 lastCheckPointPosition;

    private void Awake()
    {
        // Find every obstacle in the level
        obstacles = FindObjectsOfType<IObstacle>();

        obstacleDictionary = new Dictionary<string, IObstacle>();

        foreach(IObstacle o in obstacles)
        {
            obstacleDictionary.Add(o.id, o);
        }

        PlayerData data = SaveLoadSystem.LoadPlayerData();

        if(data != null)
        {
            LoadCheckpoint(data);
        }

    }

    public void LoadCheckpoint(PlayerData data)
    {
        
        for(int i = 0; i < data.numberOfObstacles; i++)
        {
            IObstacle currentObstacle = obstacleDictionary[data.obstaclesIDs[i]];
            if (data.areObstaclesTriggered[i])
            {
                currentObstacle.UnresetObstacle();
            }
        }

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
