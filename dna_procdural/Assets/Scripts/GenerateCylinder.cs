using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCylinder : MonoBehaviour {
    private void OnDrawGizmosSelected () {
        //MyGizmos.DrawWireCirlce (transform.position, Quaternion.identity, 10);
        Init();
        GenerateMesh();
    }

    private Mesh mMesh;
    [SerializeField][Range (1, 100)]  private float mRadius = 2;

    [SerializeField][Range (8, 128)] private int m_Detail = 8;

    private void Awake () {
        Init();
    }

    private void Init(){
        mMesh = new Mesh ();
        mMesh.name = "Cylinder Quad";
        GetComponent<MeshFilter> ().sharedMesh = mMesh;
    }

    private void GenerateMesh () {
        mMesh.Clear ();

        List<Vector3> vertexList = new List<Vector3> ();
        List<Vector3> normalList = new List<Vector3> ();
        List<Vector2> uvs = new List<Vector2> ();

        for (int i = 0; i < m_Detail + 1; i++) {
            float t = i / (float) m_Detail;
            float angleInRad = Maths.TAU * t;

            Vector2 dir = new Vector2();
            dir.x = Mathf.Cos(angleInRad);
            dir.y = Mathf.Sin(angleInRad);

            Vector2 pos = mRadius * dir;
            Gizmos.DrawWireSphere(pos, 0.1f);

            vertexList.Add(pos);

            vertexList.Add(transform.position);
        }

        List<int> triangleIndices = new List<int> ();

        for (int i = 0; i < m_Detail; i++)
        {
            int rootIndex = i * 2;
            int rootBotIndex = rootIndex+1;
            int rootBotNextIndex = rootIndex + 3;
            int rootTopNextIndex =rootIndex + 2;

            triangleIndices.Add(rootIndex);
            triangleIndices.Add(rootBotIndex);
            triangleIndices.Add(rootBotNextIndex);

            triangleIndices.Add(rootBotNextIndex);
            triangleIndices.Add(rootTopNextIndex);
            triangleIndices.Add(rootIndex);
        }

        mMesh.SetVertices(vertexList);
        mMesh.SetTriangles(triangleIndices,0);

        mMesh.RecalculateNormals();
    }

}