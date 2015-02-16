using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour 
{
	public GameObject spawnThis;
	public GameObject spawnedEnemyList;

	public float spawnFrequency;

	private bool spawning = true;
	
	// Use this for initialization
	void Start () 
	{
		StartCoroutine (spawnLoop ());
	}
	
	// Update is called once per frame
	void Update () 
	{	
	}

	public void stopSpawning()
	{
		print ("Stopping spawning");
		spawning = false;
	}

	public void startSpawning()
	{
		spawning = true;
	}

	IEnumerator spawnLoop()
	{
		while(true)
		{
			if (spawning)
			{
				GameObject monster = (GameObject) Instantiate (spawnThis, transform.position, Quaternion.identity);
				monster.transform.parent = spawnedEnemyList.transform;
				BasicEnemyController enemy = (BasicEnemyController) monster.GetComponent(typeof(BasicEnemyController));
				enemy.enabled = true;
			}
			yield return new WaitForSeconds(spawnFrequency);
		}
	}
}

