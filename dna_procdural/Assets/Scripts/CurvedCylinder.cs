using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CurvedCylinder : MonoBehaviour {
    [SerializeField] private Transform[] m_ControlPoints = new Transform[4];
    [SerializeField][Range (0, 1)] float m_SliderTest = 0;
    [SerializeField][Range (0, 1)] float m_MeshSliderTest = 0;
    [SerializeField][Range (0.1f, 5)] float m_Radius = 0.2f;

    [SerializeField][Range (4, 128)] int m_MeshDetailLevel = 4;
    [SerializeField][Range (4, 128)] int m_FaceDetailLevel = 4;
    [SerializeField] DataAsset m_Data;

    DataAsset m_CircleData;

    Vector3 GetPos (int i) => m_ControlPoints[i].position;
    private Mesh mMesh;

    private void Init () {
        mMesh = new Mesh ();
        mMesh.name = "Cylinder Quad";
        GetComponent<MeshFilter> ().sharedMesh = mMesh;
        
        if (!File.Exists ("Assets/CircularData.asset")) {
            m_CircleData = ScriptableObject.CreateInstance<DataAsset> ();

            List<Vertex> vertices = new List<Vertex> ();
            for (int i = 0; i < m_FaceDetailLevel; i++) {
                float t = i / (float) m_FaceDetailLevel;
                float angleInRad = t * Maths.TAU;

                Vertex currentPoint = new Vertex ();
                currentPoint.points.x = Mathf.Cos (angleInRad) * m_Radius;
                currentPoint.points.y = Mathf.Sin (angleInRad) * m_Radius;

                currentPoint.normals = currentPoint.points.normalized;
                //2 because of hard edge
                vertices.Add (currentPoint);

                Vertex nextPoint = new Vertex ();
                nextPoint.points = currentPoint.points;
                nextPoint.normals = new Vector2 (Mathf.Cos (angleInRad + (Maths.TAU / 4)), Mathf.Sin (angleInRad + (Maths.TAU / 4)));

                vertices.Add (currentPoint);
            }

            List<int> lineIndices = new List<int> ();
            for (int i = 0; i < m_FaceDetailLevel * 2; i++) {
                lineIndices.Add ((i + 1) % (m_FaceDetailLevel * 2));
            }

            m_CircleData.vertices = vertices.ToArray ();
            m_CircleData.lineIndices = lineIndices.ToArray ();

            AssetDatabase.CreateAsset (m_CircleData, "Assets/CircularData.asset");
        } else {
            m_CircleData = AssetDatabase.LoadAssetAtPath<DataAsset>("Assets/CircularData.asset");
        }
    }

    private void GenerateMesh (DataAsset _useData) {
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
                bp.position,
                Vector3.Lerp (m_ControlPoints[0].position, m_ControlPoints[1].position, t),
                Vector3.Lerp (bp.position, m_ControlPoints[2].position, t)
            );

            for (int i = 0; i < _useData.vertices.Length; i++) {
                vertexList.Add (bpPath.LocalToWorld (_useData.vertices[i].points));
                Gizmos.DrawSphere (vertexList[vertexList.Count - 1], 0.1f);
                normalList.Add (bpPath.LocalToWorldNormal (_useData.vertices[i].normals));
            }
        }

        //trianlges
        for (int ring = 0; ring < m_MeshDetailLevel - 1; ring++) {
            int rootIndex = ring * _useData.vertices.Length;
            int rootIndexNext = (ring + 1) * _useData.vertices.Length;

            for (int line = 0; line < _useData.lineIndices.Length; line += 2) {
                int line0 = _useData.lineIndices[line];
                int line1 = _useData.lineIndices[line + 1];

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

        mMesh.RecalculateNormals ();
    }

    private void OnDrawGizmosSelected () {
        Init ();
        GenerateMesh (m_CircleData);
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

}