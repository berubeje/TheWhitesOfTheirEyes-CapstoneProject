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
    public float secondsforFade = 3.0f;
    public float secondsTillCreditsAfterFade = 2.0f;

    public Material bossCoreMaterial;

    public Vector3 impulseForce = new Vector3(0, 0, 10);

    private float _currentTime = 0.0f;
    private bool _faded = false;


    private void Awake()
    {
        Color newColor = bossCoreMaterial.color;

        newColor.a = 1;

        bossCoreMaterial.color = newColor;
    }

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
        // Fade the core out once its pulled. Once the core is faded, show credits after a few seconds.
        if (_faded == false)
        {
            Color newColor = bossCoreMaterial.color;

            if (secondsforFade > 0.0f)
            {
                newColor.a -= Time.deltaTime / secondsforFade;
            }
            else
            {
                newColor.a -= Time.deltaTime;
            }

            bossCoreMaterial.color = newColor;


            if (newColor.a <= 0.0f)
            {
                _faded = true;
                _currentTime = 0.0f;
            }

        }
        else
        {
            _currentTime += Time.deltaTime;

            if (_currentTime >= secondsTillCreditsAfterFade)
            {
                InputManager.Instance.currentGameState = InputManager.GameStates.GameFinished;
                this.enabled = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        Color newColor = bossCoreMaterial.color;

        newColor.a = 1;

        bossCoreMaterial.color = newColor;
    }
}
