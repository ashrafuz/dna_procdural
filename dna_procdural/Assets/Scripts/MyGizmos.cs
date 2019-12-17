using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPoint {
    public Vector3 position;
    public Quaternion rotation;

    public Vector3 LocalToWorld( Vector3 localSpace){
        return this.position + rotation * localSpace;
    }

    public Vector3 LocalToWorldNormal( Vector3 localSpace){
        return  rotation * localSpace;
    }
}

public static class Maths {
    public const float TAU = 6.283185f;

    public static Vector2 GetUnitVectorByAngle (float _angRad) {
        return new Vector2 (Mathf.Cos (_angRad), Mathf.Sin (_angRad));
    }

    public static BezierPoint GetBezierPointAlongPath (float _t, Transform[] _points) {
        return Maths.GetBezierPointAlongPath(_t, new Vector3[]{
            _points[0].position,
            _points[1].position,
            _points[2].position,
            _points[3].position });
    }

    public static BezierPoint GetBezierPointAlongPath (float _t, Vector3[] _points) {
        Vector3 p0 = _points[0];
        Vector3 p1 = _points[1];
        Vector3 p2 = _points[2];
        Vector3 p3 = _points[3];

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

    public static BezierPoint GetBezierPointAlongPath (float _t, Vector3 _start, Vector3 _end, Vector3 _tanA, Vector3 _tanB) {
        return Maths.GetBezierPointAlongPath(_t, new Vector3[] {_start, _tanA, _tanB,_end});
    }

    public static BezierPoint GetBezierPointAlongPath (float _t, Vector3 _start, Vector3 _end, float _offset) {
        Vector3 _tanA = Vector3.Lerp(_start, _end, 0.3f);
        Vector3 _tanB = Vector3.Lerp(_start, _end, 0.6f);

        _tanA = _tanA + ( Vector3.right * _offset); // offset along x
        _tanB = _tanB + ( Vector3.right * _offset);// offset along x

        return Maths.GetBezierPointAlongPath(_t, new Vector3[] {_start, _tanA, _tanB,_end});
    }

}

public static class MyGizmos {
    public static void DrawWireCirlce (Vector3 _pos, Quaternion _rot, float _radius, int _detail = 32) {
        Vector3[] points = new Vector3[_detail];

        for (int i = 0; i < _detail; i++) {
            float t = i / (float) _detail;
            float angleInRad = t * Maths.TAU;

            Vector2 point2d = Maths.GetUnitVectorByAngle (angleInRad) * _radius;
            points[i] = _pos + _rot * point2d; //vector transformation
        }

        for (int i = 0; i < _detail - 1; i++) {
            Gizmos.DrawLine (points[i], points[i + 1]);
            Gizmos.DrawWireSphere(points[i], 0.2f);
        }
        Gizmos.DrawLine (points[_detail - 1], points[0]);

        //Vector3[] ponts3d = new Vector3[_detail];
    }
}