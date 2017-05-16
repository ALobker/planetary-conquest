using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { MainMenu, SinglePlayerMenu, MultiPlayerMenu, OptionsMenu, CreditsMenu, Playing, End };
    public static State gameState = State.MainMenu;

    public static int numPlayers = 3;
    //public static Color[] playerColors = { Color.white, Color.cyan, Color.magenta, Color.yellow, Color.blue, Color.red, Color.green, Color.gray };
    public static Color[] colors = {  new Color(1,1,1), //white
                                            new Color(.86f, .43f, 0), //orange
                                            new Color(0, .43f, .86f), //blue
                                            new Color(.71f, .43f, 1), //light purple
                                            new Color(1, .43f, .71f), //pink
                                            new Color(.71f, .86f, 1), //light blue
                                            new Color(1, 1, .43f), //light yellow
                                            new Color(1, .71f, .86f), //light pink
                                            new Color(.57f, 0, 0), //dark red
                                            new Color(.29f, 0, .57f), //dark purple
                                            new Color(0, .57f, .57f), //aqua
                                            new Color(0,0,0) };
    public static int[] playerColors = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // new int[11];
    public static int[] playerType = { 1, 3, 3, 0, 0, 0, 0, 0, 0, 0 };
    public static int[] playerCiv = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public static string[] civNames = { "Random", "A", "B", "C" };
    public static string[] typeNames = { "None", "Player", "Easy", "Medium", "Hard" };

    public Transform planet;

    // Use this for initialization
    void Start()
    {
        if (planet == null)
        {
            planet = GameObject.Find("Planet").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == State.Playing)
        {
            planet.rotation = Quaternion.Euler(planet.rotation.eulerAngles - Vector3.up * Time.deltaTime);
        }
    }
}
