///-------------------------------------------------------------------------------------------------
// file: DetachFromParentScript.cs
//
// author: Jesse Berube
// date: 2020/07/17
//
// summary: The script detaches the gameobject from the parent on start. Mainly used to detatch the boss waypoints from the boss. The waypoints are attatched to the boss when the game is not playing to keep them in position while moving the boss in the editor.
///-------------------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachFromParentScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent = null;
    }

}
