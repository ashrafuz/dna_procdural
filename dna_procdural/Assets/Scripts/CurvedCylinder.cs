using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class CurvedCylinder : MonoBehaviour {
    [SerializeField] private Transform[] m_ControlPoints = new Transform[4];
    [SerializeField][Range (0, 1)] float m_SliderTest = 0;
    [SerializeField][Range (0, 1)] float m_MeshSliderTest = 0;
    [SerializeField][Range (0.1f, 5)] float m_Radius = 0.2f;

    [SerializeField][Range (4, 128)] int m_MeshDetailLevel = 4;
    [SerializeField][Range (4, 128)] float m_FaceDetailLevel = 4;

    Vector3 GetPos (int i) => m_ControlPoints[i].position;
    private Mesh mMesh;

    private void Awake () {
        Init ();
        GenerateMesh ();
    }

    private void Init () {
        mMesh = new Mesh ();
        mMesh.name = "Cylinder Quad";
        GetComponent<MeshFilter> ().sharedMesh = mMesh;
    }

    private void GenerateMesh () {
        mMesh.Clear ();
        List<Vector3> vertexList = new List<Vector3> ();
        List<int> triangleIndices = new List<int> ();

        for (int ring = 0; ring < m_MeshDetailLevel + 1; ring++) {
            float pathPercent = ring / (float) m_MeshDetailLevel;
            BezierPoint rootPoint = Maths.GetBezierPointAlongPath (pathPercent, m_ControlPoints);

            for (int i = 0; i < m_FaceDetailLevel + 1; i++) {
                float t = i / (float) m_FaceDetailLevel;
                float angleInRad = Maths.TAU * t;

                Vector3 dir = Vector3.one;
                dir.x = Mathf.Cos (angleInRad);
                dir.y = Mathf.Sin (angleInRad);

                Vector3 pos = rootPoint.LocalToWorld (m_Radius * dir);
                Gizmos.DrawWireSphere (pos, 0.1f);

                vertexList.Add (pos);
                vertexList.Add (rootPoint.position);
            }
        }

        // for (int ring = 0; ring < m_MeshDetailLevel; ring++) {
        //     int rootIndexCurr = ring * m_MeshDetailLevel;

        // }

        for (int i = 0; i < 1; i++) {
            int rATopRight = (i * 2);
            int rABotRight = rATopRight + 1;
            int rABotLeft = rATopRight + 2;
            int rATopLeft = rATopRight + 3;

            int rBTopRight = rATopRight + m_MeshDetailLevel;
            int rBBotRight = rBTopRight + 1;
            int rBBotLeft = rBTopRight + 2;
            int rBTopLeft = rBTopRight + 3;

            // //face a
            // triangleIndices.Add (rootIndexA);
            // triangleIndices.Add (rootBotIndexA);
            // triangleIndices.Add (rootBotNextIndexA);

            // //face b
            // triangleIndices.Add (rootBotNextIndexA);
            // triangleIndices.Add (rootTopNextIndexA);
            // triangleIndices.Add (rootIndexA);

            //TOP
            triangleIndices.Add (rBTopLeft);
            triangleIndices.Add (rATopLeft);
            triangleIndices.Add (rATopRight);

            Debug.Log (rBTopLeft + ", " + rATopLeft + ", " + rATopRight);

            triangleIndices.Add (rATopRight);
            triangleIndices.Add (rBTopRight);
            triangleIndices.Add (rBTopLeft);

            Debug.Log (rATopRight + ", " + rBTopLeft + ", " + rBTopRight);
        }

        mMesh.SetVertices (vertexList);
        mMesh.SetTriangles (triangleIndices, 0);

        mMesh.RecalculateNormals ();
    }

    private void OnDrawGizmosSelected () {
        Init ();
        GenerateMesh ();
    }

    private void OnDrawGizmos () {
        Handles.DrawBezier (
            GetPos (0),
            GetPos (3),
            GetPos (1),
            GetPos (2),
            Color.white, EditorGUIUtility.whiteTexture, 2);

        BezierPoint bpAlongPath = Maths.GetBezierPointAlongPath (m_SliderTest, m_ControlPoints);

        Gizmos.DrawWireSphere (bpAlongPath.position, 0.2f);
        //Handles.PositionHandle (bpAlongPath.position, bpAlongPath.rotation);

        //Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld (Vector3.right * 2), 0.2f);
        //Gizmos.DrawWireSphere (bpAlongPath.LocalToWorld (Vector3.right * -2), 0.2f);
    }

    private void Update () {
        GenerateMesh ();
    }

}