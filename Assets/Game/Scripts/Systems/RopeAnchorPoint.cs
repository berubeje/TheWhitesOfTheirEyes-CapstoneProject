using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RopeAnchorPoint : MonoBehaviour
{

    public AnchorType anchorType;
    public enum AnchorType
    {
        Swing,
        Pull
    }

    public float pullSpeed;

}
