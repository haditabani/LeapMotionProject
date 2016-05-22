using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmitterBehaviorBallista : MonoBehaviour 
{
	public GameObject bolt;
	public float emissionVelocity;
	public float emissionFrequency;
	public float attackRadius;
	public bool multiAttack;
	private Animator anim;
	private int boltTimer;
	private int blockTimer;
	public Vector3 boltHeightOffset;
	public AudioClip arrowFire;
	private bool firing;
	private AudioSource source;
	public GameObject ballistaDestroyParticle;
	public AudioClip ballistaBreakingSound;

	// Use this for initialization
	void Start () 
	{
		boltTimer = 0;
		anim = gameObject.GetComponent<Animator> ();
		firing = false;
		source = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{	
//		// Only make the bolt if you're the server for synchronization purposes (so the two bolts generated at the same time
//		// won't have different hashes)
//		if (network.isServer)
//		{
		// Acquire the nearest target
		Collider[] nearbyObjects = Physics.OverlapSphere (transform.position, attackRadius);
		float minDistance = float.MaxValue;
		BasicEnemyController nearestEnemy = null;
		TutorialEnemyController nearestTutEnemy = null;
		for (int i = 0; i < nearbyObjects.Length; i++)
		{
			if (nearbyObjects[i].transform.childCount > 0)
			{
				BasicEnemyController enemy = (BasicEnemyController) nearbyObjects[i].gameObject.GetComponent(typeof(BasicEnemyController));
				TutorialEnemyController enemyTut = (TutorialEnemyController) nearbyObjects[i].gameObject.GetComponent(typeof(TutorialEnemyController));
				if (enemy != null && !enemy.isMonsterDying() && !enemy.isAlly)
				{
					float distance = Vector3.Distance (transform.position, enemy.transform.position);
					if (distance < minDistance)
					{
						minDistance = distance;
						nearestEnemy = enemy;
					}
				}

				if (enemyTut != null)
				{
					float distance = Vector3.Distance (transform.position, enemyTut.transform.position);
					if (distance < minDistance)
					{
						minDistance = distance;
						nearestTutEnemy = enemyTut;
					}
				}
			}
		}

		if (nearestEnemy != null)
		{
			// Turn to face the enemy
			transform.rotation = Quaternion.LookRotation(nearestEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

			// Launch bolt as long as an enemy is found
			boltTimer++;
			if (boltTimer > emissionFrequency && !firing)
			{
				boltTimer = 0;
				Vector3 velocity = transform.forward.normalized;
				Vector3 startPosition = transform.position + velocity * 1;
				// Apply the correct velocity to the emission
				velocity *= emissionVelocity;
				GameObject newbolt = createbolt(startPosition, transform.rotation, velocity, 0);
				MoveBolt bolt = (MoveBolt) newbolt.GetComponent(typeof(MoveBolt));
				bolt.setTarget(nearestEnemy.gameObject);

				// Create Coroutine to stop firing after playing the animation once
				StartCoroutine (shootBolt());
			}
		}
		// TODO: CHANGE THIS
		else if (nearestTutEnemy != null)
		{
			// Turn to face the enemy
			transform.rotation = Quaternion.LookRotation(nearestTutEnemy.transform.position - transform.position);
			transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
			
			// Launch bolt as long as an enemy is found
			boltTimer++;
			if (boltTimer > emissionFrequency)
			{
				boltTimer = 0;
				Vector3 velocity = transform.forward.normalized;
				Vector3 startPosition = transform.position + velocity * 1;
				// Apply the correct velocity to the emission
				velocity *= emissionVelocity;
				GameObject newbolt = createbolt(startPosition, transform.rotation, velocity, 0);
				MoveBolt bolt = (MoveBolt) newbolt.GetComponent(typeof(MoveBolt));
				bolt.setTarget(nearestTutEnemy.gameObject);

				// Create Coroutine to stop firing after playing the animation once
				StartCoroutine (shootBolt());
			}
		}
		else
		{
			transform.rotation = Quaternion.identity;
		}
	}
	
//	[RPC]
//	public void makeboltNetwork(Vector3 position, Quaternion rotation, Vector3 velocity, int hashValue)
//	{
//		createbolt (position, rotation, velocity, hashValue);
//	}
	
	public GameObject createbolt(Vector3 position, Quaternion rotation, Vector3 velocity, int hashValue)
	{
		GameObject newbolt = (GameObject) Instantiate(bolt, position - boltHeightOffset, rotation);

		newbolt.SetActive(true); 
		MoveBolt moveThis = (MoveBolt) newbolt.GetComponent(typeof(MoveBolt));
		moveThis.enabled = true;
		moveThis.setVelocity(velocity);
//		newbolt.GetComponent<Renderer>().enabled = true;
		moveThis.setHash (0);

		return newbolt;
//		Movebolt moveThis = (Movebolt) newbolt.GetComponent(typeof(Movebolt));
//		moveThis.setVelocity(velocity);
//		newbolt.GetComponent<Renderer>().enabled = true;
//		moveThis.setHash (hashValue);
//		// Also enable this particle's self destruct feature
//		ProjectileDestroy destroyThis = (ProjectileDestroy) newbolt.GetComponent (typeof(ProjectileDestroy));
//		destroyThis.enabled = true;
		//return newbolt;
	}

	public void highlight(bool highlightOrNot)
	{
		// If highlighted show attack radius sphere
		if (highlightOrNot)
		{
			gameObject.transform.GetChild (0).gameObject.GetComponent<Renderer>().enabled = true;
		}
		// Else hide attack radius sphere
		else
		{
			gameObject.transform.GetChild (0).gameObject.GetComponent<Renderer>().enabled = false;
		}
	}

	IEnumerator shootBolt()
	{
		anim.SetBool ("Firing", true);
		source.PlayOneShot (arrowFire);
		firing = true;
//		while(!anim.GetCurrentAnimatorStateInfo(0).IsName("BallistaFire"))
//		{
//		print ("Length of shoot: " + anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
		yield return new WaitForSeconds(1.0f);
//		}
		//		print ("Animation time: " + anim.GetCurrentAnimationClipState (0) [0].clip.length);
//		yield return new WaitForSeconds (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length - .1f);

		anim.SetBool ("Firing", false);
		firing = false;
	}

	void OnTriggerEnter(Collider other) 
	{
		BasicEnemyController enemy = (BasicEnemyController) other.gameObject.GetComponent(typeof(BasicEnemyController));
		//		if (other.name == "CrazyFireball(Clone)") {
		//			print("EXPLOSION!");
		//			Vector3 boomPos = transform.position + new Vector3(0f,1f,0f);
		//			Instantiate (boom, boomPos, Quaternion.identity);
		//			Destroy(gameObject);
		//		}
		if (enemy != null) 
		{
			StartCoroutine( WaitForAnimation ());
		}
	}

//	public GameObject ballistaDestroyParticle;
//	public AudioClip ballistaBreakingSound;
//

	private IEnumerator WaitForAnimation ()
	{
		anim.SetBool("Breaking", true);
		// Generate a firey explosion here to signify the ballista got destroyed
		GameObject breakingExplosion = (GameObject) Instantiate (ballistaDestroyParticle, transform.position, Quaternion.Euler(270.0f, 0.0f, 0.0f));
		breakingExplosion.SetActive (true);
		// Play a sound when the ballista gets killed
		source.PlayOneShot (ballistaBreakingSound);
		yield return new WaitForSeconds (4);
		Destroy (gameObject.transform.parent.gameObject);
	}
}
