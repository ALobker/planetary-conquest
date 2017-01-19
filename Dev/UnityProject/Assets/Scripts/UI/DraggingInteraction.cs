using UnityEngine;
using System.Collections;

public class DraggingInteraction : MonoBehaviour {
    public Transform camps;

    private Transform selectedArrow = null;
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
                CampScript campscr = best.GetComponent<CampScript>();
                selectedArrow = campscr.arrow.transform;
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
                if (selectedArrow != null)
                {
                    Vector3 Projection = Vector3.Project(dragEnd, dragStart);
                    Vector3 perpendicular = Vector3.forward;

                    Quaternion rotation = Quaternion.identity;
                    if (dragStart.y > 0.9f || dragStart.y < -0.9f)
                    {
                        perpendicular = Vector3.Cross(dragStart, Vector3.forward);
                        float sign = dragEnd.z < dragStart.z ? -1f : 1f;
                        float angle = sign * Vector3.Angle(dragEnd - Projection, perpendicular);
                        //Debug.Log(angle);

                        if(dragStart.y > 0)
                            rotation = Quaternion.Euler(0, 90 - angle, 0);
                        else
                            rotation = Quaternion.Euler(0, -90 + angle, 180);
                    }
                    else
                    {
                        perpendicular = Vector3.Cross(dragStart, Vector3.up);
                        float sign = dragEnd.y > dragStart.y ? -1f : 1f;
                        float angle = sign * Vector3.Angle(dragEnd - Projection, perpendicular);
                        //Debug.Log(angle);

                        // -180     0 -70 90 == 180 110 -90
                        // -90      -70 0 0 == -110 -180 -180
                        // 0        0 70 -90 == -180 -110 90
                        // 90       70 0 -180
                        // 180      0 -70 90 == 180 110 -90

                        //selectedArrow.Rotate(dragStart, angle);
                        //Debug.Log(campAngle);
                        //-90 * z
                        // 1  back   -90   z
                        // 2  front   90  -z
                        // 3  left  -180  -x
                        // 4  right    0   x
                        float campAngle = Vector3.Angle(dragStart, Vector3.right) * (dragStart.z > 0 ? -1 : 1);
                        rotation = Quaternion.Euler(angle, campAngle, -90);
                    }

                    Quaternion offset = Quaternion.Euler(20, 0, 0);
                    selectedArrow.transform.rotation = rotation * offset;
                }
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
