﻿using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour 
{
	public GameLogic game;
	public AudioClip grunt;

	private int health;
	private int energy;

	// Use this for initialization
	void Start () 
	{
		health = 100;
		energy = 100;
		energyCounter = 0;
	}

	private int energyRefreshCount = 100;
	private int energyCounter;
	// Update is called once per frame
	void Update ()
	{
		energyCounter++;
		if (energyCounter > energyRefreshCount)
		{
			if (energy < 100)
				energy += 10;
			energyCounter = 0;
		}
	}

	public void respawn()
	{
		transform.position = game.RandomPointOnPlane();
	}

	public void dealDamage(int damageToDeal)
	{
		health -= damageToDeal;
		print ("Taking damage!");
		// Player is dead
		if (health < 0)
		{
			respawn ();
		}
		// Player just takes damage
		else
		{
			AudioSource.PlayClipAtPoint(grunt,transform.position);
		}
	}

	public void useEnergy(int energyCost)
	{
		energy -= energyCost;
	}

	public int getHealth()
	{
		return health;
	}

	public int getEnergy()
	{
		return energy;
	}
}
