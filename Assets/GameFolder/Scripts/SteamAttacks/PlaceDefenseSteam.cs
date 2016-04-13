using UnityEngine;
using UnityEngine.UI;

public class PlaceDefenseSteam : SteamAttacks
{
	public PlayerLogic player;
	public int defenseCost;
	public GameObject defensiveObject;
	public GameObject defensiveObjectPending;
	public AudioClip placeObjectSound;
	private bool isInstantiated = false;
	private GameObject createdDefensiveObject;
	private AudioSource source;
	public GameObject goldSpentUI;

//	public RayCastTest rayCast;
	
	void Start()
	{
		source = gameObject.GetComponent<AudioSource> ();
	}

	public override void inactiveFunction()
	{
		if (isInstantiated)
		{
			Destroy(createdDefensiveObject);
			isInstantiated = false;
		}
	}
	
	public override void releaseFunction(uint controllerIndex, SteamVR_TrackedObject trackedDevice)
	{
		Debug.Log ("Casting release function");
		if (player.getCurrencyValue() >= defenseCost) 
		{

            Vector3 spawnPosition = trackedDevice.transform.position;

            Quaternion newRotation = trackedDevice.transform.rotation;
            newRotation *= Quaternion.AngleAxis(45, Vector3.right);

            Vector3 forwardVector = newRotation * Vector3.forward;

            RaycastHit hit;
            Physics.Raycast(spawnPosition, forwardVector, out hit, 100.0f);

            if (hit.collider.gameObject != null)
            {
                Quaternion rotation = trackedDevice.transform.rotation;
                GameObject ballistaFinal = (GameObject)Instantiate(defensiveObject, hit.point, Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f));
                ballistaFinal.SetActive(true);
                source.PlayOneShot(placeObjectSound);
                Destroy(createdDefensiveObject);
                isInstantiated = false;
                //player.changeCurrency(-1 * defenseCost);
            }
        }
	}
	
	public override void holdFunction(uint controllerIndex, SteamVR_TrackedObject trackedDevice)
	{
		if (player.getCurrencyValue() >= defenseCost) 
		{
            if (!isInstantiated)
            {
                createdDefensiveObject = (GameObject)Instantiate(defensiveObjectPending);
                createdDefensiveObject.SetActive(true);
                isInstantiated = true;
            }

            Vector3 spawnPosition = trackedDevice.transform.position;

            Quaternion newRotation = trackedDevice.transform.rotation;
            newRotation *= Quaternion.AngleAxis(45, Vector3.right);

            Vector3 forwardVector = newRotation * Vector3.forward;

            RaycastHit hit;
            Physics.Raycast(spawnPosition, forwardVector, out hit, 100.0f);

            if (hit.collider.gameObject != null)
            {
                createdDefensiveObject.transform.position = hit.point;
                Quaternion rotation = trackedDevice.transform.rotation;
                createdDefensiveObject.transform.rotation = Quaternion.Euler(0.0f, rotation.eulerAngles.y, 0.0f);
            }
        }
	}

	public void switchDefense()
	{
		if (isInstantiated)
		{
			isInstantiated = false;
			Destroy (createdDefensiveObject);
		}
	}
}


