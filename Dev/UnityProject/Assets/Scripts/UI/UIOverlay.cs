using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour {
    public Transform campParent;

    private CreateCamps cc;
    private int selectedPlayers = 3, selectedSize = 0;
    private int winningPlayer = 0;

    /************
     * Menu
     * - Tutorial
     * - Single player
     *   - Game setup
     * - Multiplayer
     *   - Host
     *   - Join lan
     *   - Join online
     * - Options
     *   - Sound effects/music volume
     *   - UI size
     *   - Colorblind mode
     * - Credits
     ************/

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
        else if (GameManager.gameState == GameManager.State.End)
            DrawEndgameUI();
        else
            DrawCreationMenu();
    }

    private void DrawMainMenu()
    {
        int yOffset = (Screen.height - (5 * 50)) / 2;
        if (GUI.Button(new Rect((Screen.width - 200) / 2, yOffset, 200, 50), "Tutorial"))
            GameManager.gameState = GameManager.State.SinglePlayerMenu;
        else if (GUI.Button(new Rect((Screen.width - 200) / 2, yOffset + 50, 200, 50), "Single Player"))
            GameManager.gameState = GameManager.State.SinglePlayerMenu;
        else if (GUI.Button(new Rect((Screen.width - 200) / 2, yOffset + 100, 200, 50), "Multiplayer"))
            GameManager.gameState = GameManager.State.MultiPlayerMenu;
        else if (GUI.Button(new Rect((Screen.width - 200) / 2, yOffset + 150, 200, 50), "Options"))
            GameManager.gameState = GameManager.State.OptionsMenu;
        else if (GUI.Button(new Rect((Screen.width - 200) / 2, yOffset + 200, 200, 50), "Credits"))
            GameManager.gameState = GameManager.State.CreditsMenu;
    }

    private void DrawCreationMenu()
    {
        int buttonHeight = 35;
        int offsetX = (Screen.width-600) / 2;
        int playersY = Screen.height / 2 - buttonHeight - 10;
        int campsY = Screen.height / 2 + 10;

        GUI.Label(new Rect(offsetX, playersY, 100, buttonHeight), "Players");
        for (int i = 2; i <= 10; i++)
        {
            if (GUI.Toggle(new Rect(offsetX + 50*i, playersY, 50, buttonHeight), selectedPlayers == i, ""+i))
                selectedPlayers = i;
        }
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
        if (GUI.Toggle(new Rect(offsetX + 500, campsY, 100, buttonHeight), selectedSize == 4, "Giant"))
        {
            selectedSize = 4;
            cc.numCamps = 80;
        }

        if (GUI.Button(new Rect((Screen.width - 200) / 2, campsY + buttonHeight + 10, 200, buttonHeight), "Start"))
        {
            winningPlayer = 0;
            GameManager.gameState = GameManager.State.Playing;
            cc.CreateAllCamps();
        }
    }

    private void DrawOptionsMenu()
    {
        //sound effect volume

        //music volume

        //ui size

        //colorblind mode
        //"Off"
        //"Deuteranopia/Protanopia"
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

        int aliveFactions = 0;
        GUI.Label(new Rect(10, 40, 300, 40), "Empty\t" + factionNumbers[0]);
        GUIStyle defaultStyle = GUI.skin.label;
        for (int i = 1; i < factionNumbers.Length; i++)
        {
            defaultStyle.normal.textColor = GameManager.playerColors[i];
            GUI.Label(new Rect(10, 40 + i * 40, 300, 40), "Player " + i + "\t" + factionNumbers[i], defaultStyle);
            if (factionNumbers[i] > 0)
            {
                aliveFactions++;
                winningPlayer = i;
            }
        }
        defaultStyle.normal.textColor = Color.white;
        if (aliveFactions <= 1)
        {
            cc.RemoveAllCamps();
            GameManager.gameState = GameManager.State.End;
        }

        float runningOffset = 0;
        for (int i = 0; i < factionNumbers.Length; i++) {
            Texture2D tex = new Texture2D(1,1, TextureFormat.ARGB32, false);
            tex.SetPixel(0, 0, GameManager.playerColors[i]);
            tex.Apply();
            float width = ((float)Screen.width * factionNumbers[i]) / numCamps;
            GUI.DrawTexture(new Rect(runningOffset, 0, width, 30), tex);
            runningOffset += width;
        }
    }

    private void DrawEndgameUI()
    {
        GUI.Label(new Rect((Screen.width - 200)/2, Screen.height/2 - 100, 200, 100), "Player " + winningPlayer + " was victorious");
        if (GUI.Button(new Rect((Screen.width - 200)/2, Screen.height/2, 200, 100), "Main Menu"))
        {
            GameManager.gameState = GameManager.State.MainMenu;
        }
    }
}
