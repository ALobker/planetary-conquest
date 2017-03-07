using UnityEngine;
using System.Collections;

public class DraggingInteraction : MonoBehaviour
{
    public Transform camps;

    private CampScript selectedCamp = null;
    private Transform selectedArrow = null;
    private Vector3 dragStart = Vector3.zero;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameManager.State.Playing)
            return;
        if (camps.childCount == 0)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            //do raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude + 0.5f))
            {
                //Transform objectHit = hit.transform;
                dragStart = hit.point;
                //find closest base
                Transform best = camps.GetChild(0);
                float bestDist = Vector3.Distance(dragStart, best.position);
                foreach (Transform camp in camps)
                {
                    if (!camp.gameObject.activeInHierarchy)
                        continue;

                    float dist = Vector3.Distance(dragStart, camp.position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        best = camp;
                    }
                }
                dragStart = best.position;
                selectedCamp = best.GetComponent<CampScript>();

                //do not allow dragging if not player faction
                if (selectedCamp.faction != 1)
                {
                    dragStart = Vector3.zero;
                    selectedCamp = null;
                    return;
                }

                selectedArrow = selectedCamp.arrow;
                selectedArrow.gameObject.SetActive(true);
                selectedArrow.localPosition = Vector3.zero;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            //no dragging if no camp selected
            if (dragStart == Vector3.zero)
                return;

            //do raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude + 0.5f))
            {
                Vector3 dragEnd = hit.point;
                Debug.DrawLine(dragStart, dragEnd);

                //draw arrow in the desired angle
                if (selectedArrow != null)
                {
                    Vector3 Projection = Vector3.Project(dragEnd, dragStart);
                    float angle = -AngleSigned(dragEnd - Projection, selectedCamp.transform.right, selectedCamp.transform.up);
                    //Debug.Log(angle);

                    selectedArrow.localPosition = Vector3.zero;
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, angle, 0));
                    //Quaternion offset = Quaternion.Euler(20, 0, 0);
                    //selectedArrow.transform.rotation = rotation * offset;
                    selectedArrow.transform.localRotation = rotation;
                }
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            //no dragging if no camp selected
            if (dragStart == Vector3.zero)
                return;

            //do raycast
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Camera.main.transform.position.magnitude + 0.5f))
            {
                Vector3 dragEnd = hit.point;

                if (Vector3.Distance(dragStart, dragEnd) < 1f || selectedCamp.neighbours.Length == 0)
                {
                    //do not select a camp
                    selectedCamp.selectedNeighbour = null;
                    selectedArrow.gameObject.SetActive(false);
                }
                else
                {
                    Vector3 Projection = Vector3.Project(dragEnd, dragStart);
                    float angle = -AngleSigned(dragEnd - Projection, selectedCamp.transform.right, selectedCamp.transform.up);

                    CampScript[] neighs = selectedCamp.neighbours;
                    CampScript bestNeigh = neighs[0];
                    float campAngle = -AngleSigned(bestNeigh.transform.position - Vector3.Project(bestNeigh.transform.position, dragStart), selectedCamp.transform.right, selectedCamp.transform.up);
                    float bestAngle = Mathf.Abs(campAngle - angle);
                    //select intended neighbour
                    foreach (CampScript neigh in neighs)
                    {
                        campAngle = -AngleSigned(neigh.transform.position - Vector3.Project(neigh.transform.position, dragStart), selectedCamp.transform.right, selectedCamp.transform.up);
                        float diff = Mathf.Abs(campAngle - angle);
                        diff = Mathf.Min(diff, 360 - diff);
                        if (diff < bestAngle)
                        {
                            bestNeigh = neigh;
                            bestAngle = diff;
                        }
                    }

                    selectedArrow.position = Vector3.Slerp(selectedCamp.transform.position, bestNeigh.transform.position, .25f);
                    campAngle = -AngleSigned(bestNeigh.transform.position - Vector3.Project(bestNeigh.transform.position, dragStart), selectedCamp.transform.right, selectedCamp.transform.up);
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, campAngle, 0));
                    selectedArrow.transform.localRotation = rotation;
                    selectedCamp.selectedNeighbour = bestNeigh;
                }
            }
            else
            {
                selectedCamp.selectedNeighbour = null;
                selectedArrow.gameObject.SetActive(false);
            }
        }
    }

    public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
            Vector3.Dot(n, Vector3.Cross(v1, v2)),
            Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }
}
