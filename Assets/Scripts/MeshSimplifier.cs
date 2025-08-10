using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class MeshSimplifier : MonoBehaviour
    {
        public float quality = 0.5f;

    void Start()
    {
        var originalMesh = GetComponent<MeshFilter>().sharedMesh;
        var simplifiedMesh = Simplify(originalMesh, quality);
        GetComponent<MeshFilter>().sharedMesh = simplifiedMesh;
    }

    Mesh Simplify(Mesh originalMesh, float quality)
    {
        Vector3[] vertices = originalMesh.vertices;
        int[] triangles = originalMesh.triangles;
        Vector3[] normals = originalMesh.normals;
        Vector2[] uvs = originalMesh.uv;

        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();

        Dictionary<int, int> oldToNewVertexMap = new Dictionary<int, int>();

        float threshold = (1.0f - quality) * 0.1f; // Adjust threshold based on quality

        for (int i = 0; i < vertices.Length; i++)
        {
            bool merged = false;

            for (int j = 0; j < newVertices.Count; j++)
            {
                if (Vector3.Distance(vertices[i], newVertices[j]) < threshold)
                {
                    oldToNewVertexMap[i] = j;
                    merged = true;
                    break;
                }
            }

            if (!merged)
            {
                oldToNewVertexMap[i] = newVertices.Count;
                newVertices.Add(vertices[i]);
                newNormals.Add(normals[i]);
                newUVs.Add(uvs[i]);
            }
        }

        for (int i = 0; i < triangles.Length; i++)
        {
            newTriangles.Add(oldToNewVertexMap[triangles[i]]);
        }

        Mesh simplifiedMesh = new Mesh();
        simplifiedMesh.vertices = newVertices.ToArray();
        simplifiedMesh.triangles = newTriangles.ToArray();
        simplifiedMesh.normals = newNormals.ToArray();
        simplifiedMesh.uv = newUVs.ToArray();

        simplifiedMesh.RecalculateBounds();
        simplifiedMesh.RecalculateNormals();

        return simplifiedMesh;
    }
    }
}
