using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPoint {
    public Vector3 position;
    public Quaternion rotation;

    public Vector3 LocalToWorld( Vector3 localSpace){
        return this.position + rotation * localSpace;
    }
}

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


        BezierPoint bpAlongPath = GetBezierPointAlongPath(sliderTest);

        Gizmos.DrawWireSphere (bpAlongPath.position, 0.2f);
        Handles.PositionHandle(bpAlongPath.position, bpAlongPath.rotation);

        Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld(Vector3.right * 2), 0.2f);
        Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld(Vector3.right * -2), 0.2f);
    }

    private BezierPoint GetBezierPointAlongPath (float _t) {
        Vector3 p0 = GetPos (0);
        Vector3 p1 = GetPos (1);
        Vector3 p2 = GetPos (2);
        Vector3 p3 = GetPos (3);

        Vector3 a = Vector3.Lerp (p0, p1, _t);
        Vector3 b = Vector3.Lerp (p1, p2, _t);
        Vector3 c = Vector3.Lerp (p2, p3, _t);

        Vector3 d = Vector3.Lerp (a, b, _t);
        Vector3 e = Vector3.Lerp (b, c, _t);

        Vector3 f = Vector3.Lerp (d, e, _t);

        BezierPoint bp = new BezierPoint ();
        bp.position = f;
        bp.rotation = Quaternion.LookRotation(e-d);
        return bp;
    }

}