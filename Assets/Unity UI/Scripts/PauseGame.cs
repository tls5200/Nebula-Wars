﻿// written by: Shane Barry, Thomas Stewart, Metin Erman
// tested by: Michael Quinn
// debugged by: Shane Barry, Thomas Stewart

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static GameStates;

/// <summary>
/// PauseGame is a MonoBehavior that controls the Pause Game menu and
/// has methods that are called when the menus' buttons are pressed.
/// </summary>
public class PauseGame : MonoBehaviour
{
    //initilized in editor
    public Button resumeGameButton;

    private void OnEnable()
    {
        resumeGameButton.Select();
    }

    // method used by the resume button on the pause menu to change the screen back to Playing or Replay
    public void Unpause()
    {
        gameState = previousPlay;
	}

    /// <summary>
    /// Method called by the Restart button on the gameover menu to set level to its beginning
    /// </summary>
    public void Restart()
    {
        //if there is not a current Level, display error message
        if (Level.current == null)
        {
            DialogBox.Create("Current Level is null, can't restart", "Error");
        }
        else
        {
            Level.current.RestartLevel();
            Unpause();
        }
	}

    /// <summary>
    /// Method the Options button calls, changes the screen to the Options Hub screen
    /// </summary>
    public void Options()
    {
        gameState = GameState.Options;
	}

    /// <summary>
    /// Method called by the Quit button, destroys the current Level and changes the screen to the Main menu 
    /// </summary>
    public void Exit()
    {
        if (Level.current != null)
        {
            Destroy(Level.current.gameObject);
        }
        gameState = GameState.Main; // sets gamestate back to main menu
    }
}
