using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerateCube : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] int pointAmount;
    ConnectionPoints connectionPoints;
    // Start is called before the first frame update
    void Start()
    {
        connectionPoints = new ConnectionPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateFloor(Vector3 position, Vector3 scale, out ConnectionPoints connectionPoints)
    {
        connectionPoints = new ConnectionPoints();
        GameObject cube = new GameObject("Cube");
        cube.transform.position = position;
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cube.AddComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = material;

        Mesh cubeMesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[36];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(scale.x, 0, 0);
        vertices[2] = new Vector3(0, scale.y, 0);
        vertices[3] = new Vector3(scale.x, scale.y, 0);
        vertices[4] = new Vector3(0, scale.y, -scale.z);
        vertices[5] = new Vector3(0, 0, -scale.z);
        vertices[6] = new Vector3(scale.x, 0, -scale.z);
        vertices[7] = new Vector3(scale.x, scale.y, -scale.z);

        //face1
        triangles[2] = 1; triangles[1] = 0; triangles[0] = 2;
        triangles[5] = 2; triangles[4] = 3; triangles[3] = 1;

        //face2
        triangles[6] = 2; triangles[7] = 3; triangles[8] = 7;
        triangles[9] = 7; triangles[10] = 4; triangles[11] = 2;

        triangles[12] = 3; triangles[13] = 1; triangles[14] = 6;
        triangles[15] = 6; triangles[16] = 7; triangles[17] = 3;

        triangles[20] = 0; triangles[19] = 1; triangles[18] = 6;
        triangles[23] = 6; triangles[22] = 5; triangles[21] = 0;

        triangles[24] = 6; triangles[25] = 5; triangles[26] = 4;
        triangles[27] = 4; triangles[28] = 7; triangles[29] = 6;

        triangles[32] = 0; triangles[31] = 5; triangles[30] = 4;
        triangles[35] = 4; triangles[34] = 2; triangles[33] = 0;

        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        //for (int x = 0; x < pointAmount; x++)
        //{
        //    for (int y = 0; y < pointAmount; y++)
        //    {
        //        ConnectionPoint connectionPoint = new ConnectionPoint();
        //        connectionPoint.point = new Vector3(0 + (scale.x / (pointAmount - 1)) * x, scale.y, 0 + -(scale.z / (pointAmount - 1)) * y);
        //        connectionPoint.connectionType = ConnectionType.wall;
        //        connectionPoints.points.Add(connectionPoint);
        //    }
        //}

        ConnectionPoint cp1 = new ConnectionPoint();
        cp1.point = new Vector3(0, scale.y, 0 );
        cp1.rotation = Quaternion.Euler(0, 0, 0);
        cp1.connectionType = ConnectionType.wall;
        connectionPoints.points.Add(cp1);

        ConnectionPoint cp2 = new ConnectionPoint();
        cp2.point = new Vector3(scale.x, scale.y,0);
        cp2.rotation = Quaternion.Euler(0,90,0);
        cp2.connectionType = ConnectionType.wall;
        connectionPoints.points.Add(cp2);

        ConnectionPoint cp3 = new ConnectionPoint();
        cp3.point = new Vector3(0, scale.y,-scale.z +1);
        cp3.rotation = Quaternion.Euler(0, 0, 0);
        cp3.connectionType = ConnectionType.wall;
        connectionPoints.points.Add(cp3);

        ConnectionPoint cp4 = new ConnectionPoint();
        cp4.point = new Vector3(1, scale.y, 0);
        cp4.rotation = Quaternion.Euler(0, 90, 0);
        cp4.connectionType = ConnectionType.wall;
        connectionPoints.points.Add(cp4);

        this.connectionPoints = connectionPoints;
        meshFilter.mesh = cubeMesh;
    }


    public void GenerateWall(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject cube = new GameObject("Cube");
        cube.transform.rotation = rotation;
        cube.transform.position = position;
        MeshFilter meshFilter = cube.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cube.AddComponent<MeshRenderer>();

        meshRenderer.sharedMaterial = material;

        Mesh cubeMesh = new Mesh();

        Vector3[] vertices = new Vector3[8];
        int[] triangles = new int[36];

        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(scale.x, 0, 0);
        vertices[2] = new Vector3(0, scale.y, 0);
        vertices[3] = new Vector3(scale.x, scale.y, 0);
        vertices[4] = new Vector3(0, scale.y, -scale.z);
        vertices[5] = new Vector3(0, 0, -scale.z);
        vertices[6] = new Vector3(scale.x, 0, -scale.z);
        vertices[7] = new Vector3(scale.x, scale.y, -scale.z);

        //face1
        triangles[2] = 1; triangles[1] = 0; triangles[0] = 2;
        triangles[5] = 2; triangles[4] = 3; triangles[3] = 1;

        //face2
        triangles[6] = 2; triangles[7] = 3; triangles[8] = 7;
        triangles[9] = 7; triangles[10] = 4; triangles[11] = 2;

        triangles[12] = 3; triangles[13] = 1; triangles[14] = 6;
        triangles[15] = 6; triangles[16] = 7; triangles[17] = 3;

        triangles[20] = 0; triangles[19] = 1; triangles[18] = 6;
        triangles[23] = 6; triangles[22] = 5; triangles[21] = 0;

        triangles[24] = 6; triangles[25] = 5; triangles[26] = 4;
        triangles[27] = 4; triangles[28] = 7; triangles[29] = 6;

        triangles[32] = 0; triangles[31] = 5; triangles[30] = 4;
        triangles[35] = 4; triangles[34] = 2; triangles[33] = 0;

        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        for (int x = 0; x < pointAmount; x++)
        {
            for (int y = 0; y < pointAmount; y++)
            {
                ConnectionPoint connectionPoint = new ConnectionPoint();
                connectionPoint.point = new Vector3(0 + (scale.x / (pointAmount - 1)) * x, scale.y, 0 + -(scale.z / (pointAmount - 1)) * y);
                connectionPoint.connectionType = ConnectionType.wall;
                //connectionPoints.points.Add(connectionPoint);
            }
        }

        meshFilter.mesh = cubeMesh;
    }
    private void OnDrawGizmos()
    {
        if(connectionPoints != null)
        {
            if(connectionPoints.points != null)
            {
                if(connectionPoints.points.Count != 0)
                {
                    for(int i = 0; i < connectionPoints.points.Count; i++)
                    {
                        Gizmos.DrawSphere(connectionPoints.points[i].point, 0.2f);
                    }
                }
            }
        }

    }
}

public class ConnectionPoints
{
    public List<ConnectionPoint> points = new List<ConnectionPoint>();
}

public struct ConnectionPoint
{
    public Vector3 point;
    public Quaternion rotation;
    public ConnectionType connectionType;
}

public enum ConnectionType
{
    wall,
    ground,
    ceiling
}