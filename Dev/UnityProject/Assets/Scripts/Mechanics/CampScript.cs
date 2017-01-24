using UnityEngine;
using System.Collections;

public class CampScript : MonoBehaviour {
    public Transform arrow;
    public CampScript[] neighbours;

	// Use this for initialization
	void Start () {
        if (arrow == null && transform.childCount > 0)
        {
            arrow = transform.GetChild(0);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
