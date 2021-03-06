﻿// written by: Thomas Stewart
// tested by: Michael Quinn
// debugged by: Diane Gregory, Shane Barry
// balanced by: Diane Gregory, Metin Erman, Thomas Stewart

using UnityEngine;
using System.Collections;

/// <summary>
/// Acceleraten is an Item that can be activated or deactivated. When activated, 
/// it increases its holder's acceleration and decreses their armor. 
/// </summary>
public class Accelerant : Item
{
    protected const float USE_POINTS = -0.001f;

    public float accelerationPerSecGain = 15f;
    public float armorLoss = 2.0f;

    private bool activated = false;

    protected override void DropItem()
    {
        //if activated when dropped, deactivate before dropping.
        if (activated)
        {
            holder.accelerationPerSec -= accelerationPerSecGain;
            holder.armor += armorLoss;
            activated = false;
        }
    }

    protected override void HoldingItem(bool use, bool startUse, bool endUse, bool doubleUse)
    {
        //if the user presses this item's corropsoing key, toggle this item's activation.
        if (startUse)
        {
            activated = !activated;

            //if it is activated, increase holder's acceleration and decrease their armor.
            if (activated)
            {
                holder.accelerationPerSec += accelerationPerSecGain;
                holder.armor -= armorLoss;
            }
            //if it is deactivated, decrease the holder's acceleration and increase their armor
            //to bring the values back to normal.
            else
            {
                holder.accelerationPerSec -= accelerationPerSecGain;
                holder.armor += armorLoss;
            }
        }

        if (activated)
        {
            level.score += USE_POINTS;
        }
    }

    protected override void PickupItem()
    {
        
    }
}
