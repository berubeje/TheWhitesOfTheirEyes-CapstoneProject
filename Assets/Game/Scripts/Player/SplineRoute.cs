using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SplineRoute : MonoBehaviour
{
    public Transform[] controlPoints;

    private Vector3 _gizmosPosition;

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.green;
        for (float t = 0.0f; t <= 1; t += 0.025f)
        {
            _gizmosPosition = Mathf.Pow(1 - t, 3) * controlPoints[0].position +
                3 * Mathf.Pow(1 - t, 2) * t * controlPoints[1].position +
                3 * (1 - t) * Mathf.Pow(t, 2) * controlPoints[2].position +
                Mathf.Pow(t, 3) * controlPoints[3].position;

            Gizmos.DrawSphere(_gizmosPosition, 0.05f);
        }

        Gizmos.color = Color.white;

        Gizmos.DrawLine(controlPoints[0].position, controlPoints[1].position);

        Gizmos.DrawLine(controlPoints[2].position, controlPoints[3].position);
    }
}
