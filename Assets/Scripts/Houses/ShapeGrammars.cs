using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class HouseTransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 offset;
    public int connectionPointIndex;
    public ConnectionPoints connectionPoints;
}

public class ShapeGrammars : MonoBehaviour
{
    [SerializeField] public string axiom;
    [SerializeField] public int recursion = 1;
    [SerializeField] public Vector3 WallSize;
    [SerializeField] public Vector3 FloorSize;

    [SerializeField] public List<char> letter;
    [SerializeField] public List<string> rule;

    Dictionary<char, string> rules;
    private string currentString = string.Empty;
    private Stack<HouseTransformInfo> transformStack;

    GenerateCube generateCube;

    // Start is called before the first frame update
    void Awake()
    {
        transformStack = new Stack<HouseTransformInfo>();

        if (letter.Count != rule.Count)
        {
            Debug.LogError("The 'letter' and 'rule' lists must have the same length.");
            return;
        }
        rules = letter.Zip(rule, (l, r) => new { Letter = l, Rule = r })
            .ToDictionary(item => item.Letter, item => item.Rule);
        generateCube = GetComponent<GenerateCube>();
        //spawntrees();
        Generate();
    }

    public void Generate()
    {
        currentString = axiom;
        GameObject tree = new GameObject("tree");
        tree.AddComponent<MeshRenderer>();
        tree.AddComponent<MeshFilter>();
        transform.position = Vector3.zero;
        transform.rotation = new Quaternion(0, 0, 0, 0);

        ConnectionPoints connectionPoints = new ConnectionPoints();
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.identity;
        Vector3 offset = Vector3.zero;
        int connectionPointIndex = 0;
        for (int i = 0; i < recursion; i++)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in currentString)
            {
                if (rules.ContainsKey(c))
                {
                    stringBuilder.Append(rules[c]);
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
            currentString = stringBuilder.ToString();
        }
        Debug.Log(currentString);

        // Create an array of CombineInstance with the appropriate size
        List<GameObject> oldGameObjects = new List<GameObject>();


        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    if(transformStack.Count > 0 && connectionPoints.points.Count > 0)
                    {
                        generateCube.GenerateFloor(connectionPoints.points[connectionPointIndex].point + offset, FloorSize, out connectionPoints);
                        Debug.Log("A");
                    }
                    else
                    {
                        generateCube.GenerateFloor(this.transform.position + offset, FloorSize, out connectionPoints);
                        Debug.Log("B");
                    }
                    break;
                case 'C':
                    if (transformStack.Count > 0 && connectionPoints.points.Count > 0)
                    {
                        generateCube.GenerateFloor(connectionPoints.points[connectionPointIndex].point + offset, FloorSize, out connectionPoints);
                        Debug.Log("A");
                    }
                    else
                    {
                        generateCube.GenerateFloor(new Vector3(0, 0, 0) + offset, FloorSize, out connectionPoints);
                        Debug.Log("B");
                    }
                    break;
                case 'W':
                    generateCube.GenerateWall(connectionPoints.points[connectionPointIndex].point + offset, connectionPoints.points[connectionPointIndex].rotation, WallSize);
                    break;
                case '[':
                    transformStack.Push(new HouseTransformInfo()
                    {
                        position = position,
                        rotation = rotation,
                        offset = offset,
                        connectionPointIndex = connectionPointIndex,
                        connectionPoints = connectionPoints
                    }) ;
                    Debug.Log("New TransformStack :" + transformStack.Peek().connectionPoints.points.Count);
                    break;
                case ']':
                    HouseTransformInfo hti = transformStack.Pop();
                    position = hti.position;
                    rotation = hti.rotation;
                    offset = hti.offset;
                    connectionPointIndex = hti.connectionPointIndex;
                    connectionPoints = hti.connectionPoints;
                    break;
                case '>':
                    if (connectionPoints.points.Count > connectionPointIndex)
                    {
                        connectionPointIndex ++;
                    }
                    else
                    {
                        if (!(connectionPoints.points.Count >= connectionPointIndex-1))
                        {
                            connectionPointIndex = 0;
                        }
                    }
                    Quaternion rotation2 = Quaternion.Euler(0,90,0);
                    rotation*= rotation2;
                    break;
                case '<':
                    if (connectionPoints.points.Count > connectionPointIndex)
                    {
                        connectionPointIndex++;
                    }
                    else
                    {
                        if (!(connectionPoints.points.Count >= connectionPointIndex - 1))
                        {
                            connectionPointIndex = 0;
                        }
                    }
                    Quaternion rotation3 = Quaternion.Euler(0, -90, 0);
                    rotation *= rotation3; 
                    break;
                case '^':
                    offset.y += WallSize.y;
                    break;
                case '+':
                    offset.x += FloorSize.x;
                    break;
                case '-':
                    //transform.Rotate(Vector3.forward * Random.Range(-angle, angle));
                    break;
                case ',':
                    break;
                case '.':
                    break;
            }

        }
        foreach (var oldGameObject in oldGameObjects)
        {
            Destroy(oldGameObject);
        }

        // Create separate meshes for branches and leaves


        // Assign the branch and leaf meshes to the tree's MeshFilter

        // Set the material for branches
        tree.GetComponent<MeshRenderer>().material.SetVector("_Vector2", new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)));
        float randomFloat = Random.Range(0f, 1f);
        float randomFloat2 = Random.Range(0f, 1f);

        tree.GetComponent<MeshRenderer>().material.SetFloat("_Float", randomFloat);
        //tree.GetComponent<MeshFilter>().sharedMesh = MeshSmoothener.SmoothMesh(tree.GetComponent<MeshFilter>().sharedMesh, 1, MeshSmoothener.Filter.Laplacian);
        // Optionally, you can create a separate GameObject for leaves if needed


        // Set the material for leaves
        }

    private void OnDrawGizmos()
    {
        
    }
}
