using UnityEngine;
using System.Collections;


public class FacePlayerFadeOutAndMoveUp : MonoBehaviour
{
	private PlayerLogic player;
	public float upwardSpeed;

    void Start()
    {
        player = GetObjects.getPlayer();
    }

	void Update()
	{
		transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
		transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
		transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));
		float distanceMovedUp = Time.deltaTime * upwardSpeed;
		transform.position += new Vector3 (0.0f, distanceMovedUp, 0.0f);
	}
}


