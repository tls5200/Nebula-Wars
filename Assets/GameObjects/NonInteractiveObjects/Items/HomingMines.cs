﻿// written by: Thomas Stewart
// tested by: Michael Quinn
// debugged by: Diane Gregory, Shane Barry
// balanced by: Diane Gregory, Metin Erman, Thomas Stewart

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HomingMines is an Item that allows its holder to create upto a certain number
/// of (HomingMine)s. 
/// </summary>
public class HomingMines : Item
{
    protected const float USE_POINTS = -10f;

    public float mineDamage = 25f;
    public float mineHealth = 20f;
    public float mineAcceleration = 0.5f;
    public float mineMaxSpeed = 15f;
    public float findTargetProximity = 5f;
    public float loseTargetProximity = 10f;
    public Vector2 offset = new Vector2(0, -2);
    public int maxMines = 5;

    public float layTimeSecs = 1f;
    private int layTimer = 0;

    private LinkedList<HomingMine> mines = new LinkedList<HomingMine>();

    //if this Item is dropped, destroy all of it's mines
    protected override void DropItem()
    {
        foreach (HomingMine item in mines)
        {
            if (item != null)
                item.DestroyThis();
        }

        mines.Clear();
    }

    protected override void HoldingItem(bool use, bool startUse, bool endUse, bool doubleUse)
    {
        //Find and remove from the list any mines that no longer exist
        List<HomingMine> remove = new List<HomingMine>();
        foreach (HomingMine item in mines)
        {
            if (item == null || !item.active)
            {
                remove.Add(item);
            }
        }
        foreach (HomingMine item in remove)
        {
            mines.Remove(item);
        }

        //update the time before the next mine can be layed
        if (layTimer > 0)
        {
            layTimer--;
        }
        //if it is time to lay a mine and the holder is pressing this Item's key, 
        //then create a new mine behind the holder
        else if (use)
        {
            //create a mine behind the holder
            Vector2 direction = holder.velocity.normalized;
            direction.x *= -1;
            HomingMine mine = (HomingMine)level.CreateObject("HomingMinePF", holder.position + offset.Rotate(direction.GetAngle()));

            //set mine initial settings
            mine.acceleration = mineAcceleration;
            mine.damage = mineDamage;
            mine.health = mineHealth;
            mine.maxSpeed = mineMaxSpeed;
            mine.color = color;
            mine.team = holder.team;

            //if there are too many mines, destroy the oldest
            if (mines.Count >= maxMines)
            {
                mines.Last.Value.DestroyThis();
                mines.RemoveLast();
            }

            mines.AddFirst(mine);

            //reset timer to lay next mine
            layTimer = (int)(layTimeSecs * level.updatesPerSec);

            level.score += USE_POINTS;
}
    }

    protected override void PickupItem()
    {
        
    }
}
