﻿// written by: Thomas Stewart
// tested by: Michael Quinn
// debugged by: Diane Gregory, Thomas Stewart

using UnityEngine;
using System.Collections;

/// <summary>
/// DestructableObjects are InteractiveObjects that have health which can be damaged 
/// and are destroyed when they lose all their health
/// </summary>
public abstract class DestructableObject : InteractiveObject
{
    private float theMaxHealth;
    public float health = 100;
    public float armor = 1;

    private GameObject healthBar;

    public float maxHealth
    {
        get
        {
            return theMaxHealth;
        }
        set
        {
            theMaxHealth = value;
        }
    }

    /// <summary>
    /// Deal damage to this. Its armor will mitigate some of the damage.
    /// If its health decreases to 0 or less, it will be destroyed next time it is updated.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void DamageThis(float damage)
    {
        if (damage > armor)
        {
            health -= (damage - armor);

            if (team <= 0)
                level.score += damage - armor;
        }
    }

    // Called shortly after this is created
    protected abstract void StartDestructableObject();
    protected override void StartInteractiveObject()
    {
        //if there is no level, this should not exist, so it is destoryed
        if (level == null) 
        {
            Debug.Log("Destroying " + this + " since level is null when it is being created.");
            Destroy(this.gameObject);
        }
        else
        {
            StartDestructableObject();
            if (this.GetType() != typeof(Player))
            {
                //add this to the Level's lists of what it contains
                level.AddToGame(this);
            }

            theMaxHealth = health;

            //create a healBar for this DestructableObject if it does not exist
            if (healthBar == null)
            {
                healthBar = Instantiate(Resources.Load("HealthBarPF"), new Vector3(position.x, position.y, -5), Quaternion.Euler(0, 0, 0)) as GameObject;
                healthBar.GetComponent<HealthBar>().owner = this;
            }
        }
    }

    // Called every time the game is FixedUpated, 50 times a second by default
    protected abstract void UpdateDestructableObject();
    protected override void UpdateInteractiveObject()
    {
        UpdateDestructableObject();

        //if this has 0 or less health, deactivate it if is a Player, otherwise destroy it
        if (health <= 0)
        {
            if (this.GetType() == typeof(Player))
            {
                active = false;
            }
            else
            {
                DestroyThis();
            }
        }       
    }

    
    /// <summary>
    /// get: return whether this is active or not
    /// set: activate and deactivate this DestructableObject and it's HealthBar
    /// </summary>
    public override bool active
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);

            if (healthBar == null)
            {
                healthBar = Instantiate(Resources.Load("HealthBarPF"), new Vector3(position.x, position.y, -5), Quaternion.Euler(0, 0, 0)) as GameObject;
                healthBar.GetComponent<HealthBar>().owner = this;
            }

            healthBar.SetActive(value);
        }
    }

    // Called right before this is destroyed
    // removes this from the Level's lists and destroys its healhbar
    protected abstract void DestroyDestructableObject();
    protected override void DestroyInteractiveObject()
    {
        DestroyDestructableObject();
        if (GetType() != typeof(Player))
        {
            if (level != null && level.destructables != null)
            {
                level.RemoveFromGame(this);
            }
            Destroy(healthBar);
        }
    }

    /// <summary>
    /// Called by Unity when this GameObject collides with another GameObject
    /// Calls another method depending on the collision type to categorize the collision
    /// Calculates 
    /// </summary>
    /// <param name="collision">holds the properties of the collision</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        SpaceObject spaceObject = collision.gameObject.GetComponent<SpaceObject>();

        if (spaceObject.GetType() == (typeof(Player)))
            PlayerCollision((Player)spaceObject, collision);
        else if (spaceObject.GetType().IsSubclassOf(typeof(DestructableObject)))
            DestructableObjectCollision((DestructableObject)spaceObject, collision);
        else if (spaceObject.GetType().IsSubclassOf(typeof(IndestructableObject)))
            IndestructableObjectCollision((IndestructableObject)spaceObject, collision);
        else
            Debug.Log(this.ToString() + " collided with an unknown type of object: " + spaceObject.ToString());
    }
}
