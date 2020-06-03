using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public List<ControllableBase> controllables;

    private MethodInfo[] methodInfo;

    private void Start()
    {
        // Get all the methods in the ControllableBase class
        methodInfo = typeof(ControllableBase).GetMethods();    
    }

    void Update()
    {
        foreach(ControllableBase cb in controllables)
        {
            foreach (MethodInfo mi in methodInfo)
            {
                // Invoke every method from the ControllableBase class in the player object
                cb.Invoke(mi.Name, 0.0f);
            }
        }
        
    }

}
