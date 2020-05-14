using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableBase : MonoBehaviour
{
    public virtual void LeftAnalogStick() { }
    public virtual void RightAnalogStick() { }
    public virtual void NorthFaceButton() { }
    public virtual void EastFaceButton() { }
    public virtual void SouthFaceButton() { }
    public virtual void WestFaceButton() { }
    public virtual void LeftShoulderButton() { }
    public virtual void LeftTriggerButton() { }
    public virtual void RightShoulderButton() { }
    public virtual void RightTriggerButton() { }
    public virtual void RightTriggerButtonUp() { }
}
