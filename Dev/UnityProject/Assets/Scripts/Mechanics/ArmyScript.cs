using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyScript : MonoBehaviour
{
    public CampScript from, to;
    public float speed = 1;
    public int faction = 0;

    private float progress = 0;

    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = GameManager.colors[GameManager.playerColors[faction]];
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameManager.State.Playing)
            return;
        Vector3 far = (2 * to.transform.position) - from.transform.position;
        progress += Time.deltaTime * speed;
        if (progress < 1f)
        {
            transform.position = Vector3.Slerp(from.transform.position, to.transform.position, progress);
            transform.rotation = Quaternion.LookRotation(far - Vector3.Project(far, transform.position), transform.position);
        }
        else
        {
            to.addUnit(this);
            GameObject.Destroy(this.gameObject);
        }
    }
}
