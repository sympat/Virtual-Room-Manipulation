using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Start() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        // 2 -- 3
        // |    |
        // 0 -- 1
        foreach(var eachUV in meshFilter.mesh.uv)
            Debug.Log(eachUV);
        
        // float extents = 0.5f;
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-0.5f, -0.5f, 0.0f);
        vertices[1] = new Vector3(10f, -0.5f, 0.0f);
        vertices[2] = new Vector3(-0.5f, 10f, 0.0f);
        vertices[3] = new Vector3(10f, 10f, 0.0f);

        // Vector2[] uv = null;

        // Mesh mesh = new Mesh();
        // mesh.vertices = vertices;
        // mesh.uv = uv;

        meshFilter.mesh.vertices = vertices;
    }
}
