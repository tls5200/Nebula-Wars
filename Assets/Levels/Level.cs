﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using static User;

public abstract class Level : MonoBehaviour
{
    public static Vector2 GAME_SIZE = new Vector2(26.6666f, 20);
    public const int UPDATES_PER_SEC = 60;
    public const float PRECISION = 1024;
    public const string SAVE_PATH = "saves/";
    public const string AUTO_SAVE_EXTENTION = ".NEBULA";
    public const string SAVE_EXTENTION = ".nebula";
    public const string REPLAY_EXTENTION = ".replay";
    public const int MAX_AUTOSAVES = 20;

    private Canvas canvas;

    private bool isReplay = false;
    private System.IO.StreamReader updateFile;

    public abstract int levelNumber{ get; }

    private static Level theCurrentLevel;

    public static Level currentLevel
    {
        get
        {
            return theCurrentLevel;
        }
    }

    private DateTime theStartTime;
    public DateTime startTime
    {
        get
        {
            return theStartTime;
        }
    }

    public TimeSpan duration
    {
        get
        {
            return System.DateTime.Now - startTime;
        }
    }

    private int theDifficulty = 1;
    public int difficulty
    {
        get
        {
            return theDifficulty;
        }
    }

    private int randomSeed;
    private System.Random theRandom;
    public System.Random random
    {
        get
        {
            return theRandom;
        }
    }

    public float getRandomAngle()
    {
        return random.Next((int)(360 * PRECISION)) / PRECISION;
    }

    public Vector2 getRandomVector2(float maxValues)
    {
        return new Vector2(random.Next((int)(maxValues * PRECISION)) / PRECISION,
            random.Next((int)(maxValues * PRECISION)) / PRECISION);
    }

    public Vector2 getRandomVector2(float minValues, float maxValues)
    {
        return new Vector2(random.Next((int)(minValues * PRECISION), (int)(maxValues * PRECISION)) / PRECISION,
           random.Next((int)(minValues * PRECISION), (int)(maxValues * PRECISION)) / PRECISION);
    }

    public Vector2 getRandomPosition()
    {
        return new Vector2(random.Next((int)(GAME_SIZE.x * PRECISION)) / PRECISION,
            random.Next((int)(GAME_SIZE.y * PRECISION)) / PRECISION);
    }

    public Vector2 getRandomVelocity(float maxSpeed)
    {
        float angle = getRandomAngle() * Mathf.Deg2Rad;

        return new Vector2(random.Next((int)(maxSpeed * PRECISION)) / PRECISION * Mathf.Cos(angle),
            random.Next((int)(maxSpeed * PRECISION)) / PRECISION) * Mathf.Sin(angle);
    }

    public SpaceObject createObject(string namePF)
    {
        return createObject(namePF, getRandomPosition(), getRandomAngle());
    }

    public SpaceObject createObject(string namePF, Vector2 position)
    {
        SpaceObject current = createObject(namePF, position, getRandomAngle());
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle)
    {
        UnityEngine.GameObject obj = Instantiate(Resources.Load(namePF), position, Quaternion.Euler(0, 0, angle)) as GameObject;
        return obj.GetComponent<SpaceObject>();
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, Vector2 velocity)
    {
        SpaceObject current = createObject(namePF, position, angle);
        current.velocity = velocity;
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, Vector2 veloctiy, float angularVelocity)
    {
        SpaceObject current = createObject(namePF, position, angle, veloctiy);
        current.angularVelocity = angularVelocity;
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, float speedForward)
    {
        SpaceObject current = createObject(namePF, position, angle);
        current.moveForward(speedForward);
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, float speedForward, float angularVelocity)
    {
        SpaceObject current = createObject(namePF, position, angle, speedForward);
        current.angularVelocity = angularVelocity;
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, Vector2 veloctiy, float angularVelocity, Vector2 scale)
    {
        SpaceObject current = createObject(namePF, position, angle, veloctiy, angularVelocity);
        current.scale = scale;
        return current;
    }

