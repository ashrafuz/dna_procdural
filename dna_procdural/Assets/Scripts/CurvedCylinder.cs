using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CurvedCylinder : MonoBehaviour {
    [SerializeField] private Transform[] m_ControlPoints = new Transform[4];
    [SerializeField][Range (0, 1)] float m_SliderTest = 0;
    [SerializeField][Range (0, 1)] float m_MeshSliderTest = 0;
    [SerializeField][Range (0.1f, 5)] float m_Radius = 0.2f;

    [SerializeField][Range (4, 128)] int m_MeshDetailLevel = 4;
    [SerializeField][Range (4, 128)] float m_FaceDetailLevel = 4;
    [SerializeField] DataAsset m_Data;

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
        List<Vector3> normalList = new List<Vector3> ();
        List<int> triangleIndices = new List<int> ();

        Gizmos.color = Color.green;

        BezierPoint bp = Maths.GetBezierPointAlongPath (m_MeshSliderTest, m_ControlPoints);
        //Vector3[] worldVerts = m_Data.vertices.Select (v => bp.LocalToWorld (v.points)).ToArray ();

        for (int ring = 0; ring < m_MeshDetailLevel; ring++) {
            float t = ring / (m_MeshDetailLevel - 1f);
            BezierPoint bpPath = Maths.GetBezierPointAlongPath (t,
                m_ControlPoints[0].position, //start
                bp.position, //end
                4.5f // offset
            );

            for (int i = 0; i < m_Data.vertices.Length; i++) {
                vertexList.Add (bpPath.LocalToWorld (m_Data.vertices[i].points));
                Gizmos.DrawSphere (vertexList[vertexList.Count - 1], 0.1f);
                normalList.Add (bpPath.LocalToWorldNormal (m_Data.vertices[i].normals));
            }
        }

        //trianlges
        for (int ring = 0; ring < m_MeshDetailLevel - 1; ring++) {
            int rootIndex = ring * m_Data.vertices.Length;
            int rootIndexNext = (ring + 1) * m_Data.vertices.Length;

            for (int line = 0; line < m_Data.lineIndices.Length; line += 2) {
                int line0 = m_Data.lineIndices[line];
                int line1 = m_Data.lineIndices[line + 1];

                int currentA = rootIndex + line0;
                int currentB = rootIndex + line1;

                int nextA = rootIndexNext + line0;
                int nextB = rootIndexNext + line1;

                triangleIndices.Add (currentA);
                triangleIndices.Add (nextB);
                triangleIndices.Add (nextA);

                triangleIndices.Add (nextB);
                triangleIndices.Add (currentA);
                triangleIndices.Add (currentB);
            }
        }

        // for (int i = 0; i < m_Data.lineIndices.Length; i += 2) {
        //     Vector3 a = worldVerts[m_Data.lineIndices[i]];
        //     Vector3 b = worldVerts[m_Data.lineIndices[i + 1]];

        //     Gizmos.DrawLine (a, b);
        // }

        mMesh.SetVertices (vertexList);
        mMesh.SetTriangles (triangleIndices, 0);
        mMesh.SetNormals (normalList);

        //mMesh.RecalculateNormals ();
    }

    private void OnDrawGizmosSelected () {
        Init ();
        GenerateMesh ();
    }

    private void OnDrawGizmos () {
        // Handles.DrawBezier (
        //     GetPos (0),
        //     GetPos (3),
        //     GetPos (1),
        //     GetPos (2),
        //     Color.white, EditorGUIUtility.whiteTexture, 2);

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