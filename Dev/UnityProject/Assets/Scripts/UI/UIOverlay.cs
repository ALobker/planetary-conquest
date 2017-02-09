using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour {
    public Transform campParent;

    private CreateCamps cc;
    private int selectedPlayers = 3, selectedSize = 0;

	// Use this for initialization
	void Start () {
        if (campParent == null)
            campParent = GameObject.Find("Camps").transform;
        cc = GetComponent<CreateCamps>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUI.skin.label.fontSize = 25;
        GUI.skin.button.fontSize = 25;
        GUI.skin.toggle.fontSize = 25;

        if (GameManager.gameState == GameManager.State.Playing)
            DrawIngameUI();
        else
            DrawCreationMenu();
    }

    private void DrawCreationMenu()
    {
        int buttonHeight = 35;
        int offsetX = Screen.width / 2 - 250;
        int playersY = Screen.height / 2 - buttonHeight - 10;
        int campsY = Screen.height / 2 + 10;

        GUI.Label(new Rect(offsetX, playersY, 100, buttonHeight), "Players");
        if (GUI.Toggle(new Rect(offsetX + 100, playersY, 70, buttonHeight), selectedPlayers == 2, "2"))
            selectedPlayers = 2;
        if (GUI.Toggle(new Rect(offsetX + 200, playersY, 70, buttonHeight), selectedPlayers == 3, "3"))
            selectedPlayers = 3;
        if (GUI.Toggle(new Rect(offsetX + 300, playersY, 70, buttonHeight), selectedPlayers == 4, "4"))
            selectedPlayers = 4;
        if (GUI.Toggle(new Rect(offsetX + 400, playersY, 70, buttonHeight), selectedPlayers == 5, "5"))
            selectedPlayers = 5;
        GameManager.numPlayers = selectedPlayers;

        GUI.Label(new Rect(offsetX, campsY, 100, buttonHeight), "Camps");
        if (GUI.Toggle(new Rect(offsetX + 100, campsY, 100, buttonHeight), selectedSize == 0, "Small"))
        {
            selectedSize = 0;
            cc.numCamps = 6;
        }
        if (GUI.Toggle(new Rect(offsetX + 200, campsY, 100, buttonHeight), selectedSize == 1, "Medium"))
        {
            selectedSize = 1;
            cc.numCamps = 12;
        }
        if (GUI.Toggle(new Rect(offsetX + 300, campsY, 100, buttonHeight), selectedSize == 2, "Large"))
        {
            selectedSize = 2;
            cc.numCamps = 25;
        }
        if (GUI.Toggle(new Rect(offsetX + 400, campsY, 100, buttonHeight), selectedSize == 3, "Huge"))
        {
            selectedSize = 3;
            cc.numCamps = 50;
        }

        if (GUI.Button(new Rect((Screen.width - 200) / 2, campsY + buttonHeight + 10, 200, buttonHeight), "Start"))
        {
            GameManager.gameState = GameManager.State.Playing;
            cc.CreateAllCamps();
        }
    }

    private void DrawIngameUI()
    {
        int[] factionNumbers = new int[GameManager.numPlayers+1];
        int numCamps = 0;
        foreach(Transform camp in campParent) {
            if(!camp.gameObject.activeSelf)
                continue;

            CampScript cs = camp.GetComponent<CampScript>();
            factionNumbers[cs.faction]++;
            numCamps++;
        }

        GUI.Label(new Rect(10, 10, 300, 40), "Empty\t" + factionNumbers[0]);
        GUIStyle defaultStyle = GUI.skin.label;
        for (int i = 1; i < factionNumbers.Length; i++)
        {
            defaultStyle.normal.textColor = GameManager.playerColors[i];
            GUI.Label(new Rect(10, 10 + i * 40, 300, 40), "Player " + i + "\t" + factionNumbers[i], defaultStyle);
            if (factionNumbers[i] == numCamps)
            {
                cc.RemoveAllCamps();
                GameManager.gameState = GameManager.State.Menu;
            }
        }
        defaultStyle.normal.textColor = Color.white;
    }
}
