 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///-------------------------------------------------------------------------------------------------
// file: FallingRockObstacle.cs
//
// author: Rishi Barnwal
// date: 06/18/2020
//
// summary: Overrides of the IObstacle abstract class to reset the falling rock trap
///-------------------------------------------------------------------------------------------------
///

public class FallingRockObstacle : IObstacle
{
    public GameObject rock;
    public GameObject trigger;

    public Vector3 initialRockPosition;

    //public override void ResetObstacle()
    //{
    //    trigger.SetActive(true);
    //    rock.transform.localPosition = initialRockPosition;
    //    rock.GetComponent<Rigidbody>().useGravity = false;
    //    isTriggered = false;
    //}
    private void Start()
    {
        CreateID();
    }

    public override void UnresetObstacle()
    {
        trigger.SetActive(false);
        rock.GetComponent<Rigidbody>().useGravity = true;

        Physics.Raycast(rock.transform.position, Vector3.down, out RaycastHit hit);

        Vector3 yOffset = new Vector3(0, transform.localScale.y / 4, 0);
        rock.transform.position = hit.point + yOffset;
        isTriggered = true;
    }
}
