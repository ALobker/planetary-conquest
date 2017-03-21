using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour
{
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
    private float uiScale = 1f;

    // Use this for initialization
    void Start()
    {
        if (campParent == null)
            campParent = GameObject.Find("Camps").transform;
        cc = GetComponent<CreateCamps>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.button.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.toggle.fontSize = Mathf.RoundToInt(25f * uiScale);

        if (GameManager.gameState == GameManager.State.MainMenu)
            DrawMainMenu();
        else if (GameManager.gameState == GameManager.State.Playing)
            DrawIngameUI();
        else if (GameManager.gameState == GameManager.State.End)
            DrawEndgameUI();
        else if (GameManager.gameState == GameManager.State.SinglePlayerMenu)
            DrawCreationMenu();
        else if (GameManager.gameState == GameManager.State.MultiPlayerMenu)
            DrawMultiplayerMenu();
        else if (GameManager.gameState == GameManager.State.OptionsMenu)
            DrawOptionsMenu();
        else
            DrawCreationMenu();
    }

    private void DrawMainMenu()
    {
        int yOffset = Mathf.RoundToInt(Screen.height - (5 * 50 * uiScale)) / 2;
        if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2, yOffset, 200 * uiScale, 50 * uiScale), "Tutorial"))
        {
            selectedPlayers = 3;
            selectedSize = 1;
            cc.numCamps = 12;
            winningPlayer = 0;
            cc.CreateAllCamps(true);
            GameManager.gameState = GameManager.State.Playing;
        }
        else if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2, yOffset + 50 * uiScale, 200 * uiScale, 50 * uiScale), "Single Player"))
            GameManager.gameState = GameManager.State.SinglePlayerMenu;
        else if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2, yOffset + 100 * uiScale, 200 * uiScale, 50 * uiScale), "Multiplayer"))
            GameManager.gameState = GameManager.State.MultiPlayerMenu;
        else if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2, yOffset + 150 * uiScale, 200 * uiScale, 50 * uiScale), "Options"))
            GameManager.gameState = GameManager.State.OptionsMenu;
        else if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2, yOffset + 200 * uiScale, 200 * uiScale, 50 * uiScale), "Credits"))
            GameManager.gameState = GameManager.State.CreditsMenu;
    }

    private void DrawCreationMenu()
    {
        int buttonHeight = 35;
        int offsetX = (Screen.width - 600) / 2;
        int playersY = 100;
        int campsY = playersY + buttonHeight + 10;

        GUI.Label(new Rect(offsetX, playersY, 100, buttonHeight), "Players");
        for (int i = 2; i <= 10; i++)
        {
            if (GUI.Toggle(new Rect(offsetX + 50 * i, playersY, 50, buttonHeight), selectedPlayers == i, "" + i))
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

        float boxWidth = Mathf.Min(800f * uiScale, Screen.width), boxHeight = Mathf.Min(500f * uiScale,Screen.height);
        float boxOffsetX = (Screen.width - boxWidth) / 2f, boxOffsetY = (Screen.height - boxHeight) / 2f;
        float divide = (boxWidth * 3f / 4f), colblocks = 7;
        float col1 = boxOffsetX, col2 = boxOffsetX + 2 * divide / colblocks, col3 = boxOffsetX + 3 * divide / colblocks, col4 = boxOffsetX + 5 * divide / colblocks, col5 = boxOffsetX + 6 * divide / colblocks;
        float lineHeight = boxHeight / 12f;
        GUI.Label(new Rect(col1, boxOffsetY, col2 - col1, lineHeight), "Players");
        GUI.Label(new Rect(col2, boxOffsetY, col3 - col2, lineHeight), "Ping");
        GUI.Label(new Rect(col3, boxOffsetY, col4 - col3, lineHeight), "Faction/Civ/Race");
        GUI.Label(new Rect(col4, boxOffsetY, col5 - col4, lineHeight), "Color");
        GUI.Label(new Rect(col5, boxOffsetY, boxOffsetX + divide - col5, lineHeight), "Team");
        for (int i = 0; i < 10; i++)
        {
            GUI.Label(new Rect(col1, boxOffsetY + lineHeight * (i + 1), col2 - col1, lineHeight), "Player " + (i + 1));
            GUI.Label(new Rect(col2, boxOffsetY + lineHeight * (i + 1), col3 - col2, lineHeight), "0");
            GUI.Label(new Rect(col3, boxOffsetY + lineHeight * (i + 1), col4 - col3, lineHeight), "Default");
            GUI.Label(new Rect(col4, boxOffsetY + lineHeight * (i + 1), col5 - col4, lineHeight), "Red");
            GUI.Label(new Rect(col5, boxOffsetY + lineHeight * (i + 1), boxOffsetX + divide - col5, lineHeight), "-");
        }
        //chat
        //right col
        //map settings
        //size
        GUI.Label(new Rect(boxOffsetX + divide + 25, boxOffsetY, boxWidth - divide, lineHeight), "Size");
        if (GUI.Toggle(new Rect(boxOffsetX + divide + 25, boxOffsetY + lineHeight, boxWidth - divide, lineHeight), selectedSize == 0, "Small"))
        {
            selectedSize = 0;
            cc.numCamps = 6;
        }
        if (GUI.Toggle(new Rect(boxOffsetX + divide + 25, boxOffsetY + 2f * lineHeight, boxWidth - divide, lineHeight), selectedSize == 1, "Medium"))
        {
            selectedSize = 1;
            cc.numCamps = 12;
        }
        if (GUI.Toggle(new Rect(boxOffsetX + divide + 25, boxOffsetY + 3f * lineHeight, boxWidth - divide, lineHeight), selectedSize == 2, "Large"))
        {
            selectedSize = 2;
            cc.numCamps = 25;
        }
        if (GUI.Toggle(new Rect(boxOffsetX + divide + 25, boxOffsetY + 4f * lineHeight, boxWidth - divide, lineHeight), selectedSize == 3, "Huge"))
        {
            selectedSize = 3;
            cc.numCamps = 50;
        }
        if (GUI.Toggle(new Rect(boxOffsetX + divide + 25, boxOffsetY + 5f * lineHeight, boxWidth - divide, lineHeight), selectedSize == 4, "Giant"))
        {
            selectedSize = 4;
            cc.numCamps = 80;
        }
        //impassible
        //ai settings
        //difficulty
        //destruction
        //TODO: dropdowns maken
        //TODO: kleuren exclusive maken
        //TODO: player disablen als niet gekozen wordt

        //ready

        if (GUI.Button(new Rect((Screen.width - 200) / 2, campsY + buttonHeight + 10, 200, buttonHeight), "Start"))
        {
            winningPlayer = 0;
            GameManager.gameState = GameManager.State.Playing;
            cc.CreateAllCamps();
        }
        if (GUI.Button(new Rect(50, Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    private void DrawMultiplayerMenu()
    {
        int buttonHeight = 35;
        //join lan
        //join online
        //host game
        if (GUI.Button(new Rect(50, Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    private void DrawOptionsMenu()
    {
        int buttonHeight = 35;
        //sound effect volume

        //music volume

        //ui size
        float currentPos = Mathf.Log(uiScale) / Mathf.Log(2);
        float scale = Mathf.Round(GUI.HorizontalSlider(new Rect((Screen.width - 300) / 2, Screen.height / 2 - 20, 300, 30 * uiScale), currentPos, -1f, 1f) * 10f) / 10f;
        uiScale = Mathf.Pow(2f, scale);
        GUI.Label(new Rect((Screen.width - 300 - 35f * uiScale) / 2, Screen.height / 2, 35 * uiScale + 5, 25 * uiScale + 5), "0.5");
        GUI.Label(new Rect((Screen.width - 15f * uiScale) / 2, Screen.height / 2, 15 * uiScale, 25 * uiScale + 5), "1");
        GUI.Label(new Rect((Screen.width + 300 - 15f * uiScale) / 2, Screen.height / 2, 15 * uiScale, 25 * uiScale + 5), "2");

        //colorblind mode
        //"Off"
        //"Deuteranopia/Protanopia"
        if (GUI.Button(new Rect(50, Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    private void DrawIngameUI()
    {
        int[] factionNumbers = new int[GameManager.numPlayers + 1];
        int numCamps = 0;
        foreach (Transform camp in campParent)
        {
            if (!camp.gameObject.activeSelf)
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
        for (int i = 0; i < factionNumbers.Length; i++)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.SetPixel(0, 0, GameManager.playerColors[i]);
            tex.Apply();
            float width = ((float)Screen.width * factionNumbers[i]) / numCamps;
            GUI.DrawTexture(new Rect(runningOffset, 0, width, 30), tex);
            runningOffset += width;
        }
    }

    private void DrawEndgameUI()
    {
        GUI.Label(new Rect((Screen.width - 200) / 2, Screen.height / 2 - 100, 200, 100), "Player " + winningPlayer + " was victorious");
        if (GUI.Button(new Rect((Screen.width - 200) / 2, Screen.height / 2, 200, 100), "Main Menu"))
        {
            GameManager.gameState = GameManager.State.MainMenu;
        }
    }
}
