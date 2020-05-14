using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public ControllableBase player;

    private MethodInfo[] methodInfo;

    private void Start()
    {
        // Get all the methods in the ControllableBase class
        methodInfo = typeof(ControllableBase).GetMethods();    
    }

    void Update()
    {
        foreach(MethodInfo mi in methodInfo)
        {
            // Invoke every method from the ControllableBase class in the player object
            player.Invoke(mi.Name, 0.0f);
        }
    }
}
