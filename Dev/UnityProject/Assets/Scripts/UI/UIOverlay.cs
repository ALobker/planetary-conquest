using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlay : MonoBehaviour
{
    public Transform campParent;
    public Texture2D buttonBackground;

    public float volumeMusic = 0.9f, volumeEffects = 0.9f;

    private CreateCamps cc;
    private int selectedSize = 0;
    private int winningPlayer = 0;

    private int showDropdownType = -1;
    private int showDropdownFaction = -1;
    private int showDropdownColor = -1;
    private Rect popupSize = new Rect();

    Texture2D[] texBlocks;

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
    private float uiSlideOffset = 0;
    private float uiSlideDir = 0;

    // Use this for initialization
    void Start()
    {
        if (campParent == null)
            campParent = GameObject.Find("Camps").transform;
        cc = GetComponent<CreateCamps>();

        int colorboxsize = Mathf.RoundToInt(Mathf.Min(500f * uiScale, Screen.height) / 10f);
        texBlocks = new Texture2D[10];
        for (int i = 0; i < 10; i++)
        {
            texBlocks[i] = new Texture2D(colorboxsize, colorboxsize, TextureFormat.RGBA32, false);
            for (int j = 0; j < colorboxsize; j++)
            {
                for (int k = 0; k < colorboxsize; k++)
                {
                    texBlocks[i].SetPixel(j, k, GameManager.colors[i + 1]);
                }
            }
            texBlocks[i].Apply();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (uiSlideDir != 0)
        {
            uiSlideOffset += Time.deltaTime * uiSlideDir * 2f;
            if (uiSlideOffset > 1f)
            {
                uiSlideOffset = 1f;
                uiSlideDir = 0;
            }
            else if (uiSlideOffset < 0)
            {
                uiSlideOffset = 0;
                uiSlideDir = 0;
            }
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.button.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.toggle.fontSize = Mathf.RoundToInt(25f * uiScale);
        if (buttonBackground != null)
            GUI.skin.button.normal.background = buttonBackground;
        GUI.skin.button.normal.textColor = Color.white;

        if (GameManager.gameState == GameManager.State.Playing)
        {
            DrawIngameUI();
        }
        else
        {
            DrawMainMenu();
            if (GameManager.gameState == GameManager.State.MainMenu)
            { }
            else if (GameManager.gameState == GameManager.State.End)
                DrawEndgameUI();
            else if (GameManager.gameState == GameManager.State.SinglePlayerMenu)
                DrawCreationMenu();
            else if (GameManager.gameState == GameManager.State.MultiPlayerMenu)
                DrawMultiplayerMenu();
            else if (GameManager.gameState == GameManager.State.OptionsMenu)
                DrawOptionsMenu();
            else if (GameManager.gameState == GameManager.State.CreditsMenu)
                DrawCreditsMenu();
            else
                DrawCreationMenu();
        }
    }

    private void DrawMainMenu()
    {
        float lineHeight = 50;
        float xOffset = ((Screen.width - 200f * uiScale) / 2f) - Screen.width - subScreenOffset();
        float yOffset = Mathf.Round(Screen.height - (5 * lineHeight * uiScale)) / 2;
        if (GUI.Button(new Rect(xOffset, yOffset, 200 * uiScale, lineHeight * uiScale), "Tutorial"))
        {
            selectedSize = 0;
            cc.numCamps = 6;
            for (int i = 1; i <= 3; i++)
            {
                GameManager.playerColors[i] = i;
            }
            uiSlideOffset = 1;
            winningPlayer = 0;
            cc.CreateAllCamps(true);
            GameManager.gameState = GameManager.State.Playing;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * uiScale, 200 * uiScale, lineHeight * uiScale), "Single Player"))
        {
            uiSlideDir = 1f;
            GameManager.gameState = GameManager.State.SinglePlayerMenu;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 2 * uiScale, 200 * uiScale, lineHeight * uiScale), "Multiplayer"))
        {
            uiSlideDir = 1f;
            GameManager.gameState = GameManager.State.MultiPlayerMenu;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 3 * uiScale, 200 * uiScale, lineHeight * uiScale), "Options"))
        {
            uiSlideDir = 1f;
            GameManager.gameState = GameManager.State.OptionsMenu;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 4 * uiScale, 200 * uiScale, lineHeight * uiScale), "Credits"))
        {
            uiSlideDir = 1f;
            GameManager.gameState = GameManager.State.CreditsMenu;
        }
    }

    private void DrawCreationMenu()
    {
        int buttonHeight = 35;

        //ui settings
        float boxWidth = Mathf.Min(800f * uiScale, Screen.width), boxHeight = Mathf.Min(500f * uiScale, Screen.height);
        float boxOffsetX = (Screen.width - boxWidth) / 2f - subScreenOffset(), boxOffsetY = (Screen.height - boxHeight) / 2f;
        float divide = (boxWidth * 3f / 4f), colblocks = 7f;
        float col1 = boxOffsetX, col2 = boxOffsetX + 2f * divide / colblocks, col3 = boxOffsetX + 3f * divide / colblocks, col4 = boxOffsetX + 5f * divide / colblocks, col5 = boxOffsetX + 6f * divide / colblocks;
        float lineHeight = boxHeight / 12f;

        //main ui area
        GUI.Label(new Rect(col1, boxOffsetY, col2 - col1, lineHeight), "Players");
        GUI.Label(new Rect(col2, boxOffsetY, col3 - col2, lineHeight), "Ping");
        GUI.Label(new Rect(col3, boxOffsetY, col4 - col3, lineHeight), "Faction/Civ/Race");
        GUI.Label(new Rect(col4, boxOffsetY, col5 - col4, lineHeight), "Color");
        GUI.Label(new Rect(col5, boxOffsetY, boxOffsetX + divide - col5, lineHeight), "Team");
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                GUI.Label(new Rect(col1, boxOffsetY + lineHeight, col2 - col1, lineHeight), "Player");
            }
            else
            {
                if (GUI.Button(new Rect(col1, boxOffsetY + lineHeight * (i + 1), col2 - col1, lineHeight), GameManager.typeNames[GameManager.playerType[i]])) //"Player " + (i + 1)
                {
                    if (showDropdownType > -1 && showDropdownType == i)
                        showDropdownType = -1;
                    else
                        showDropdownType = i;
                    showDropdownFaction = -1;
                    showDropdownColor = -1;
                }
            }
            if (GameManager.playerType[i] == 0)
                GUI.enabled = false;
            GUI.Label(new Rect(col2, boxOffsetY + lineHeight * (i + 1), col3 - col2, lineHeight), "0");
            if (GUI.Button(new Rect(col3, boxOffsetY + lineHeight * (i + 1), col4 - col3, lineHeight), GameManager.civNames[GameManager.playerCiv[i]]))
            {
                if (showDropdownFaction > -1 && showDropdownFaction == i)
                    showDropdownFaction = -1;
                else
                    showDropdownFaction = i;
                showDropdownType = -1;
                showDropdownColor = -1;
            }
            if (GameManager.playerColors[i + 1] == 0)
            {
                if (GUI.Button(new Rect(col4, boxOffsetY + lineHeight * (i + 1), col5 - col4, lineHeight), "Rand"))
                {
                    if (showDropdownColor > -1 && showDropdownColor == i)
                        showDropdownColor = -1;
                    else
                        showDropdownColor = i;
                    showDropdownType = -1;
                    showDropdownFaction = -1;
                }
            }
            else
            {
                if (GUI.Button(new Rect(col4, boxOffsetY + lineHeight * (i + 1), col5 - col4, lineHeight), texBlocks[GameManager.playerColors[i + 1] - 1]))
                {
                    if (showDropdownColor > -1 && showDropdownColor == i)
                        showDropdownColor = -1;
                    else
                        showDropdownColor = i;
                    showDropdownType = -1;
                    showDropdownFaction = -1;
                }
            }
            GUI.Label(new Rect(col5, boxOffsetY + lineHeight * (i + 1), boxOffsetX + divide - col5, lineHeight), "-");
            GUI.enabled = true;
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
        //TODO: impassible
        //TODO: destruction


        //show dropdowns
        if (showDropdownType > -1)
        {
            popupSize = new Rect(col1, boxOffsetY + lineHeight * (showDropdownType + 2), col2 - col1, lineHeight * 4f + 10f);
            GUI.Window(0, popupSize, DrawDropdown, "");
        }
        if (showDropdownFaction > -1)
        {
            popupSize = new Rect(col3, boxOffsetY + lineHeight * (showDropdownFaction + 2), col4 - col3, lineHeight * 4f + 10f);
            GUI.Window(1, popupSize, DrawDropdown, "");
        }
        if (showDropdownColor > -1)
        {
            int numColors = GameManager.playerColors.Count(i => i == 0) + (GameManager.playerColors[showDropdownColor + 1] != 0 ? 1 : 0);
            popupSize = new Rect(col4, boxOffsetY + lineHeight * (showDropdownColor + 2), col5 - col4 + 20, (lineHeight - 5f) * numColors + 15f);
            GUI.Window(2, popupSize, DrawDropdown, "");
        }


        //main buttons
        if (GUI.Button(new Rect(Screen.width - 250 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Start"))
        {
            //set players by skipping every 'none'
            int[] playercolors = new int[11];
            int[] playertype = new int[10];
            int[] playerciv = new int[10];
            int index = 0;
            for (int i = 0; i < 10; i++)
            {
                if (GameManager.playerType[i] > 0)
                {
                    playercolors[index + 1] = GameManager.playerColors[i + 1];
                    playertype[index] = GameManager.playerType[i];
                    playerciv[index] = GameManager.playerCiv[i];
                    index++;
                }
            }
            GameManager.numPlayers = index;
            for (int i = 0; i < 10; i++)
            {
                if (GameManager.playerType[i] == 0)
                {
                    playercolors[index + 1] = 0;
                    playertype[index] = 0;
                    playerciv[index] = 0;
                    index++;
                }
            }
            //Debug.Log(intArrToString(GameManager.playerColors));
            //Debug.Log(intArrToString(GameManager.playerType));
            //Debug.Log(intArrToString(GameManager.playerCiv));
            //Debug.Log(intArrToString(playercolors));
            //Debug.Log(intArrToString(playertype));
            //Debug.Log(intArrToString(playerciv));
            GameManager.playerColors = playercolors;
            GameManager.playerType = playertype;
            GameManager.playerCiv = playerciv;

            if (GameManager.numPlayers > 6 && selectedSize == 0)
            {
                selectedSize = 1;
                cc.numCamps = 12;
            }

            //set remaining colors
            int[] available = Enumerable.Range(1, 10).Except(GameManager.playerColors).ToArray();
            int[] shuffled = available.OrderBy(x => UnityEngine.Random.Range(0, 1f)).ToArray();
            int shufind = 0;
            for (int i = 0; i < 10; i++)
            {
                if (GameManager.playerColors[i + 1] == 0)
                {
                    GameManager.playerColors[i + 1] = shuffled[shufind];
                    shufind++;
                }
            }
            //Debug.Log(intArrToString(GameManager.playerColors));

            winningPlayer = 0;
            GameManager.gameState = GameManager.State.Playing;
            cc.CreateAllCamps();
        }
        if (GUI.Button(new Rect(50 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            uiSlideDir = -1f;
        }
    }

    private void DrawDropdown(int windowID)
    {
        float offset = 0;
        float lineHeight = Mathf.Min(500f * uiScale, Screen.height) / 12f;
        if (windowID == 0)
        {
            for (int i = 0; i < GameManager.typeNames.Length; i++)
            {
                if (i == 1) continue;
                if (GUI.Button(new Rect(5, 5 + offset, popupSize.width - 10, lineHeight), GameManager.typeNames[i]))
                {
                    GameManager.playerType[showDropdownType] = i;
                    showDropdownType = -1;
                }
                offset += lineHeight;
            }
        }
        else if (windowID == 1)
        {
            for (int i = 0; i < GameManager.civNames.Length; i++)
            {
                if (GUI.Button(new Rect(5, 5 + offset, popupSize.width - 10, lineHeight), GameManager.civNames[i]))
                {
                    GameManager.playerCiv[showDropdownFaction] = i;
                    showDropdownFaction = -1;
                }
                offset += lineHeight;
            }
        }
        else if (windowID == 2)
        {
            if (GUI.Button(new Rect(5, 5, popupSize.width - 10, lineHeight), "Rand"))
            {
                GameManager.playerColors[showDropdownColor + 1] = 0;
                //Debug.Log(intArrToString(GameManager.playerColors));
                showDropdownColor = -1;
            }
            for (int i = 1; i <= 10; i++)
            {
                int index = Array.IndexOf(GameManager.playerColors, i);
                if (index > -1 && i != GameManager.playerColors[showDropdownColor + 1])
                    continue;

                offset += lineHeight - 5f;
                if (GUI.Button(new Rect(5, 10 + offset, popupSize.width - 10, (lineHeight - 5f)), texBlocks[i - 1]))
                {
                    GameManager.playerColors[showDropdownColor + 1] = i;
                    //Debug.Log(intArrToString(GameManager.playerColors));
                    showDropdownColor = -1;
                }
            }
        }
    }

    private void DrawMultiplayerMenu()
    {
        int buttonHeight = 35;
        //join lan
        //join online
        //host game
        if (GUI.Button(new Rect(50 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            uiSlideDir = -1f;
            //GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    private void DrawOptionsMenu()
    {
        int buttonHeight = 35;
        //sound effect volume
        volumeEffects = GUI.HorizontalSlider(new Rect((Screen.width - 300) / 2 - subScreenOffset(), Screen.height / 2 - 220, 300, 30 * uiScale), volumeEffects, 0, 1f);
        GUI.Label(new Rect((Screen.width + 300) / 2 - subScreenOffset(), Screen.height / 2 - 220, 50 * uiScale, buttonHeight * uiScale), "" + Mathf.RoundToInt(volumeEffects * 100));

        //music volume
        volumeMusic = GUI.HorizontalSlider(new Rect((Screen.width - 300) / 2 - subScreenOffset(), Screen.height / 2 - 120, 300, 30 * uiScale), volumeMusic, 0, 1f);
        GUI.Label(new Rect((Screen.width + 300) / 2 - subScreenOffset(), Screen.height / 2 - 120, 50 * uiScale, buttonHeight * uiScale), "" + Mathf.RoundToInt(volumeMusic * 100));

        //ui size
        float currentPos = Mathf.Log(uiScale) / Mathf.Log(2);
        float scale = Mathf.Round(GUI.HorizontalSlider(new Rect((Screen.width - 300) / 2 - subScreenOffset(), Screen.height / 2 - 20, 300, 30 * uiScale), currentPos, -1f, 1f) * 10f) / 10f;
        uiScale = Mathf.Pow(2f, scale);
        GUI.Label(new Rect((Screen.width - 300 - 35f * uiScale) / 2 - subScreenOffset(), Screen.height / 2, 35 * uiScale + 5, 25 * uiScale + 5), "0.5");
        GUI.Label(new Rect((Screen.width - 15f * uiScale) / 2 - subScreenOffset(), Screen.height / 2, 15 * uiScale, 25 * uiScale + 5), "1");
        GUI.Label(new Rect((Screen.width + 300 - 15f * uiScale) / 2 - subScreenOffset(), Screen.height / 2, 15 * uiScale, 25 * uiScale + 5), "2");

        //colorblind mode
        //"Off"
        //"Deuteranopia/Protanopia"
        if (GUI.Button(new Rect(50 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            uiSlideDir = -1f;
            //GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    private void DrawCreditsMenu()
    {
        int buttonHeight = 35;
        //about us
        //art
        //models
        //programming
        //design
        //ai
        if (GUI.Button(new Rect(50 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            uiSlideDir = -1f;
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

        GUIStyle defaultStyle = GUI.skin.label;
        int aliveFactions = 0;
        defaultStyle.normal.textColor = GameManager.colors[0];
        GUI.Label(new Rect(10, 40, 300, 40), "Empty\t" + factionNumbers[0]);

        for (int i = 1; i < factionNumbers.Length; i++)
        {
            defaultStyle.normal.textColor = GameManager.colors[GameManager.playerColors[i]];
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
            tex.SetPixel(0, 0, GameManager.colors[GameManager.playerColors[i]]);
            tex.Apply();
            float width = ((float)Screen.width * factionNumbers[i]) / numCamps;
            //GUI.DrawTexture(new Rect(runningOffset, 0, width, 30), tex);
            EditorGUI.DrawPreviewTexture(new Rect(runningOffset, 0, width, 30), tex);
            runningOffset += width;
        }
    }

    private void DrawEndgameUI()
    {
        int buttonHeight = 35;
        GUI.Label(new Rect((Screen.width - 300 * uiScale) / 2 - subScreenOffset(), Screen.height / 2 - 100, 300 * uiScale, buttonHeight * 2 * uiScale), "Player " + winningPlayer + " was victorious");
        if (GUI.Button(new Rect((Screen.width - 200 * uiScale) / 2 - subScreenOffset(), Screen.height / 2, 200 * uiScale, buttonHeight * uiScale), "Main Menu"))
        {
            uiSlideDir = -1f;
            //GameManager.gameState = GameManager.State.MainMenu;
        }
    }

    public static string intArrToString(int[] arr)
    {
        string outstr = "";
        foreach (int item in arr)
        {
            outstr += item + ",";
        }
        return outstr;
    }

    private float subScreenOffset()
    {
        if (uiSlideOffset == 0 || uiSlideOffset == 1)
            return (uiSlideOffset - 1) * Screen.width;
        //float offset = 0.5f - 0.5f * Mathf.Cos(uiSlideOffset * Mathf.PI);
        float offset = 0.5f + 0.5f * (float)Math.Tanh(5.0 * (uiSlideOffset - 0.5));
        return (offset - 1) * Screen.width;
    }
}
