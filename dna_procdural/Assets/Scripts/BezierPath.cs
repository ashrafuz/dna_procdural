using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPath : MonoBehaviour {
    [SerializeField] private Transform[] m_ControlPoints = new Transform[4];
    [SerializeField][Range (0, 1)] float sliderTest = 0;

    Vector3 GetPos (int i) => m_ControlPoints[i].position;

    private void OnDrawGizmos () {
        Handles.DrawBezier (
            GetPos (0),
            GetPos (3),
            GetPos (1),
            GetPos (2),
            Color.white, EditorGUIUtility.whiteTexture, 2);


        BezierPoint bpAlongPath = Maths.GetBezierPointAlongPath(sliderTest, m_ControlPoints);

        Gizmos.DrawWireSphere (bpAlongPath.position, 0.2f);
        Handles.PositionHandle(bpAlongPath.position, bpAlongPath.rotation);

        Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld(Vector3.right * 2), 0.2f);
        Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld(Vector3.right * -2), 0.2f);
    }

}