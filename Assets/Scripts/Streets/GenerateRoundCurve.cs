using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class GenerateRoundCurve : MonoBehaviour
{
    public float radius = 50f;
    public int numPoints = 100;
    public float noiseScale = 5f;
    public float radiusDecrement;
    public int numRings = 2;
    [SerializeField] Material mat;
    Vector3[] points;
    [SerializeField] List<GameObject> ringsAndPerimeter;
    [SerializeField] bool Generate;
    public List<List<GameObject>> ringsAndRoads = new List<List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {
        ringsAndPerimeter = new List<GameObject>();
        GenerateCircularPerimeter();
        AddRings(numRings, radiusDecrement);
        GenerateStreets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Generate)
        {
            GenerateCircularPerimeter();
            AddRings(numRings, radiusDecrement);
            GenerateStreets();
            Generate = false;
        }
    }

    void GenerateStreets()
    {
        bool previuosTrue = false; // else roads too close to each other
                                   // Connect each point on the perimeter to the corresponding point on each ring
        ringsAndRoads.Add(new List<GameObject>());
        Debug.Log(ringsAndRoads.Count);
        for (int i = 0; i < numPoints; i++)
        {
            if (previuosTrue)
            {
                previuosTrue = false ; 
                continue ;
            }
            if (Random.Range(0f, 2f) < 0.75f && !previuosTrue)
            {
                previuosTrue = true;
                for (int ringIndex = 1; ringIndex <= numRings; ringIndex++)
                {
                    Vector3 perimeterPoint = GetRingPoint(i, ringIndex - 1);
                    Vector3 ringPoint = GetRingPoint(i, ringIndex);

                    // Create a street segment between the points
                    GameObject curveObject = new GameObject("StreetSegment" + i + "_Ring" + ringIndex);
                    curveObject.transform.parent = transform;
                    curveObject.AddComponent<MeshFilter>();
                    curveObject.AddComponent<MeshRenderer>();
                    curveObject.GetComponent<MeshRenderer>().material = mat;

                    BezierCurve curve = curveObject.AddComponent<BezierCurve>();
                    BezierPoint p0 = curve.AddPointAt(perimeterPoint);

                    BezierPoint p1 = curve.AddPointAt((perimeterPoint + 0.5f * (ringPoint - perimeterPoint)) +new Vector3(Random.Range(-10,10),0, Random.Range(-10, 10)) );
                    BezierPoint p2 = curve.AddPointAt(ringPoint);
                    p2.handle2 = (p2.position - p1.position).normalized * 2;
                    p0.handle2 = (p0.position - p1.position).normalized * 2;


                    p1.handleStyle = BezierPoint.HandleStyle.Broken;
                    p1.handle1 = (p1.position - p2.position).normalized*3;
                    p1.handle2 = (p1.position - p0.position).normalized*3;
                    GenerateRoad road = curveObject.AddComponent<GenerateRoad>();
                    road.width = 2;
                    road.res = 30;
                    road.straightThreshold = 0.98f;
                    road.close = false;
                    Debug.Log("ring index " + (ringIndex - 1) + " l am " + ringsAndRoads.Count);
                    if (ringsAndRoads.Count <= ringIndex - 1)
                    {
                        ringsAndRoads.Add(new List<GameObject>());
                        Debug.Log("Added new ringAndRoads list. Total count: " + ringsAndRoads.Count);
                    }
                    ringsAndRoads[ringIndex - 1].Add(curveObject);

                }
            }

        }
    }


    Vector3 GetRingPoint(int index, int ringIndex)
    {
        Vector3 middleOfPerimeter = GetMiddleOfPerimeter();

        float angle = Mathf.Lerp(0f, 2f * Mathf.PI, index / (float)numPoints);
        float x = middleOfPerimeter.x + radius * Mathf.Cos(angle);
        float z = middleOfPerimeter.z + radius * Mathf.Sin(angle);
        if(ringsAndPerimeter[ringIndex].GetComponent<BezierCurve>() != null)
        {
            //Debug.Log(index + "  " + ringIndex + "  " + ringsAndPerimeter.Count + "  " + ringsAndPerimeter[ringIndex].GetComponent<BezierCurve>()[index].position);
        }
        else
        {
            Debug.Log("WTF IS THS NOT ERE" + ringsAndPerimeter[ringIndex].name);
            for(int i = 0; i < ringsAndPerimeter.Count; i++)
            {
                Debug.Log(ringsAndPerimeter[i].name);
            }
        }
        //return new Vector3(x, 0, z);
        return ringsAndPerimeter[ringIndex].GetComponent<BezierCurve>()[index].position;
    }

    void GenerateCircularPerimeter()
    {
        GameObject curveObject = new GameObject("BezierCurve");
        BezierCurve curve = curveObject.AddComponent<BezierCurve>();
        BezierPoint previousPoint = null;

        for (int i = 0; i < numPoints; i++)
        {

            float angle = Mathf.Lerp(0f, 2f * Mathf.PI, i / (float)numPoints);
            float x = radius * Mathf.Cos(angle) + Random.Range(-noiseScale, noiseScale);
            float z = radius * Mathf.Sin(angle) + Random.Range(-noiseScale, noiseScale);

            BezierPoint p1 = curve.AddPointAt(new Vector3(x, 0f, z));
            p1.handleStyle = BezierPoint.HandleStyle.Broken;
            p1.handle1 = new Vector3(-0.28f, 0, 0);
            if (i != 0)
            {
                previousPoint.handle2 = (p1.position - previousPoint.position).normalized*2;
                p1.handle1 = (previousPoint.position - p1.position).normalized*2;
            }
            previousPoint = p1;
        }
        curveObject.AddComponent<MeshFilter>();
        curveObject.AddComponent<MeshRenderer>();
        curveObject.GetComponent<MeshRenderer>().material = mat;
        GenerateRoad road = curveObject.AddComponent<GenerateRoad>();
        GeneratePlot plot = curveObject.AddComponent<GeneratePlot>();
        road.width = 3;
        road.res = 30;
        road.straightThreshold = 0.999f;
        points = curve.GetPoints(30);
        plot.res = points.Length;
        ringsAndPerimeter.Add(curveObject);
        //plot.Generate(road.rightVertices);
    }
    public Vector3 GetMiddleOfPerimeter()
    {
        Vector3 middle = Vector3.zero;

        foreach (Vector3 point in points)
        {
            middle += point;
        }

        middle /= points.Length;
        return middle;
    }

    void AddRings(int numRings, float radiusDecrement)
    {
        int ringIndex = numRings;
        BezierPoint previousPoint = null;

        for (int i = 1; i <= numRings; i++)
        {
            float currentRadius;
            radiusDecrement = radiusDecrement + Random.Range(-2, 2);

                currentRadius = radius - i * radiusDecrement;
            

            Vector3 middleOfPerimeter = GetMiddleOfPerimeter();

            GameObject curveObject = new GameObject("BezierCurve");
            BezierCurve curve = curveObject.AddComponent<BezierCurve>();

            for (int j = 0; j < numPoints; j++)
            {
                float theta = Mathf.Lerp(0f, 2f * Mathf.PI, j / (float)numPoints);
                float x = middleOfPerimeter.x + currentRadius * Mathf.Cos(theta) + Random.Range(-noiseScale, noiseScale);
                float z = middleOfPerimeter.z + currentRadius * Mathf.Sin(theta) + Random.Range(-noiseScale, noiseScale);

                BezierPoint p1 = curve.AddPointAt(new Vector3(x, 0f, z));
                p1.handleStyle = BezierPoint.HandleStyle.Broken;
                p1.handle1 = new Vector3(-0.28f, 0, 0);
                if(j != 0)
                {
                    previousPoint.handle2 = (p1.position - previousPoint.position).normalized;
                    p1.handle1 = (previousPoint.position - p1.position).normalized;
                }
                previousPoint = p1;
            }
            curveObject.AddComponent<MeshFilter>();
            curveObject.AddComponent<MeshRenderer>();
            curveObject.GetComponent<MeshRenderer>().material = mat;
            GenerateRoad road = curveObject.AddComponent<GenerateRoad>();
            road.width = 4;
            road.res = 30;
            road.straightThreshold = 0.98f;
            ringsAndPerimeter.Add(curveObject);
        }
    }
}