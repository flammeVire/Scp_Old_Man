using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Papy_Vision))]
public class FovPapyEditor : Editor
{
    private void OnSceneGUI()
    {
        Papy_Vision fov = (Papy_Vision)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.raduis);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.raduis);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.raduis);

        if (fov.canSeePlayer)
        {
            Handles.color = Color.yellow;
          //  Handles.DrawLine(fov.transform.position, fov.playerMesh.transform.position);
        }
    }


    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
