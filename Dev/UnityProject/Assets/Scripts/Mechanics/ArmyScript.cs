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
        Color c = Color.white;
        switch (faction)
        {
            case 1: c = Color.cyan; break;
            case 2: c = Color.magenta; break;
            case 3: c = Color.yellow; break;
            case 4: c = Color.blue; break;
            case 5: c = Color.red; break;
        }
        gameObject.GetComponent<Renderer>().material.color = c;
    }

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime * speed;
        if (progress < 1f)
            transform.position = Vector3.Slerp(from.transform.position, to.transform.position, progress);
        else
        {
            to.addUnit(this);
            GameObject.Destroy(this.gameObject);
        }
    }
}
