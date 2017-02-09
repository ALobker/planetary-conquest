using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { Menu, Playing, Post };
    public static State gameState = State.Menu;

    public static int numPlayers = 3;
    public static Color[] playerColors = { Color.white, Color.cyan, Color.magenta, Color.yellow, Color.blue, Color.red, Color.green, Color.gray };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
