using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct Tuple<T, U> : IEquatable<Tuple<T, U>>
{
    readonly T first;
    readonly U second;

    public Tuple(T first, U second)
    {
        this.first = first;
        this.second = second;
    }

    public T First { get { return first; } }
    public U Second { get { return second; } }

    public override int GetHashCode()
    {
        return first.GetHashCode() ^ second.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return Equals((Tuple<T, U>)obj);
    }

    public bool Equals(Tuple<T, U> other)
    {
        return other.first.Equals(first) && other.second.Equals(second);
    }
}

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
        //CreateAllCamps();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameManager.State.Playing)
            return;
        if (drawFrom == null)
            return;

        for (int i = 0; i < drawFrom.Count; i++)
        {
            Debug.DrawLine(drawFrom[i], drawTo[i], Color.white);
        }
    }

    public void CreateAllCamps()
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
        else
        {
            points = GenerateSpherePoints(numCamps);
            neighbours = FindNeighboursBetter(points);
            Debug.Log("Numpoints " + points.Count + ", neighs: " + neighbours.Count);
        }

        List<CampScript> camps = new List<CampScript>(points.Count);

        Vector3 one = Vector3.one + 0.001f * Vector3.up; //slightly offsetted vector one
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i].normalized * 5.5f;
            GameObject newCamp = GameObject.Instantiate<GameObject>(campPrefab);
            newCamp.transform.parent = campParent;
            newCamp.transform.position = p;
            newCamp.transform.rotation = Quaternion.LookRotation(one - Vector3.Project(one, p), p);

            CampScript cs = newCamp.GetComponent<CampScript>();
            cs.armyParent = armyParent;
            cs.armyPrefab = armyPrefab;
            cs.faction = 0; // UnityEngine.Random.Range(0, 4);

            HiveAI ai = newCamp.AddComponent<HiveAI>();
            ai.camp = cs;

            camps.Add(cs);
        }

        //set neighbours
        for (int i = 0; i < points.Count; i++)
        {
            camps[i].neighbours = new CampScript[neighbours[i].Length];
            for (int j = 0; j < neighbours[i].Length; j++)
            {
                camps[i].neighbours[j] = camps[neighbours[i][j]];
            }
        }

        //select a few bases as starting positions
        //create an array and shuffle it
        int[] indexes = Enumerable.Range(0, camps.Count).OrderBy(x => UnityEngine.Random.Range(0, 1f)).ToArray();
        for (int i = 0; i < GameManager.numPlayers; i++)
        {
            camps[indexes[i]].faction = i + 1;
        }

        drawFrom = new List<Vector3>();
        drawTo = new List<Vector3>();
        //DrawConnections(camps);
        //DrawBorders(camps);
        DrawBordersAlternative(camps);

        //set camera for player
        Vector3 cameraPos = camps[indexes[0]].transform.position.normalized * 20f;
        Camera.main.transform.position = cameraPos;
        Camera.main.transform.LookAt(Vector3.zero, Vector3.up);
        OrbitCamera orb = Camera.main.GetComponent<OrbitCamera>();
        orb.SetRotation(Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.x);
    }

    public void RemoveAllCamps()
    {
        DestroyAllChildren(campParent);
        DestroyAllChildren(armyParent);
    }

    private void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }

    private List<Vector3> GenerateSpherePoints(int approxNumPoints)
    {
        // https://www.cmu.edu/biolphys/deserno/pdf/sphere_equi.pdf
        List<Vector3> points = new List<Vector3>(approxNumPoints);

        float a = (4f * Mathf.PI) / approxNumPoints;
        float d = Mathf.Sqrt(a);
        float mv = Mathf.Round(Mathf.PI / d);
        float dv = Mathf.PI / mv;
        float dp = a / dv;

        for (int m = 0; m < mv; m++)
        {
            float theta = Mathf.PI * (m + 0.5f) / mv;
            float mp = Mathf.Round(2 * Mathf.PI * Mathf.Sin(theta) / dp);
            for (int n = 0; n < mp; n++)
            {
                float phi = 2 * Mathf.PI * n / mp;
                points.Add(CreatePoint(theta, phi));
            }
        }
        return points;
    }

    // theta 0-pi, phi 0-2pi
    private Vector3 CreatePoint(float theta, float phi)
    {
        return new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta));
    }

    //TODO: find neighbours using delaunay
    private List<int[]> FindNeighbours(List<Vector3> points)
    {
        List<int[]> neighbours = new List<int[]>(numCamps);

        for (int i = 0; i < points.Count; i++)
        {
            Dictionary<int, float> distances = new Dictionary<int, float>();
            for (int j = 0; j < points.Count; j++)
            {
                if (i == j)
                    continue;
                float dist = Vector3.Distance(points[i], points[j]);
                if (dist > 0.001f)
                    distances.Add(j, dist);
            }

            List<KeyValuePair<int, float>> best4 = distances.OrderBy(pair => pair.Value).Take(4).ToList();
            int[] neighs = new int[best4.Count];
            for (int k = 0; k < best4.Count; k++)
            {
                neighs[k] = best4[k].Key;
            }
            neighbours.Add(neighs);
        }
        return neighbours;
    }

    private List<int[]> FindNeighboursBetter(List<Vector3> points)
    {
        List<int[]> neighbours = new List<int[]>(numCamps);

        for (int i = 0; i < points.Count; i++)
        {
            Dictionary<int, float> distances = new Dictionary<int, float>();
            for (int j = 0; j < points.Count; j++)
            {
                if (i == j)
                    continue;
                float dist = Vector3.Distance(points[i], points[j]);
                if (dist > 0.001f)
                    distances.Add(j, dist);
            }

            List<KeyValuePair<int, float>> best6 = distances.OrderBy(pair => pair.Value).Take(5).ToList();
            /*
            if (best6.Count > 5 && best6[5].Value / best6[3].Value > best6[3].Value / best6[1].Value)
                best6.RemoveAt(5);
            */
            if (best6.Count > 4 && best6[4].Value / best6[2].Value > best6[2].Value / best6[1].Value)
                best6.RemoveAt(4);

            int[] neighs = new int[best6.Count];
            for (int k = 0; k < best6.Count; k++)
            {
                neighs[k] = best6[k].Key;
            }
            neighbours.Add(neighs);
        }

        // reciprocate neighbour
        for (int i = 0; i < points.Count; i++)
        {
            int[] neighs = neighbours[i];
            foreach(int n in neighs) {
                if (!neighbours[n].Contains(i))
                {
                    //create bigger array and add i as neighbour
                    int[] ints = new int[neighbours[n].Length + 1];
                    for (int j = 0; j < neighbours[n].Length; j++)
                        ints[j] = neighbours[n][j];
                    ints[neighbours[n].Length] = i;
                    neighbours[n] = ints;
                }
            }
        }

        return neighbours;
    }

    private void DrawBorders(List<CampScript> camps)
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
                    //drawLine(left, right);
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
                if (LineLineIntersectionPoints(out intersect, segmentA[i], segmentB[i], segmentA[j], segmentB[j]))
                {
                    if (intersect.magnitude > 7f || intersect.magnitude < 4f)
                        continue;
                    intersect = intersect.normalized * 5.1f;
                    if (!intersections.Contains(intersect))
                    {
                        intersections.Add(intersect);
                    }
                }
            }
        }

        //remove nearly identical points
        for (int i = 0; i < intersections.Count - 1; i++)
        {
            for (int j = i + 1; j < intersections.Count; j++)
            {
                float dist = Vector3.Distance(intersections[i], intersections[j]);
                if (dist < 0.001f)
                {
                    intersections.RemoveAt(j);
                    j--;
                }
            }
        }
        //Debug.Log("Remaining points: " + intersections.Count);

        float bestDist = 10f;
        //create lines
        for (int i = 0; i < intersections.Count - 1; i++)
        {
            //drawLine(intersections[i] + Vector3.up, intersections[i]);
            //drawLine(intersections[i] + Vector3.right, intersections[i]);
            //drawLine(intersections[i] + Vector3.forward, intersections[i]);

            Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>();
            for (int j = i + 1; j < intersections.Count; j++)
            {
                distances.Add(intersections[j], Vector3.Distance(intersections[i], intersections[j]));
                //Debug.Log(Vector3.Distance(intersections[i], intersections[j]));
            }
            List<KeyValuePair<Vector3, float>> best4 = distances.OrderBy(pair => pair.Value).Take(4).ToList();
            if (best4.Count > 0)
                bestDist = Mathf.Min(bestDist, best4[0].Value);
            foreach (KeyValuePair<Vector3, float> edge in best4)
            {
                if (edge.Value - bestDist < 0.01f)
                    DrawLine(intersections[i], edge.Key);
            }
        }
    }

    private void DrawBordersAlternative(List<CampScript> camps)
    {
        //find all intersections between points
        foreach (CampScript camp in camps)
        {
            List<Vector3> intersections = new List<Vector3>();
            foreach (CampScript neigh in camp.neighbours)
            {
                foreach (CampScript second in neigh.neighbours)
                {
                    if (camp == second)
                        continue;
                    if (camp.neighbours.Contains(second) || second.neighbours.Contains(camp))
                    {
                        //Debug.Log("intersect");
                        //tri-point found
                        Vector3 mid = GetCircleCenter(camp.transform.position, neigh.transform.position, second.transform.position);
                        mid = mid.normalized * 5.1f;
                        if (!intersections.Contains(mid))
                            intersections.Add(mid);
                    }
                    else
                    {
                        foreach (CampScript third in second.neighbours)
                        {
                            if (camp == third || neigh == third)
                                continue;
                            if (neigh.neighbours.Contains(third) || third.neighbours.Contains(neigh))
                                continue;
                            if (camp.neighbours.Contains(third) || third.neighbours.Contains(camp))
                            {
                                //quad-point found
                                Vector3 mid = GetCircleCenter(camp.transform.position, neigh.transform.position, second.transform.position, third.transform.position);
                                mid = mid.normalized * 5.1f;
                                if (!intersections.Contains(mid))
                                    intersections.Add(mid);
                            }
                            else
                            {
                                foreach (CampScript fourth in third.neighbours)
                                {
                                    if (camp == fourth || neigh == fourth || second == fourth)
                                        continue;
                                    if (neigh.neighbours.Contains(fourth) || fourth.neighbours.Contains(neigh) || second.neighbours.Contains(fourth) || fourth.neighbours.Contains(second))
                                        continue;
                                    if (camp.neighbours.Contains(fourth) || fourth.neighbours.Contains(camp))
                                    {
                                        //quin-point found
                                        Vector3 mid = GetCircleCenter(camp.transform.position, neigh.transform.position, second.transform.position, third.transform.position, fourth.transform.position);
                                        mid = mid.normalized * 5.1f;
                                        if (!intersections.Contains(mid))
                                        {
                                            intersections.Add(mid);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //remove nearly identical points
            for (int i = 0; i < intersections.Count - 1; i++)
            {
                //drawLine(intersections[i] + Vector3.up, intersections[i]);
                //drawLine(intersections[i] + Vector3.right, intersections[i]);
                //drawLine(intersections[i] + Vector3.forward, intersections[i]);

                for (int j = i + 1; j < intersections.Count; j++)
                {
                    float dist = Vector3.Distance(intersections[i], intersections[j]);
                    if (dist < 0.001f)
                    {
                        intersections.RemoveAt(j);
                        j--;
                    }
                }
            }

            //find the distances between all points
            Dictionary<Tuple<Vector3, Vector3>, float> distances = new Dictionary<Tuple<Vector3, Vector3>, float>();
            for (int i = 0; i < intersections.Count - 1; i++)
            {
                for (int j = i + 1; j < intersections.Count; j++)
                {
                    distances.Add(new Tuple<Vector3, Vector3>(intersections[i], intersections[j]), Vector3.Distance(intersections[i], intersections[j]));
                }
            }

            //draw line around camp
            List<KeyValuePair<Tuple<Vector3, Vector3>, float>> best5 = distances.OrderBy(pair => pair.Value).Take(5).ToList();
            if (best5.Count > 4 && best5[4].Value / best5[3].Value > best5[3].Value / best5[1].Value)
                best5.RemoveAt(4);

            //remove lines through bases
            for (int i = 0; i < best5.Count; i++)
            {
                Vector3 mid = (best5[i].Key.First + best5[i].Key.Second).normalized;
                if (Vector3.Distance(camp.transform.position.normalized, mid) < 0.1f)
                {
                    best5.RemoveAt(i);
                    i--;
                }
                else
                {
                    foreach (CampScript neigh in camp.neighbours)
                    {
                        if (Vector3.Distance(neigh.transform.position.normalized, mid) < 0.1f)
                        {
                            best5.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            }

            foreach (KeyValuePair<Tuple<Vector3, Vector3>, float> edge in best5)
            {
                DrawLine(edge.Key.First, edge.Key.Second);
            }
        }
    }

    private void DrawConnections(List<CampScript> camps)
    {
        foreach (CampScript camp in camps)
        {
            foreach (CampScript neigh in camp.neighbours)
            {
                DrawLine(camp.transform.position, neigh.transform.position);
            }
        }
    }

    private void DrawLine(Vector3 p1, Vector3 p2)
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

    //Overlay function because i want to use points instead of directions
    public static bool LineLineIntersectionPoints(out Vector3 intersection, Vector3 lineAPoint1, Vector3 lineAPoint2, Vector3 lineBPoint1, Vector3 lineBPoint2)
    {
        Vector3 intersect;
        bool found = LineLineIntersection(out intersect, lineAPoint1, lineAPoint2 - lineAPoint1, lineBPoint1, lineBPoint2 - lineBPoint1);
        intersection = intersect;
        return found;
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

    public static Vector3 GetCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        // triangle "edges"
        Vector3 t = p2 - p1;
        Vector3 u = p3 - p1;
        Vector3 v = p3 - p2;

        // triangle normal
        Vector3 w = Vector3.Cross(t, u);
        float wsl = w.sqrMagnitude;
        if (wsl < .000001f)
            return (p1 + p2 + p3) / 3f; // area of the triangle is too small (you may additionally check the points for colinearity if you are paranoid)

        // helpers
        float iwsl2 = 1.0f / (2.0f * wsl);
        float tt = Vector3.Dot(t, t);
        float uu = Vector3.Dot(u, u);

        // result circle
        Vector3 circCenter = p1 + (u * tt * (Vector3.Dot(u, v)) - t * uu * (Vector3.Dot(t, v))) * iwsl2;
        return circCenter;

        //return (p1 + p2 + p3) / 3f;
    }

    public static Vector3 GetCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        //return (p1 + p2 + p3 + p4) / 4f;
        Vector3 c1 = GetCircleCenter(p2, p3, p4);
        Vector3 c2 = GetCircleCenter(p1, p3, p4);
        Vector3 c3 = GetCircleCenter(p1, p2, p4);
        Vector3 c4 = GetCircleCenter(p1, p2, p3);

        return (c1 + c2 + c3 + c4) / 4f;
    }

    public static Vector3 GetCircleCenter(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5)
    {
        //return (p1 + p2 + p3 + p4 + p5) / 5f;
        //approx
        Vector3 c1 = GetCircleCenter(p1, p2, p3);
        Vector3 c2 = GetCircleCenter(p3, p4, p5);
        Vector3 c3 = GetCircleCenter(p1, p3, p5);
        Vector3 c4 = GetCircleCenter(p2, p4, p5);

        return (c1 + c2 + c3 + c4) / 4f;
    }
}
