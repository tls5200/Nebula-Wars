﻿// written by: Thomas Stewart
// tested by: Michael Quinn
// debugged by: Diane Gregory, Shane Barry
// balanced by: Diane Gregory, Metin Erman, Thomas Stewart

using UnityEngine;
using System.Collections;

/// <summary>
/// Armor is an Item that can be activated or deactivated. When activated, 
/// it increase's the holder's armor and makes them slowly lose health. 
/// </summary>
public class Armor : Item
{
    protected const float USE_POINTS = -0.001f;

    public float armorGain = 3.0f;
    public float healthPerSecondLoss = 0.5f;

    private bool activated = false;


    protected override void DropItem()
    {
        //if activated when dropped, deactivate before dropping.
        if (activated)
        {
            holder.armor -= armorGain;
            activated = false;
        }
    }

    protected override void HoldingItem(bool use, bool startUse, bool endUse, bool doubleUse)
    {
        //if the user presses this item's corropsoing key, toggle this item's activation.
        if (startUse)
        {
            activated = !activated;

            //if it is activated, increase holder's armor.
            if (activated)
            {
                 holder.armor += armorGain;
            }
            //if it is deactivated, decrease the holder's armor to bring the value back to normal.
            else
            {
                holder.armor -= armorGain;
            }
        }

        //every update, if activated, decrease the holder's health until the holder runs out of health.
        if (activated)
        {
            if (holder.health > healthPerSecondLoss * level.secsPerUpdate)
            {
                holder.health -= healthPerSecondLoss * level.secsPerUpdate;
                level.score += USE_POINTS;
            }
            //if the holder runs out of health, display that the Item is being deacitvated, then deactivate it.
            else
            {
                IngameInterface.DisplayMessage("Armor deactivated, not enough health remaining!", 5f);
                activated = false;
                holder.armor -= armorGain;
            }
        }
    }

    protected override void PickupItem()
    {
        
    }
}
