using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnchorPoint : MonoBehaviour
{
    public AnchorType anchorType;
    public enum AnchorType
    {
        Swing,
        Pull
    }
}
