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
    public GUISkin skin;
    public Font MenuFont;
    public AudioClip soundMenuNext, soundMenuPrev;

    public float volumeMusic = 0.9f, volumeEffects = 0.9f;

    private CreateCamps cc;
    private int selectedSize = 1;
    private int winningPlayer = 0;

    private int subMenu = 0;
    private int showDropdownType = -1;
    private int showDropdownFaction = -1;
    private int showDropdownColor = -1;
    private Rect popupSize = new Rect();

    private Texture2D[] texBlocks;
    private Font defaultFont;
    private AudioSource audioSource;

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

        defaultFont = skin.label.font;
        audioSource = GetComponent<AudioSource>();
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
        GUI.skin = skin;
        GUI.skin.label.padding.left = 5;
        GUI.skin.label.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.button.fontSize = Mathf.RoundToInt(25f * uiScale);
        GUI.skin.toggle.fontSize = Mathf.RoundToInt(25f * uiScale);
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
            else
                DrawCreationMenu();
        }
    }

    private void DrawMainMenu()
    {
        float lineHeight = 50;
        float buttonWidth = 250;
        float xOffset = (Screen.width / 5f) - Screen.width - subScreenOffset();
        float yOffset = Mathf.Min(Mathf.Round(Screen.height - (5 * 1.5f * lineHeight * uiScale)) / 2, Screen.height / 4);

        skin.label.font = MenuFont;
        GUI.Label(new Rect(xOffset, yOffset, buttonWidth + lineHeight * 0.5f, lineHeight * uiScale), "Main menu");
        skin.label.font = defaultFont;

        if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 1.5f * uiScale, buttonWidth, lineHeight * uiScale), "Tutorial"))
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
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 1.5f * 2 * uiScale, buttonWidth, lineHeight * uiScale), "Single Player"))
        {
            uiSlideDir = 1f;
            //play next
            audioSource.clip = soundMenuNext;
            audioSource.Play();
            GameManager.gameState = GameManager.State.SinglePlayerMenu;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 1.5f * 3 * uiScale, buttonWidth, lineHeight * uiScale), "Multiplayer"))
        {
            //uiSlideDir = 1f;
            //play next
            audioSource.clip = soundMenuNext;
            audioSource.Play();
            //GameManager.gameState = GameManager.State.MultiPlayerMenu;
            subMenu = 1;
        }
        else if (GUI.Button(new Rect(xOffset, yOffset + lineHeight * 1.5f * 4 * uiScale, buttonWidth, lineHeight * uiScale), "Settings"))
        {
            //uiSlideDir = 1f;
            //play next
            audioSource.clip = soundMenuNext;
            audioSource.Play();
            subMenu = 2;
        }

        GUI.Window(subMenu, new Rect(xOffset + buttonWidth + lineHeight * 0.5f, yOffset, (3 * Screen.width) / 5f - buttonWidth - lineHeight * 0.5f, Screen.height - yOffset * 2), DrawSubMenu, "");
    }

    private void DrawCreationMenu()
    {
        int buttonHeight = 35;

        //ui settings
        float boxWidth = Mathf.Min(800f * uiScale, Screen.width), boxHeight = Mathf.Min(500f * uiScale, Screen.height);
        float boxOffsetX = (Screen.width - boxWidth) / 2f - subScreenOffset(), boxOffsetY = (Screen.height - boxHeight) / 2f;
        float divide = (boxWidth * 3f / 4f), colblocks = 7f;
        float col1 = boxOffsetX, col2 = boxOffsetX + 2f * divide / colblocks, col3 = boxOffsetX + 3f * divide / colblocks, col4 = boxOffsetX + 5f * divide / colblocks, col5 = boxOffsetX + 6f * divide / colblocks;
        float lineHeight = Mathf.Round(boxHeight / 12f);

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
            else if (GUI.Button(new Rect(col1, boxOffsetY + lineHeight * (i + 1), col2 - col1, lineHeight), GameManager.typeNames[GameManager.playerType[i]])) //"Player " + (i + 1)
            {
                if (showDropdownType > -1 && showDropdownType == i)
                    showDropdownType = -1;
                else
                    showDropdownType = i;
                showDropdownFaction = -1;
                showDropdownColor = -1;
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
            popupSize = new Rect(col4, boxOffsetY + lineHeight * (showDropdownColor + 2), col5 - col4, (lineHeight - 5f) * numColors + 15f);
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
            //play prev
            audioSource.clip = soundMenuPrev;
            audioSource.Play();
        }
    }

    private void DrawSubMenu(int windowID)
    {
        int buttonHeight = 35;

        if (windowID == 0)
        {
            skin.label.font = MenuFont;
            GUI.Label(new Rect(5, 5, 300 * uiScale, buttonHeight * 2 * uiScale), "Planetary Conquest");
            skin.label.font = defaultFont;
        }
        else if (windowID == 1)
        {
            skin.label.font = MenuFont;
            GUI.Label(new Rect(5, 5, 300 * uiScale, buttonHeight * uiScale), "Multiplayer");
            skin.label.font = defaultFont;

            //host
            //join lan
            //join internet
            if (GUI.Button(new Rect(5, 5 + buttonHeight * uiScale + 20f, 100 * uiScale, buttonHeight * uiScale), "Host"))
            {
                uiSlideDir = 1f;
                //play next
                audioSource.clip = soundMenuNext;
                audioSource.Play();
                GameManager.gameState = GameManager.State.MultiPlayerMenu;
            }
            GUI.TextField(new Rect(5, 5 + buttonHeight * 2 * uiScale + 30f, 100 * uiScale, buttonHeight * uiScale), "IP");
            if (GUI.Button(new Rect(5 + 100 * uiScale + 10f, 5 + buttonHeight * 2 * uiScale + 30f, 100 * uiScale, buttonHeight * uiScale), "Join"))
            {
                uiSlideDir = 1f;
                //play next
                audioSource.clip = soundMenuNext;
                audioSource.Play();
                GameManager.gameState = GameManager.State.MultiPlayerMenu;
            }
        }
        else if (windowID == 2)
        {
            skin.label.font = MenuFont;
            GUI.Label(new Rect(5, 5, 300 * uiScale, buttonHeight * uiScale), "Settings");
            skin.label.font = defaultFont;

            //sound effect volume
            volumeEffects = GUI.HorizontalSlider(new Rect(20, 50, 300, 20), volumeEffects, 0, 1f);
            GUI.Label(new Rect(330, 45, 50 * uiScale, buttonHeight * uiScale), "" + Mathf.RoundToInt(volumeEffects * 100));
            audioSource.volume = 0.5f * volumeEffects;

            //music volume
            volumeMusic = GUI.HorizontalSlider(new Rect(20, 100, 300, 20), volumeMusic, 0, 1f);
            GUI.Label(new Rect(330, 95, 50 * uiScale, buttonHeight * uiScale), "" + Mathf.RoundToInt(volumeMusic * 100));

            //ui size
            float currentPos = Mathf.Log(uiScale) / Mathf.Log(2);
            float scale = Mathf.Round(GUI.HorizontalSlider(new Rect(20, 150, 300, 20), currentPos, -.5f, .5f) * 10f) / 10f;
            uiScale = Mathf.Pow(2f, scale);
            GUI.Label(new Rect(20f - (32 * uiScale + 10) / 2, 170, 32 * uiScale + 10, 25 * uiScale + 6), "0.5");
            GUI.Label(new Rect(170f - (12 * uiScale + 10) / 2, 170, 12 * uiScale + 10, 25 * uiScale + 6), "1");
            GUI.Label(new Rect(320f - (12 * uiScale + 10) / 2, 170, 12 * uiScale + 10, 25 * uiScale + 6), "2");
        }
    }

    private void DrawDropdown(int windowID)
    {
        float offset = 0;
        float lineHeight = Mathf.Round(Mathf.Min(500f * uiScale, Screen.height) / 12f);
        if (windowID == 0)
        {
            for (int i = 0; i < GameManager.typeNames.Length; i++)
            {
                if (i == 1) continue;

                //selected option
                string prefix = "";
                if (GameManager.playerType[showDropdownType] == i)
                    prefix = "* ";

                if (GUI.Button(new Rect(5, 5f + offset, popupSize.width - 10, lineHeight), prefix + GameManager.typeNames[i]))
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
                //selected option
                string prefix = "";
                if (GameManager.playerCiv[showDropdownFaction] == i)
                    prefix = "* ";

                if (GUI.Button(new Rect(5, 5f + offset, popupSize.width - 10, lineHeight), prefix + GameManager.civNames[i]))
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
                if (GUI.Button(new Rect(5, 10f + offset, popupSize.width - 10, (lineHeight - 5f)), texBlocks[i - 1]))
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

        if (GUI.Button(new Rect(50 - subScreenOffset(), Screen.height - 50 - buttonHeight * uiScale, 200 * uiScale, buttonHeight * uiScale), "Back"))
        {
            uiSlideDir = -1f;
            //play prev
            audioSource.clip = soundMenuPrev;
            audioSource.Play();
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

        int lineHeight = Mathf.RoundToInt(36 * uiScale);
        GUIStyle defaultStyle = GUI.skin.label;
        int aliveFactions = 0;
        defaultStyle.normal.textColor = GameManager.colors[0];
        defaultStyle.padding.left = 15;
        GUI.Label(new Rect(0, 40, 250 * uiScale, lineHeight), "Empty\t" + factionNumbers[0], defaultStyle);

        for (int i = 1; i < factionNumbers.Length; i++)
        {
            //background color
            Texture2D tex = new Texture2D(100, 1, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            Color c = GameManager.colors[GameManager.playerColors[i]];
            if (factionNumbers[i] == 0)
                c = new Color(c.grayscale, c.grayscale, c.grayscale);
            for (int j = 0; j < 100; j++)
            {
                c.a = (100f - j) / 100f;
                tex.SetPixel(j, 0, c);
            }
            tex.Apply();

            defaultStyle.normal.textColor = GameManager.colors[GameManager.playerColors[i]];
            defaultStyle.normal.background = tex;
            GUI.Label(new Rect(0, 40 + i * (lineHeight + 3), 250 * uiScale, lineHeight), "Player " + i + "\t" + factionNumbers[i], defaultStyle);
            if (factionNumbers[i] > 0)
            {
                aliveFactions++;
                winningPlayer = i;
            }
        }
        defaultStyle.normal.textColor = Color.white;
        defaultStyle.normal.background = null;
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
            audioSource.clip = soundMenuPrev;
            audioSource.Play();
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
