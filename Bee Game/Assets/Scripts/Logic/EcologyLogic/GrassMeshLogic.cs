using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrassMeshLogic : MonoBehaviour
{
    [SerializeField]
    private Mesh groundMesh;

    [SerializeField]
    private Material GrassMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //meshFilter = this.GetComponent<MeshFilter>();
        //meshRenderer = this.GetComponent<MeshRenderer>();

        // Create Vector2 vertices
        //Vector2[] vertices2D = new Vector2[] {groundMesh.vertices.GetEnumerator(vert => new Vector2(vert.x, vert.z))};
        List<Vector2> vert2D = new List<Vector2>(groundMesh.vertices.Select(x => new Vector2(x.x, x.z)));
        Vector2[] vertices2D = vert2D.ToArray();
        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, -10+ groundMesh.vertices[i].y, vertices2D[i].y);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        MeshRenderer renderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        transform.eulerAngles = new Vector3(180, 0, 0);
        //renderer.material = GrassMaterial;
    }
}
