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
    public Vector3 impulseForce = new Vector3(0, 0, 10);

    private float currentTime = 0.0f;

    void Start()
    {
        transform.parent = null;

        Rigidbody rigidbody = GetComponent<Rigidbody>();

        rigidbody.isKinematic = false;

        rigidbody.AddRelativeForce(impulseForce, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= secondsTillCredits)
        {
            InputManager.Instance.currentGameState = InputManager.GameStates.GameFinished;
            this.enabled = false;
        }
    }
}
