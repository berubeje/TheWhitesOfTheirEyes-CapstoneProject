using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : Singleton<CheckpointManager>
{
    public JimController jimController;

    public static Dictionary<string, IObstacle> obstacleDictionary;


    //public static IObstacle[] obstacles;

    public static float playerHealth;
    public static Vector3 lastCheckPointPosition;

    private Animator _jimAnimator;

    private void Awake()
    {
        applicationClosing = false; 
        obstacleDictionary = new Dictionary<string, IObstacle>();

    }

    public void LoadCheckpoint(PlayerData data)
    {
        for (int i = 0; i < obstacleDictionary.Count; i++)
        {
            // Go through each obstacle and check what its state was
            IObstacle currentObstacle = obstacleDictionary[data.obstaclesIDs[i]];

            if (!data.areObstaclesTriggered[i])
            {
                currentObstacle.ResetObstacle();
            }
            else
            {
                currentObstacle.UnresetObstacle();
            }
        }



        //Set the player to the position of the last checkpoint
        Vector3 lastCheckpointPosition = new Vector3(data.position[0], data.position[1], data.position[2]);
        jimController.transform.position = lastCheckpointPosition + new Vector3(0, 1, 0);
        jimController.currentHealth = data.health;

        _jimAnimator = jimController.GetComponent<Animator>();
        _jimAnimator.SetBool("dead", false);

    }

    public void SaveCheckpoint(Vector3 triggerPosition, float health)
    {
        lastCheckPointPosition = triggerPosition;
        playerHealth = health;

        SaveLoadSystem.SavePlayerData();
    }

}
