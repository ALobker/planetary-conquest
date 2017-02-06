using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour {
    public Transform campParent;

	// Use this for initialization
	void Start () {
        if (campParent == null)
            campParent = GameObject.Find("Camps").transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        int[] factionNumbers = new int[4];
        foreach(Transform camp in campParent) {
            if(!camp.gameObject.activeSelf)
                continue;

            CampScript cs = camp.GetComponent<CampScript>();
            factionNumbers[cs.faction]++;
        }
        GUI.Label(new Rect(10, 10, 200, 100), "Empty\t" + factionNumbers[0] + "\nPlayer 1 (cyan)\t" + factionNumbers[1] + "\nPlayer 2 (magenta)\t" + factionNumbers[2] + "\nPlayer 3 (yellow)\t" + factionNumbers[3]);
    }
}
