﻿// written by: Shane Barry, Thomas Stewart
// tested by: Michael Quinn
// debugged by: Shane Barry, Thomas Stewart

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameStates;

/// <summary>
/// NewGame is a MonoBehavior that controls the New Game menu and has methods 
/// that are called when the menu's buttons are pressed. 
/// </summary>
public class NewGame : MonoBehaviour
{
    //initilized in editor
    public Toggle onePlayer;
    public Toggle twoPlayer;
    public Toggle threePlayer;
    public Toggle fourPlayer;
    public Toggle cooperative;
    public Toggle competative;
    public Toggle teams;
    public Toggle campaign;
    public Toggle custom;
    public Toggle survival;
    public Toggle easy;
    public Toggle normal;
    public Toggle hard;
    public Button startGameButton;

	void Start ()
    {
        //remove later once these features are implimented
        custom.interactable = false;
        teams.interactable = false;
	}

    private void OnEnable()
    {
        startGameButton.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("InterfaceCancel"))
        {
            Back();
        }
    }

    /// <summary>
    /// Method called by the Start button, creates a new Level with the settings selected by the user
    /// </summary>
    public void StartGame()
    {
        int numPlayers = 1;
        bool pvp = false;
        float difficulty = 2;

        //set numPlayers depending on num players setting
        if (onePlayer.isOn) 
            numPlayers = 1;
        else if (twoPlayer.isOn)
            numPlayers = 2;
        else if (threePlayer.isOn)
            numPlayers = 3;
        else if (fourPlayer.isOn)
            numPlayers = 4;
        else
            throw new System.Exception("No number players setting was selected when trying to start a new game.");

        //set pvp depending on competitive type setting
        if (cooperative.isOn)
            pvp = false;
        else if (competative.isOn)
            pvp = true;
        else if (teams.isOn)
            pvp = false;//to impliment
        else
            throw new System.Exception("No competative setting was selected when trying to start a new game.");

        //set difficulty depending on difficulty setting
        if (easy.isOn)
            difficulty = 1;
        else if (normal.isOn)
            difficulty = 2;
        else if (hard.isOn)
            difficulty = 3;
        else
            throw new System.Exception("No difficulty setting was selected when trying to start a new game.");

        //if campaign is selected, Create Level1 of the campain
        if (campaign.isOn) 
        {
            Level level1 = Level.GetLevel(1);
            level1.Create(numPlayers, difficulty, (int)System.DateTime.Now.Ticks, pvp);
        }
        else if (custom.isOn)
        {
            //to impliment later
        }
        //if survival is selected, create LevelSurvival
        else if (survival.isOn)
        {
            Level levelSurvival = (Instantiate(Resources.Load(Level.LEVEL_PATH + "LevelSurvivalPF"))
                as GameObject).GetComponent<Level>();
            levelSurvival.Create(numPlayers, difficulty, (int)System.DateTime.Now.Ticks, pvp);
        }
        else
        {
            throw new System.Exception("No game type was selected when trying to start a new game.");
        }

        gameState = GameState.Playing;
    }

    /// <summary>
    /// method the back button calls, changes the screen to the Main menu
    /// </summary>
    public void Back()
    {
        gameState = GameState.Main;
    }
}
