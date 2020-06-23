using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------------------------
// file: IObstacle.cs
//
// author: Rishi Barnwal
// date: 06/18/2020
//
// summary: Abstract class that all obstacles need to inherit from. Functions will reset or unreset depending
//          on whether or not the player has passed a checkpoint
///-------------------------------------------------------------------------------------------------
///

public abstract class IObstacle : MonoBehaviour
{
    public bool isTriggered = false;

    public string id;
    //public abstract void ResetObstacle();
    public abstract void UnresetObstacle();

    protected void CreateID()
    {
        // Use object position as ID since no two obstacles should be at the same point
        id = (transform.position.x).ToString() + " " + (transform.position.y).ToString() + " " + (transform.position.z).ToString();

        // Add this obstacle to the dictionary
        CheckpointManager.obstacleDictionary.Add(id, this);
    }
}
