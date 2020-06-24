///-------------------------------------------------------------------------------------------------
// file: DollyCameraManager.cs
//
// author: Jesse Berube
// date: 06/24/2020
//
// summary: Keeps track of the current dolly track, as well as switch tracks
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCameraManager : Singleton<DollyCameraManager>
{
    public GameObject currentDolly;

    public void ChangeDolly(GameObject newDolly)
    {
        currentDolly.SetActive(false);
        newDolly.SetActive(true);
        currentDolly = newDolly;
    }
}
