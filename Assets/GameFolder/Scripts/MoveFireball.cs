﻿using UnityEngine;
using System.Collections;

public class MoveFireball : MonoBehaviour {
	public GameLogic game;

	public GameObject explosion;

	private Vector3 velocity;

	private int hashValue;

	// Use this for initialization
	void Awake () 
	{
		hashValue = 0; // Default hash value
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (velocity != null)
		{
			gameObject.transform.position += velocity;
		}
	}

	public void setVelocity(Vector3 newVelocity)
	{
		velocity = newVelocity;
	}

	public void setHash(int newHash)
	{
		hashValue = newHash;
	}

	public int getHash()
	{
		return hashValue;
	}


	void OnCollisionEnter (Collision col)
	{
		// Ignore collisions with player's hands
		if (col.gameObject.name == "palm" || col.gameObject.name == "bone1" || col.gameObject.name == "bone2")
		{

		}
		// Check if a player or their leap motion hands are hit
		else if (col.gameObject.name == "LeapOVRPlayerController")
		{
			if (!game.isPlayerBlocking())
			{
				Instantiate (explosion, transform.position, Quaternion.identity);
				PlayerLogic hitPlayer = (PlayerLogic) col.gameObject.GetComponent(typeof(PlayerLogic));
				hitPlayer.respawn();
			}
			// Reflect back blocked fireballs
			else
			{
				reverseVelocity();
				game.reverseProjectileOnOtherClients(hashValue);
			}
		}
		else if (col.gameObject.name == "Zombie(Clone)")
		{
			print ("Hit this!");
			print (velocity);
			col.gameObject.rigidbody.AddForce(velocity * 60, ForceMode.Impulse);
			BasicEnemyController enemy = (BasicEnemyController) col.transform.GetChild(0).GetComponent(typeof(BasicEnemyController));
			enemy.dealDamage(10);
			Instantiate (explosion, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
		else
		{
			Instantiate (explosion, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}

	public void reverseVelocity()
	{
		velocity *= -1.0f;
	}
}
