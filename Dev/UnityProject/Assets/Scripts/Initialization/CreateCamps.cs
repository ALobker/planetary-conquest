using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCamps : MonoBehaviour {
    public GameObject campPrefab;
    public Transform campParent;
    public int numCamps = 10;

	// Use this for initialization
	void Start () {
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
        else if(numCamps <= 8)
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
            camps.Add(cs);
        }

        //set neighbours
        for(int i = 0; i < numCamps; i++)
        {
            camps[i].neighbours = new CampScript[neighbours[i].Length];
            for(int j = 0; j < neighbours[i].Length; j++) {
                camps[i].neighbours[j] = camps[neighbours[i][j]];
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
