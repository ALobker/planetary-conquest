using UnityEngine;
using System.Collections;

public class DraggingInteraction : MonoBehaviour {
    public Transform camps;

    private Vector3 dragStart = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1"))
        {
            //do raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude + 0.5f)) {
                //Transform objectHit = hit.transform;
                dragStart = hit.point;
                //find closest base
                Transform best = camps.GetChild(0);
                float bestDist = Vector3.Distance(dragStart, best.position);
                foreach(Transform camp in camps) {
                    float dist = Vector3.Distance(dragStart, camp.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        best = camp;
                    }
                }
                dragStart = best.position;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            //do raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude + 0.5f)) {
                Vector3 dragEnd = hit.point;
                Debug.DrawLine(dragStart, dragEnd);
                //find closest base
            }
            //find cardinal directions
            //set arrow direction
        }
        if (Input.GetButtonUp("Fire1"))
        {
            //set arrow
        }
	}
}
