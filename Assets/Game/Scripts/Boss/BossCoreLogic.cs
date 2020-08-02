///-------------------------------------------------------------------------------------------------
// file: BossCoreLogic.cs
//
// author: Jesse Berube
// date: 2020/07/31
//
// summary: This will show the credits after the core as been pulled out for a certain amount of time
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoreLogic : MonoBehaviour
{
    public float secondsTillCredits = 3.0f;

    private float currentTime = 0.0f;
    private RopeAnchorPoint ropeAnchorPoint;


    // Start is called before the first frame update
    void Start()
    {
        ropeAnchorPoint = GetComponent<RopeAnchorPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ropeAnchorPoint.pullDone)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= secondsTillCredits)
            {
                InputManager.Instance.currentGameState = InputManager.GameStates.GameFinished;
                this.enabled = false;
            }
        }
    }
}