    public SpaceObject createObject(string namePF, Vector2 position, float angle, Vector2 veloctiy, float angularVelocity, float scale)
    {
        SpaceObject current = createObject(namePF, position, angle, veloctiy, angularVelocity);
        current.scale = new Vector2(scale, scale);
        return current;
    }

    protected Text addTextToCanvas(string textToAdd, Vector2 position)
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        GameObject obj = Instantiate(Resources.Load("TextPF"), Vector2.zero, Quaternion.identity) as GameObject;
        //obj.transform.SetParent(canvas.transform);
        Text text = obj.GetComponent<Text>();
        text.text = textToAdd;
        text.transform.position = position;
        return text;
    }


    private LinkedList<DestructableObject> theDestructables = new LinkedList<DestructableObject>();
	public LinkedList<DestructableObject> destructables
	{
		get 
		{
			return theDestructables;
		}
	}

    private LinkedList<IndestructableObject> theIndestructables = new LinkedList<IndestructableObject>();
	public LinkedList<IndestructableObject> indestructables
	{
		get 
		{
			return theIndestructables;
		}
	}

	private LinkedList<NonInteractiveObject> theNonInteractives = new LinkedList<NonInteractiveObject>();
	public LinkedList<NonInteractiveObject> nonInteractives
	{
		get 
		{
			return theNonInteractives;
		}
	}

    private List<Player> initialPlayers;
    private List<Player> thePlayers = new List<Player>(Controls.MAX_PLAYERS);
	public List<Player> players
	{
		get 
		{
			return thePlayers;
		}
	}
    
    public void Start()
    {
        
    }

    protected abstract void initilizeLevel();

    public void initilize(int numPlayers, int difficulty, int randomSeed)
    {
        theCurrentLevel = this;

        this.randomSeed = randomSeed;
        theRandom = new System.Random(randomSeed);

        thePlayers = new List<Player>(numPlayers);

        for (int i = 0; i < numPlayers; i++)
        {
            if (i >= Controls.MAX_PLAYERS)
            {
                break;
            }

            UnityEngine.GameObject obj = Instantiate(Resources.Load("PlayerPF"), 
                new Vector2(GAME_SIZE.x / 2 - (numPlayers - 1) * 2 + i * 4, GAME_SIZE.y / 2) , Quaternion.identity) as GameObject;
            Player current = obj.GetComponent<Player>();
            if(current != null)
            {
                current.playerNum = i;
                players.Add(current);
            }
            else
            {
                Debug.Log("There is a null reference for the current object!!");
            }
        }

        foreach (PlayerControls item in Controls.get().players)
        {
            item.clearInputs();
        }

        isReplay = false;

        initilizeLevel();

        theStartTime = DateTime.Now;
    }

    protected abstract void updateLevel();
    
    public void FixedUpdate()
    {
        if (isReplay)
        {
            Controls.get().updateFromFile(updateFile);
        }
        else
        {
            Controls.get().updateFromInput();
        }

        if (!isReplay && duration.TotalSeconds > 2 && destructables.Count <= 0 && theCurrentLevel == this)
        {
            nextLevel();
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == null || !players[i].inPlay || players[i].health < 0)
            {
                //game over
            }
        }
    }

    private void OnDestroy()
    {
        clearLevel();

        if (theCurrentLevel == this)
        {
            theCurrentLevel = null;
        }
    }

    private void clearLevel()
    {
        foreach (Player item in thePlayers)
        {
            Destroy(item.gameObject);
        }
        thePlayers.Clear();

        foreach (DestructableObject item in theDestructables)
        {
            Destroy(item.gameObject);
        }
        theDestructables.Clear();

        foreach (IndestructableObject item in theIndestructables)
        {
            Destroy(item.gameObject);
        }
        theIndestructables.Clear();

        foreach (NonInteractiveObject item in theNonInteractives)
        {
            if (!item.GetType().IsSubclassOf(typeof(Item)))
            {
                Destroy(item.gameObject);
            }
        }
        theNonInteractives.Clear();
    }

    private void saveItems(System.IO.StreamWriter save)
    {
        saveItems(save, thePlayers);
    }

    private void saveItems(System.IO.StreamWriter save, List<Player> players)
    {
        foreach (Player player in players)
        {
            foreach (Item item in player.items)
            {
                if (item == null)
                {
                    save.WriteLine("");
                    save.WriteLine("");
                }
                else
                {
                    string name = item.ToString();

                    name = name.Substring(0, name.IndexOf('(') + 1);
                    save.WriteLine(name);
                    save.WriteLine(item.getValues());
                }
            }
        }
    }

    private void loadItems(System.IO.StreamReader load)
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < player.items.Length; i++)
            {
                string name = load.ReadLine();
                string values = load.ReadLine();

                if (name == "")
                {
                    player.items[i] = null;
                }
                else
                {
                    Item item = (Item)createObject(name, Vector2.zero, 0);
                    item.loadValues(values);
                    player.items[i] = item;
                    //set item position
                }
            }
        }
    }

    public void restartLevel()
    {
        int numPlayers = thePlayers.Count;

        clearLevel();

        initilize(numPlayers, theDifficulty, randomSeed);

        for (int i = 0; i < initialPlayers.Count; i++)
        {
            for (int j = 0; j < thePlayers[i].items.Length; j++)
            {
                thePlayers[i].items[j] = initialPlayers[i].items[j];
            }
        }
    }
    
    /// <summary>
    /// Makes an autosave for the next Level
    /// Creates the next Level and destroys the current one
    /// Should only be called when the current Level is completed
    /// </summary>
    /// <returns>The next Level</returns>
    public Level nextLevel()
    {
        Level lvl = getLevel(levelNumber + 1);

        if (lvl == null)
        {
            //show victory screen, the game has been beat
            restartLevel();
        }
        else
        {
            save();

            lvl.initilize(thePlayers.Count, theDifficulty, theRandom.Next());

            lvl.initialPlayers = new List<Player>(lvl.players.Count);

            for (int i = 0; i < thePlayers.Count; i++)
            {
                for (int j = 0; j < thePlayers[i].items.Length; j++)
                {
                    lvl.thePlayers[i].items[j] = thePlayers[i].items[j];
                }
                lvl.initialPlayers.Add(lvl.thePlayers[i].clone());
            }

            Destroy(this.gameObject);
        }

        return lvl;
    }

    private bool save()
    {
        System.IO.Directory.CreateDirectory(SAVE_PATH);
        string[] filePaths = System.IO.Directory.GetFiles(SAVE_PATH, "*" + AUTO_SAVE_EXTENTION);

        Array.Sort(filePaths);

        for (int i = 0; i < filePaths.Length - MAX_AUTOSAVES - 1; i++)
        {
            System.IO.File.Delete(filePaths[i]);
        }

        return save(DateTime.Now.ToLocalTime().ToString("yy-MM-dd-HH-mm-ss"), true);
    }

    private bool save(string fileName, bool autoSave)
    {
        System.IO.Directory.CreateDirectory(SAVE_PATH);
        System.IO.StreamWriter file;

        if (autoSave)
        {
            file = new System.IO.StreamWriter(SAVE_PATH + fileName + AUTO_SAVE_EXTENTION, false);
        }
        else
        {
            file = new System.IO.StreamWriter(SAVE_PATH + fileName + SAVE_EXTENTION, false);
        }

        file.WriteLine(Convert.ToString(levelNumber + 1));
        file.WriteLine(Convert.ToString(thePlayers.Count));
        file.WriteLine(Convert.ToString(theDifficulty));
        file.WriteLine(Convert.ToString(random.Next()));

        saveItems(file);

        file.Close();

        return true;
    }

    /// <summary>
    /// Makes a save for the next Level with the given filename
    /// Should only be called when the current level is completed
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>True if successful</returns>
    public bool save(string fileName)
    {
        return save(fileName, false);
    }

    public bool saveReplay(string fileName)
    {
        System.IO.Directory.CreateDirectory(SAVE_PATH);
        System.IO.StreamWriter save = new System.IO.StreamWriter(SAVE_PATH + fileName + REPLAY_EXTENTION, false);

        save.WriteLine(Convert.ToString(levelNumber));
        save.WriteLine(Convert.ToString(thePlayers.Count));
        save.WriteLine(Convert.ToString(theDifficulty));
        save.WriteLine(Convert.ToString(randomSeed));

        saveItems(save, initialPlayers);

        Controls.get().saveInputsToFile(save);

        save.Close();

        return true;
    }

    public static Level getLevel(int levelNum)
    {
        UnityEngine.GameObject obj;

        try
        {
            switch (levelNum)
            {
                case 1:
                    obj = Instantiate(Resources.Load("Level1PF")) as GameObject;
                    break;
                case 2:
                    obj = Instantiate(Resources.Load("Level2PF")) as GameObject;
                    break;
                case 3:
                    obj = Instantiate(Resources.Load("Level3PF")) as GameObject;
                    break;
                case 4:
                    obj = Instantiate(Resources.Load("Level4PF")) as GameObject;
                    break;
                case 5:
                    obj = Instantiate(Resources.Load("Level5PF")) as GameObject;
                    break;
                case 6:
                    obj = Instantiate(Resources.Load("Level6PF")) as GameObject;
                    break;
                case 7:
                    obj = Instantiate(Resources.Load("Level7PF")) as GameObject;
                    break;
                case 8:
                    obj = Instantiate(Resources.Load("Level8PF")) as GameObject;
                    break;
                case 9:
                    obj = Instantiate(Resources.Load("Level9PF")) as GameObject;
                    break;
                default:
                    try
                    {
                        obj = Instantiate(Resources.Load("Level" + levelNum.ToString() + "PF")) as GameObject;
                    }
                    catch
                    {
                        return null;
                    }
                    break;
            }

            return obj.GetComponent<Level>();
        }
        catch
        {
            throw new Exception("Error loading level " + levelNum.ToString());
        }
    }

    public static Level loadLevel(string fileName)
    {
        System.IO.StreamReader save = new System.IO.StreamReader(fileName);

        Level lvl = getLevel(Convert.ToInt32(save.ReadLine()));

        int players = Convert.ToInt32(save.ReadLine());
        int difficulty = Convert.ToInt32(save.ReadLine());
        int randomSeed = Convert.ToInt32(save.ReadLine());

        lvl.initilize(players, difficulty, randomSeed);

        lvl.loadItems(save);

        lvl.initialPlayers = new List<Player>(lvl.players.Count);
        for (int i = 0; i < lvl.thePlayers.Count; i++)
        {
            lvl.initialPlayers[i] = lvl.thePlayers[i].clone();
        }

        save.Close();

        return lvl;
    }


    public static Level loadReplay(string fileName)
    {
        System.IO.StreamReader replay = new System.IO.StreamReader(fileName);

        Level lvl = getLevel(Convert.ToInt32(replay.ReadLine()));

        int players = Convert.ToInt32(replay.ReadLine());
        int difficulty = Convert.ToInt32(replay.ReadLine());
        int randomSeed = Convert.ToInt32(replay.ReadLine());
        lvl.initilize(players, difficulty, randomSeed);

        lvl.loadItems(replay);

        lvl.initialPlayers = new List<Player>(lvl.players.Count);
        for (int i = 0; i < lvl.thePlayers.Count; i++)
        {
            lvl.initialPlayers[i] = lvl.thePlayers[i].clone();
        }
        

        lvl.updateFile = replay;
        lvl.isReplay = true;

        replay.Close();

        return lvl;
    }
}