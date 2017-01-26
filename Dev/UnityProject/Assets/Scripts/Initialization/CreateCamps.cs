using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreateCamps : MonoBehaviour
{
    public GameObject campPrefab;
    public Transform campParent;
    public GameObject armyPrefab;
    public Transform armyParent;
    public int numCamps = 10;

    private List<Vector3> drawFrom, drawTo;

    // Use this for initialization
    void Start()
    {
        List<Vector3> points = new List<Vector3>(numCamps);
        List<int[]> neighbours = new List<int[]>(numCamps);

        if (numCamps <= 6)
        {
            numCamps = 6;
            points.Add(new Vector3(1, 0, 0));
            points.Add(new Vector3(-1, 0, 0));
            points.Add(new Vector3(0, 1, 0));
            points.Add(new Vector3(0, -1, 0));
            points.Add(new Vector3(0, 0, 1));
            points.Add(new Vector3(0, 0, -1));
            neighbours.Add(new int[] { 2, 3, 4, 5 });
            neighbours.Add(new int[] { 2, 3, 4, 5 });
            neighbours.Add(new int[] { 0, 1, 4, 5 });
            neighbours.Add(new int[] { 0, 1, 4, 5 });
            neighbours.Add(new int[] { 0, 1, 2, 3 });
            neighbours.Add(new int[] { 0, 1, 2, 3 });
        }
        else if (numCamps <= 8)
        {
            numCamps = 8;
            points.Add(new Vector3(1, 1, 1));
            points.Add(new Vector3(1, 1, -1));
            points.Add(new Vector3(1, -1, 1));
            points.Add(new Vector3(1, -1, -1));
            points.Add(new Vector3(-1, 1, 1));
            points.Add(new Vector3(-1, 1, -1));
            points.Add(new Vector3(-1, -1, 1));
            points.Add(new Vector3(-1, -1, -1));
            neighbours.Add(new int[] { 1, 2, 4 });
            neighbours.Add(new int[] { 0, 3, 5 });
            neighbours.Add(new int[] { 0, 3, 6 });
            neighbours.Add(new int[] { 1, 2, 7 });
            neighbours.Add(new int[] { 0, 5, 6 });
            neighbours.Add(new int[] { 1, 4, 7 });
            neighbours.Add(new int[] { 2, 4, 7 });
            neighbours.Add(new int[] { 3, 5, 6 });
        }
        else if (numCamps <= 12)
        {
            numCamps = 12;
            points.Add(new Vector3(0, -1, -1));
            points.Add(new Vector3(0, -1, 1));
            points.Add(new Vector3(0, 1, -1));
            points.Add(new Vector3(0, 1, 1));
            points.Add(new Vector3(-1, 0, -1));
            points.Add(new Vector3(-1, 0, 1));
            points.Add(new Vector3(1, 0, -1));
            points.Add(new Vector3(1, 0, 1));
            points.Add(new Vector3(-1, -1, 0));
            points.Add(new Vector3(-1, 1, 0));
            points.Add(new Vector3(1, -1, 0));
            points.Add(new Vector3(1, 1, 0));
            neighbours.Add(new int[] { 4, 6, 8, 10 }); //0
            neighbours.Add(new int[] { 5, 7, 8, 10 });
            neighbours.Add(new int[] { 4, 6, 9, 11 }); //2
            neighbours.Add(new int[] { 5, 7, 9, 11 });
            neighbours.Add(new int[] { 0, 2, 8, 9 }); //4
            neighbours.Add(new int[] { 1, 3, 8, 9 });
            neighbours.Add(new int[] { 0, 2, 10, 11 }); //6
            neighbours.Add(new int[] { 1, 3, 10, 11 });
            neighbours.Add(new int[] { 0, 1, 4, 5 }); //8
            neighbours.Add(new int[] { 2, 3, 4, 5 });
            neighbours.Add(new int[] { 0, 1, 6, 7 }); //10
            neighbours.Add(new int[] { 2, 3, 6, 7 });
        }
        List<CampScript> camps = new List<CampScript>(numCamps);

        for (int i = 0; i < numCamps; i++)
        {
            //Vector3 p = Random.onUnitSphere * 5.5f;
            Vector3 one = Vector3.one + 0.001f * Vector3.up; //slightly offsetted vector one
            Vector3 p = points[i].normalized * 5.5f;
            GameObject newCamp = GameObject.Instantiate<GameObject>(campPrefab);
            newCamp.transform.parent = campParent;
            newCamp.transform.position = p;
            newCamp.transform.rotation = Quaternion.LookRotation(one - Vector3.Project(one, p), p);

            CampScript cs = newCamp.GetComponent<CampScript>();
            cs.armyParent = armyParent;
            cs.armyPrefab = armyPrefab;
            cs.faction = Random.Range(0, 4);
            camps.Add(cs);
        }

        //set neighbours
        for (int i = 0; i < numCamps; i++)
        {
            camps[i].neighbours = new CampScript[neighbours[i].Length];
            for (int j = 0; j < neighbours[i].Length; j++)
            {
                camps[i].neighbours[j] = camps[neighbours[i][j]];
            }
        }

        drawFrom = new List<Vector3>();
        drawTo = new List<Vector3>();
        drawBorders(camps);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < drawFrom.Count; i++)
        {
            Debug.DrawLine(drawFrom[i], drawTo[i], Color.white);
        }
    }

    private List<Vector3> generateSpherePoints(int numPoints)
    {
        // https://www.cmu.edu/biolphys/deserno/pdf/sphere_equi.pdf

        List<Vector3> points = new List<Vector3>(numPoints);
        for (int i = 0; i < numPoints; i++)
        {
            float z = Random.Range(-1f, 1f);
            float phi = Random.Range(0f, Mathf.PI);
            float x = Mathf.Sqrt(1 - z * z) * Mathf.Cos(phi);
            float y = Mathf.Sqrt(1 - z * z) * Mathf.Sin(phi);
        }

        //temp
        float theta = 0;

        //int ncount = 0;
        float a = (4f * Mathf.PI) / numPoints;
        float d = Mathf.Sqrt(a);
        float mv = Mathf.Round(Mathf.PI / d);
        float dv = Mathf.PI / mv;
        float dt = a / dv;

        for (int m = 0; m < mv; m++)
        {
            float v = Mathf.PI * (m + 0.5f) / mv;
            float mp = Mathf.Round(2 * Mathf.PI * Mathf.Sin(theta) / dt);
            for (int n = 0; n < mp; n++)
            {
                float phi = 2 * Mathf.PI * n / mp;
                points.Add(createPoint(theta, phi));
            }
        }
        return points;
    }

    private Vector3 createPoint(float theta, float phi)
    {
        return new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta));
    }

    private void drawBorders(List<CampScript> camps)
    {
        List<Vector3> segmentA = new List<Vector3>(), segmentB = new List<Vector3>(), segmentMid = new List<Vector3>();

        //get all line segments
        foreach (CampScript camp in camps)
        {
            foreach (CampScript neigh in camp.neighbours)
            {
                //drawLine(camp.transform.position * 0.92f, neigh.transform.position * 0.92f);
                Vector3 mid = (camp.transform.position + neigh.transform.position) * 0.92f / 2f;
                if (!segmentMid.Contains(mid))
                {
                    Vector3 offset = Vector3.Cross(mid, camp.transform.position - mid);
                    offset.Normalize();
                    Vector3 left = mid + offset;
                    Vector3 right = mid - offset;
                    drawLine(left, right);
                    segmentMid.Add(mid);
                    segmentA.Add(left);
                    segmentB.Add(right);
                }
            }
        }
        //find intersect points
        List<Vector3> intersections = new List<Vector3>();
        for (int i = 0; i < segmentA.Count; i++)
        {
            for (int j = i + 1; j < segmentA.Count; j++)
            {
                Vector3 intersect;//, intersect2;
                if (LineLineIntersection(out intersect, segmentA[i], segmentB[i], segmentA[j], segmentB[j]))
                {
                    //if (intersect.magnitude > 10f || intersect.magnitude < 3f)
                    if (intersect.magnitude < 3f)
                        continue;
                    intersect = intersect.normalized * 5.1f;
                    if (!intersections.Contains(intersect))
                    {
                        intersections.Add(intersect);
                    }
                }
                /*else if(ClosestPointsOnTwoLines(out intersect, out intersect2, segmentA[i], segmentB[i], segmentA[j], segmentB[j]))
                {
                    if (intersect.magnitude > 9f || intersect.magnitude < 3f)
                        continue;

                    if (Vector3.Distance(intersect, intersect2) < 1f)
                    {
                        Vector3 mid = (intersect + intersect2) / 2f;
                        if (!intersections.Contains(mid))
                        {
                            intersections.Add(mid);
                        }
                    }
                }*/
            }
        }

        float bestDist = 10f;
        //create lines
        for (int i = 0; i < intersections.Count - 1; i++)
        {
            drawLine(intersections[i] + Vector3.up, intersections[i]);
            drawLine(intersections[i] + Vector3.right, intersections[i]);
            drawLine(intersections[i] + Vector3.forward, intersections[i]);
            
            Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>();
            for (int j = i + 1; j < intersections.Count; j++)
            {
                distances.Add(intersections[j], Vector3.Distance(intersections[i], intersections[j]));
                Debug.Log(Vector3.Distance(intersections[i], intersections[j]));
            }
            List<KeyValuePair<Vector3, float>> best3 = distances.OrderBy(pair => pair.Value).Take(3).ToList();
            if(best3.Count > 0)
                bestDist = Mathf.Min(bestDist, best3[0].Value);
            foreach (KeyValuePair<Vector3, float> edge in best3)
            {
                if(edge.Value - bestDist < 0.001f)
                    drawLine(intersections[i], edge.Key);
            }
        }
    }

    private void drawLine(Vector3 p1, Vector3 p2)
    {
        float dist = Vector3.Distance(p1, p2);
        int numSegs = Mathf.CeilToInt(dist / 1f);
        for (int i = 0; i < numSegs; i++)
        {
            Vector3 a = Vector3.Slerp(p1, p2, (float)i / numSegs);
            Vector3 b = Vector3.Slerp(p1, p2, (i + 1f) / numSegs);
            drawFrom.Add(a);
            drawTo.Add(b);
            //Debug.DrawLine(a, b, Color.white);
        }
    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }


    //Two non-parallel lines which may or may not touch each other have a point on each line which are closest
    //to each other. This function finds those two points. If the lines are not parallel, the function 
    //outputs true, otherwise false.
    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {
            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }
        else
        {
            return false;
        }
    }
}
