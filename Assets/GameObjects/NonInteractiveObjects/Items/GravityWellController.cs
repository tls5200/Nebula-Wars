﻿// written by: Thomas Stewart
// tested by: Michael Quinn
// debugged by: Diane Gregory, Shane Barry
// balanced by: Diane Gregory, Metin Erman, Thomas Stewart

using UnityEngine;
using System.Collections;

/// <summary>
/// GravityWellController is an Item that allows its holder to create and control a GravityWell
/// </summary>
public class GravityWellController : Item
{
    protected const float USE_POINTS = -0.001f;

    public Vector2 offset = new Vector2(0, 3);
    public float wellDamage = 10f;
    public float startGravity = 0.5f;
    public float maxGravity = 10f;
    public float gravityIncreaseSpeed = 0.1f;

    private GravityWell well;

    //If this Item is dropped, destroy its GravityWell if it currrently has one
    protected override void DropItem()
    {
        if (well != null)
        {
            well.DestroyThis();
            well = null;
        }
    }

    protected override void HoldingItem(bool use, bool startUse, bool endUse, bool doubleUse)
    {
        //When the holder first presses this Item's key, create a GravityWell 
        //behind the holder if there isn't one, if there is one move it behind the holder. 
        if (startUse)
        {
            Vector2 direction = holder.velocity.normalized;
            direction.x *= -1;
            if (well == null)
            {
                well = (GravityWell)level.CreateObject("GravityWellPF", holder.position + offset.Rotate(direction.GetAngle()));
                well.damage = wellDamage;
                well.gravity = startGravity;
            }
            else
            {
                well.position = holder.position + offset.Rotate(direction.GetAngle());
            }
        }
        //When the holder holds down this Item's key, increase the GravityWell's size.
        else if (use && well != null)
        {
            well.gravity *= 1f + gravityIncreaseSpeed;

            if (well.gravity > maxGravity)
            {
                well.gravity = maxGravity;
            }
        }

        //When the holder double presses this Item's key, destroy the GravityWell if one exists.
        if (doubleUse && well != null)
        {
            well.DestroyThis();
            well = null;
        }

        if (well != null)
        {
            level.score += USE_POINTS;
        }
    }

    protected override void PickupItem()
    {
        
    }
}
