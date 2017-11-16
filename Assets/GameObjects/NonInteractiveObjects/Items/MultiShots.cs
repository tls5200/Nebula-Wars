﻿using UnityEngine;
using System.Collections;

public class MultiShots : Item
{
    public float numberOfShots = 8f;
    public float damageEach = 10f;
    public float shotSpeed = 20f;
    public float shotLifeSecs = 3f;
    public Vector2 offset = new Vector2(0, 2f);
    public Vector2 shotScale = new Vector2(0.25f, 0.025f);

    public float shotTimeSecs = 2f;
    private int shotTimer = 0;
    private int shotCooldown = 0;

    public float spreadStart = 30f;
    public float spreadMax = 180f;
    private float spreadSpeed;
    private float spread;

    protected override void dropItem()
    {
        
    }

    private void shoot()
    {
        if (spread < spreadStart)
        {
            spread = spreadStart;
        }

        float currentAngle = 0;

        for (int i = 0; i < numberOfShots; i++)
        {
            currentAngle = holder.angle - spread / 2.0f + i * spread / (numberOfShots - 1);

            LazerShot current = (LazerShot)level.createObject("LazerShotPF", holder.position + offset.rotate(currentAngle), 
                currentAngle, new Vector2().toAngle(currentAngle, shotSpeed) + holder.velocity);
            current.damage = damageEach;
            current.timeToLiveSecs = shotLifeSecs;
            current.color = color;
            current.team = holder.team;
        }

        shotCooldown = (int)(shotTimeSecs * level.updatesPerSec);
        shotTimer = 0;
        spread = 0;
    }

    protected override void holdingItem(bool use, bool startUse, bool endUse, bool doubleUse)
    {
        if (shotCooldown > 0)
        {
            shotCooldown--;
        }
        else if (endUse)
        {
            shoot(); 
        }

        if (shotTimer > 0)
        {
            shotTimer--;

            if (shotTimer <= 0)
            {
                shoot();
            }
        }

        if (use)
        {
            if (shotTimer <= 0)
            {
                shotTimer = (int)(shotTimeSecs * level.updatesPerSec);
                spread = spreadStart;
            }
            else
            {
                spread += spreadSpeed;

                if (spread > spreadMax)
                {
                    spread = spreadMax;
                }
            }
        }
    }

    protected override void pickupItem()
    {
        spreadSpeed = (spreadMax - spreadStart) / (shotTimeSecs * level.updatesPerSec);
    }
}